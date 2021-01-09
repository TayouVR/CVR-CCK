using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using ABI.CCK.Components;
using ABI.CCK.Scripts.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace ABI.CCK.Scripts.Editor
{
    [InitializeOnLoad]
    public class CCK_BuildManagerWindow : EditorWindow
    {
        public static string Version = "2.2 RELEASE";
        private const string CCKVersion = "2.2 RELEASE (Build 62)";
        private const string supportedUnity = "2019.3.1f1";
        private const string supportedUnityLts = "2019.4.13f1";
        
        string _username;
        string _key;

        public Texture2D abiLogo;
        private bool _attemptingToLogin;
        private bool _loggedIn;
        private bool _hasAttemptedToLogin;
        private bool _allowedToUpload;
        private string _apiUserRank;
        private string _apiCreatorRank;
        Vector2 scrollPos;
        UnityWebRequest _webRequest;
        
        private int _tab;
        private Vector2 _scroll;

        private static PropertyInfo _legacyBlendShapeImporter;
        
        private static PropertyInfo legacyBlendShapeImporter
        {
            get
            {
                if(_legacyBlendShapeImporter != null)
                {
                    return _legacyBlendShapeImporter;
                }

                Type modelImporterType = typeof(ModelImporter);
                _legacyBlendShapeImporter = modelImporterType.GetProperty(
                    "legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                );

                return _legacyBlendShapeImporter;
            }
        }

        [MenuItem("Alpha Blend Interactive/====================")]
        static void Spacer1() {}
        
        [MenuItem("Alpha Blend Interactive/Control Panel (Builder and Settings)")]
        static void Init()
        {
            CCK_BuildManagerWindow window = (CCK_BuildManagerWindow)GetWindow(typeof(CCK_BuildManagerWindow), false, "CCK :: Control Panel");
            window.Show();
        }

        void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
            EditorApplication.playModeStateChanged -= OnEditorStateUpdated;
        }

        void OnEnable()
        {
            abiLogo = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/ABI.CCK/GUIAssets/abibig.png", typeof(Texture2D));
            
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
            EditorApplication.playModeStateChanged -= OnEditorStateUpdated;
            EditorApplication.playModeStateChanged += OnEditorStateUpdated;

            _username = EditorPrefs.GetString("m_ABI_Username");
            _key = EditorPrefs.GetString("m_ABI_Key");
            Login();
        }

        void OnEditorStateUpdated(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorPrefs.SetBool("m_ABI_isBuilding", false);
                EditorPrefs.SetString("m_ABI_TempVersion", Version);
                if (File.Exists(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRAvatar.prefab")) File.Delete(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRAvatar.prefab");
                if (File.Exists(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRSpawnable.prefab")) File.Delete(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRSpawnable.prefab");
                if (File.Exists(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRWorld.prefab")) File.Delete(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRWorld.prefab");
                if (File.Exists(Application.persistentDataPath + "/bundle.cvravatar")) File.Delete(Application.persistentDataPath + "/bundle.cvravatar");
                if (File.Exists(Application.persistentDataPath + "/bundle.cvravatar.manifest")) File.Delete(Application.persistentDataPath + "/bundle.cvravatar.manifest");
                if (File.Exists(Application.persistentDataPath + "/bundle.cvrprop")) File.Delete(Application.persistentDataPath + "/bundle.cvrprop");
                if (File.Exists(Application.persistentDataPath + "/bundle.cvrprop.manifest")) File.Delete(Application.persistentDataPath + "/bundle.cvrprop.manifest");
                if (File.Exists(Application.persistentDataPath + "/bundle.cvrworld")) File.Delete(Application.persistentDataPath + "/bundle.cvrworld");
                if (File.Exists(Application.persistentDataPath + "/bundle.cvrworld.manifest")) File.Delete(Application.persistentDataPath + "/bundle.cvrworld.manifest");
                if (File.Exists(Application.persistentDataPath + "/bundle.png")) File.Delete(Application.persistentDataPath + "/bundle.png");
                if (File.Exists(Application.persistentDataPath + "/bundle.png.manifest")) File.Delete(Application.persistentDataPath + "/bundle.png.manifest");
                if (File.Exists(Application.persistentDataPath + "/bundle_pano_1024.png")) File.Delete(Application.persistentDataPath + "/bundle_pano_1024.png");
                if (File.Exists(Application.persistentDataPath + "/bundle_pano_1024.png.manifest")) File.Delete(Application.persistentDataPath + "/bundle_pano_1024.png.manifest");
                if (File.Exists(Application.persistentDataPath + "/bundle_pano_4096.png")) File.Delete(Application.persistentDataPath + "/bundle_pano_4096.png");
                if (File.Exists(Application.persistentDataPath + "/bundle_pano_4096.png.manifest")) File.Delete(Application.persistentDataPath + "/bundle_pano_4096.png.manifest");
                AssetDatabase.Refresh();
            }

            if (state == PlayModeStateChange.EnteredPlayMode && EditorPrefs.GetBool("m_ABI_isBuilding"))
            {
                var ui = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ABI.CCK/GUIAssets/CCK_UploaderHead.prefab"));
                OnGuiUpdater up = ui.GetComponentInChildren<OnGuiUpdater>();
                if (File.Exists(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRAvatar.prefab"))up.asset = Resources.Load<GameObject>("Cache/_CVRAvatar").GetComponent<CVRAssetInfo>();
                if (File.Exists(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRSpawnable.prefab"))up.asset = Resources.Load<GameObject>("Cache/_CVRSpawnable").GetComponent<CVRAssetInfo>();
                if (File.Exists(Application.dataPath + "/ABI.CCK/Resources/Cache/_CVRWorld.prefab"))up.asset = Resources.Load<GameObject>("Cache/_CVRWorld").GetComponent<CVRAssetInfo>();
            }
        }
        
        void OnGUI()
        {
            var centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            
            GUILayout.Label(abiLogo, centeredStyle);
            EditorGUILayout.BeginVertical();
            
            _tab = GUILayout.Toolbar (_tab, new string[] {"Content Builder", "Settings & Options"});

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            
            switch (_tab)
            {
                case 0:
                    if (!_loggedIn)
                    {
                        Tab_LogIn();
                    }
                    else
                    {
                        Tab_LoggedIn();
                    }
                    break;
                
                case 1:
                    Tab_Settings();
                    break;
            }
            
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private void Tab_LogIn()
        {
            EditorGUILayout.LabelField("AlphaLink Access", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Please authenticate using your CCK credentials.");
            EditorGUILayout.LabelField("You can find those on hub.abinteractive.net.");
            EditorGUILayout.LabelField("Please generate a CCK Key in the key manager.");
            EditorGUILayout.Space();
            _username = EditorGUILayout.TextField("Username", _username);
            _key = EditorGUILayout.PasswordField("Key", _key);

            if (GUILayout.Button("Authenticate"))
            {
                Login();
            }

            if (_hasAttemptedToLogin && !_loggedIn)
            {
                GUILayout.Label("Incorrect User details provided");
            }
        }

        private void Tab_LoggedIn()
        {
            EditorGUILayout.LabelField("Account Information", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Authenticated as    ", _username);
            EditorGUILayout.LabelField("Api User Rank    ", _apiUserRank);
            EditorGUILayout.LabelField("Api Creator Rank    ", _apiCreatorRank);
            EditorGUILayout.LabelField("CCK version    ", CCKVersion);
            EditorGUILayout.Space();
            if (GUILayout.Button("Logout")){ 
                bool logout = EditorUtility.DisplayDialog("Remove local credentials for CCK",
                "This will remove the locally stored credentials. You will have to re-authenticate. Do you want to continue?",
                "Yes!", "No!");
                if (logout) Logout();
            }
            EditorGUILayout.HelpBox("Use our documentation to find out more about how to create content for our games. You will also find some handy tutorials on how to utilize most of the core engine features and core game features there.", MessageType.Info);
            if (GUILayout.Button("View our documentation")) Application.OpenURL("https://docs.abinteractive.net");
            EditorGUILayout.HelpBox("Want to request a feature? Found a bug? Post on our feedback platform!", MessageType.Info);
            if (GUILayout.Button("Post on our feedback platform")) Application.OpenURL("https://hub.abinteractive.net/feedback");
            EditorGUILayout.HelpBox("Please do not move the folder location of the CCK or CCK Mods folder. This will render the CCK unusable.", MessageType.Warning);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Found content", EditorStyles.boldLabel);
            List<CVRAvatar> avatars = new List<CVRAvatar>();
            List<CVRSpawnable> spawnables = new List<CVRSpawnable>();
            List<CVRWorld> worlds = new List<CVRWorld>();
            
            foreach (CVRWorld w in Resources.FindObjectsOfTypeAll<CVRWorld>())
            {
                if (w.gameObject.activeInHierarchy) worlds.Add(w);
            }
            
            foreach (CVRSpawnable s in Resources.FindObjectsOfTypeAll<CVRSpawnable>())
            {
                if (s.gameObject.activeInHierarchy) spawnables.Add(s);
            }

            foreach (CVRAvatar a in Resources.FindObjectsOfTypeAll<CVRAvatar>())
            {
                if (a.gameObject.activeInHierarchy) avatars.Add(a);
            }

            if (worlds.Count <= 0 && avatars.Count > 0 && (Application.unityVersion == supportedUnity || Application.unityVersion == supportedUnityLts))
            {
                if (avatars.Count <= 0) EditorGUILayout.LabelField("No configured avatars found in scene - CVRAvatar added?");
                else
                {
                    if (avatars.Count > 0)
                    {
                        var counter = 0;
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                        foreach (CVRAvatar a in avatars)
                        {
                            counter++;
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.Space();
                            GUILayout.Label("Avatar Object #" + counter);
                            OnGUIAvatar(a);
                        }

                        EditorGUILayout.EndScrollView();
                    }
                }
            }
            if (worlds.Count <= 0 && spawnables.Count > 0 && (Application.unityVersion == supportedUnity || Application.unityVersion == supportedUnityLts))
            {
                if (spawnables.Count <= 0) EditorGUILayout.LabelField("No configured avatars found in scene - CVRSpawnable added?");
                else
                {
                    if (spawnables.Count > 0)
                    {
                        var counter = 0;
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                        foreach (CVRSpawnable s in spawnables)
                        {
                            counter++;
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.Space();
                            GUILayout.Label("Spawnable Object #" + counter);
                            OnGUISpawnable(s);
                        }

                        EditorGUILayout.EndScrollView();
                    }
                }
            }
            if (avatars.Count <= 0 && worlds.Count == 1 && (Application.unityVersion == supportedUnity || Application.unityVersion == supportedUnityLts))
            {
                int errors = 0;
                int overallMissingScripts = 0;
                
                overallMissingScripts = CCK_Tools.CleanMissingScripts(CCK_Tools.SearchType.Scene , false, null);
                if (overallMissingScripts > 0) errors++;
                
                EditorGUILayout.HelpBox("A ChilloutVR World object has been found in the scene. Avatars can not be uploaded until the world object has been removed. Avatar objects will be part of the world and visible in-world unless they are disabled or removed.", MessageType.Info);

                //Error
                if (overallMissingScripts > 0) EditorGUILayout.HelpBox("Scene contains missing scripts. The upload will fail like this. Remove all missing script references before uploading or click Remove all missing scripts to automatically have this done for you.", MessageType.Error);
                
                //Warning
                if (worlds[0].spawns.Length == 0) EditorGUILayout.HelpBox("Your world does not have any spawn points assigned. Please add one or multiple spawn points in the CVRWorld component or the location of the CVRWorld holder object will be used. ", MessageType.Warning);
                
                //Info
                if (worlds[0].referenceCamera == null) EditorGUILayout.HelpBox("You do not have a reference camera assigned to your world. Default camera settings will be used. ", MessageType.Info);
                if (worlds[0].respawnHeightY <= -500) EditorGUILayout.HelpBox("The respawn height is under -500. It will take a long time to respawn when falling out of the map. This is probably not what you want. ", MessageType.Info);
                
                if (GUILayout.Button("Upload world") && errors <= 0)
                {
                    CCK_BuildUtility.BuildAndUploadMapAsset(EditorSceneManager.GetActiveScene(), worlds[0].gameObject);
                }
                if (overallMissingScripts > 0) if (GUILayout.Button("Remove all missing scripts")) CCK_Tools.CleanMissingScripts(CCK_Tools.SearchType.Scene , true, null);
            }
            if (avatars.Count <= 0 && worlds.Count > 1 && (Application.unityVersion == supportedUnity || Application.unityVersion == supportedUnityLts))
            {
                EditorGUILayout.HelpBox("Multiple CVR World objects are present in the scene. This will break the world. Please ensure that there is only one CVR World object in the scene or use our CVRWorld prefab.", MessageType.Error);
            }
            if (avatars.Count > 0 && worlds.Count > 0 && (Application.unityVersion == supportedUnity || Application.unityVersion == supportedUnityLts))
            {
                EditorGUILayout.HelpBox("Loaded scenes should never contain both avatar and world descriptor objects. Please setup your scenes accordingly.", MessageType.Error);
            }
            if (avatars.Count <= 0 && worlds.Count <= 0 && (Application.unityVersion == supportedUnity || Application.unityVersion == supportedUnityLts))
            {
                EditorGUILayout.HelpBox("No content found in present scene. Did you forget to add a descriptor component to a game object?", MessageType.Info);
            }
            if ((Application.unityVersion != supportedUnity && Application.unityVersion != supportedUnityLts))
            {
                EditorGUILayout.HelpBox("You are using a unity version that is not supported. Please use Unity 2019.3.1f1 (using Unity Hub makes version management easier).", MessageType.Error);
            }
        }

        void Tab_Settings()
        {
            EditorGUILayout.LabelField("Upload Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Switch Connection Encryption:");
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical();
            var ssl = EditorGUILayout.Popup(EditorPrefs.GetBool("m_ABI_SSL", true) ? 1 : 0, new []{"http", "https"}) == 1;
            EditorPrefs.SetBool("m_ABI_SSL", ssl);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox("If you have Problems uploading try switching to http.", MessageType.Info);
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Upload Region:");
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical();
            var region = EditorGUILayout.Popup(EditorPrefs.GetString("m_ABI_HOST", "EU") == "EU" ? 0 : 1, new []{"Europe", "USA"}) == 0 ? "EU" : "US";
            EditorPrefs.SetString("m_ABI_HOST", region);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.HelpBox("You can switch the Upload Region to increase your upload speed. Your content will still be available worldwide.", MessageType.Info);
            
            EditorGUILayout.Space();
        }
        
        void OnGUIAvatar(CVRAvatar avatar)
        {
            GameObject avatarObject = avatar.gameObject;
            GUI.enabled = true;
            EditorGUILayout.InspectorTitlebar(avatarObject.activeInHierarchy, avatarObject);
            int errors = 0;
            int overallPolygonsCount = 0;
            int overallSkinnedMeshRenderer = 0;
            int overallUniqueMaterials = 0;
            int overallMissingScripts = 0;
            foreach (MeshFilter filter in avatar.gameObject.GetComponentsInChildren<MeshFilter>())
            {
                if (filter.sharedMesh != null) overallPolygonsCount = overallPolygonsCount + filter.sharedMesh.triangles.Length / 3;
            }
            foreach (SkinnedMeshRenderer renderer in avatar.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                overallSkinnedMeshRenderer++;
                if (renderer.sharedMaterials != null) overallUniqueMaterials = overallUniqueMaterials + renderer.sharedMaterials.Length;
            }
            overallMissingScripts = CCK_Tools.CleanMissingScripts(CCK_Tools.SearchType.Selection ,false,avatarObject);
            if (overallMissingScripts > 0) errors++;

            //Errors
            if (overallMissingScripts > 0) EditorGUILayout.HelpBox("Avatar or its children contains missing scripts. The upload will fail like this. Remove all missing script references before uploading or click Remove all missing scripts to automatically have this done for you.", MessageType.Error);
            var animator = avatar.GetComponent<Animator>();
            if (animator == null)
            {
                errors++;
                EditorGUILayout.HelpBox("No Animator was detected for this Avatar. Make sure, that an Animator is present on the same GameObject as the CVRAvatar Component.", MessageType.Error);
            }
            if (animator != null && animator.avatar == null)
            {
                errors++;
                EditorGUILayout.HelpBox("The Avatar Slot in your Avatar is not filled. Please fill this field in with the correct avatar definition.", MessageType.Error);
            }
            
            //Warnings
            if (overallPolygonsCount > 100000) EditorGUILayout.HelpBox("Warning: This avatar has more than 100k (" + overallPolygonsCount + ") polygons in total. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Warning);
            if (overallSkinnedMeshRenderer > 10) EditorGUILayout.HelpBox("Warning: This avatar contains more than 10 (" + overallSkinnedMeshRenderer + ") SkinnedMeshRenderer components. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Warning);
            if (overallUniqueMaterials > 20) EditorGUILayout.HelpBox("Warning: This avatar utilizes more than 20 (" + overallUniqueMaterials + ") material slots. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Warning);
            if (avatar.viewPosition == Vector3.zero) EditorGUILayout.HelpBox("Warning: The view position of this avatar defaults to X=0,Y=0,Z=0. This means your view position is on the ground. This is probably not what you want.", MessageType.Warning);
            if (animator != null && animator.avatar != null && !animator.avatar.isHuman) EditorGUILayout.HelpBox("Warning: Your Avatar is not setup as Humanoid.", MessageType.Warning);
            
            var avatarMeshes = getAllAssetMeshesInAvatar(avatarObject);
            if (CheckForLegacyBlendShapeNormals(avatarMeshes))
            {
                EditorGUILayout.HelpBox("Warning: This Avatar has none legacy blend shape normals. This will lead to an increased filesize and lighting errors", MessageType.Warning);
                if(GUILayout.Button("Fix import settings"))
                {
                    FixLegacyBlendShapeNormals(avatarMeshes);
                }
            }
            
            //Info
            if (overallPolygonsCount >= 50000 && overallPolygonsCount <= 100000) EditorGUILayout.HelpBox("Info: This avatar has more than 50k (" + overallPolygonsCount + ") polygons in total. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Info);
            if (overallSkinnedMeshRenderer >= 5 && overallSkinnedMeshRenderer <= 10) EditorGUILayout.HelpBox("Info: This avatar contains more than 5 (" + overallSkinnedMeshRenderer + ") SkinnedMeshRenderer components. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Info);
            if (overallUniqueMaterials >= 10 && overallUniqueMaterials <= 20) EditorGUILayout.HelpBox("Info: This avatar utilizes more than 10 (" + overallUniqueMaterials + ") material slots. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Info);
            if (avatar.viewPosition.y <= 0.5f) EditorGUILayout.HelpBox("Info: The view position of this avatar is under 0.5 in height. This avatar is considered excessively small.", MessageType.Info);
            if (avatar.viewPosition.y > 3f) EditorGUILayout.HelpBox("Info: The view position of this avatar is under 0.5 in height. This avatar is considered excessively huge.", MessageType.Info);

            if (errors <= 0) if (GUILayout.Button("Upload Avatar")) CCK_BuildUtility.BuildAndUploadAvatar(avatarObject);
            if (overallMissingScripts > 0) if (GUILayout.Button("Remove all missing scripts")) CCK_Tools.CleanMissingScripts(CCK_Tools.SearchType.Selection ,true,avatarObject);

        }
        
        void OnGUISpawnable(CVRSpawnable s)
        {
            GameObject spawnableObject = s.gameObject;
            GUI.enabled = true;
            EditorGUILayout.InspectorTitlebar(spawnableObject.activeInHierarchy, spawnableObject);
            int errors = 0;
            int overallPolygonsCount = 0;
            int overallSkinnedMeshRenderer = 0;
            int overallUniqueMaterials = 0;
            int overallMissingScripts = 0;
            foreach (MeshFilter filter in s.gameObject.GetComponentsInChildren<MeshFilter>())
            {
                if (filter.sharedMesh != null) overallPolygonsCount = overallPolygonsCount + filter.sharedMesh.triangles.Length / 3;
            }
            foreach (SkinnedMeshRenderer renderer in s.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                overallSkinnedMeshRenderer++;
                if (renderer.sharedMaterials != null) overallUniqueMaterials = overallUniqueMaterials + renderer.sharedMaterials.Length;
            }
            overallMissingScripts = CCK_Tools.CleanMissingScripts(CCK_Tools.SearchType.Selection ,false, spawnableObject);
            if (overallMissingScripts > 0) errors++;

            //Errors
            if (overallMissingScripts > 0) EditorGUILayout.HelpBox("Spawnable Objects or its children contains missing scripts. The upload will fail like this. Remove all missing script references before uploading or click Remove all missing scripts to automatically have this done for you.", MessageType.Error);
            
            //Warnings
            if (overallPolygonsCount > 100000) EditorGUILayout.HelpBox("Warning: This spawnable object has more than 100k (" + overallPolygonsCount + ") polygons in total. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Warning);
            if (overallSkinnedMeshRenderer > 10) EditorGUILayout.HelpBox("Warning: This spawnable object contains more than 10 (" + overallSkinnedMeshRenderer + ") SkinnedMeshRenderer components. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Warning);
            if (overallUniqueMaterials > 20) EditorGUILayout.HelpBox("Warning: This spawnable object utilizes more than 20 (" + overallUniqueMaterials + ") material slots. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Warning);

            var avatarMeshes = getAllAssetMeshesInAvatar(spawnableObject);
            if (CheckForLegacyBlendShapeNormals(avatarMeshes))
            {
                EditorGUILayout.HelpBox("Warning: This spawnable object has none legacy blend shape normals. This will lead to an increased filesize and lighting errors", MessageType.Warning);
                if(GUILayout.Button("Fix import settings"))
                {
                    FixLegacyBlendShapeNormals(avatarMeshes);
                }
            }
            
            //Info
            if (overallPolygonsCount >= 50000 && overallPolygonsCount <= 100000) EditorGUILayout.HelpBox("Info: This spawnable object has more than 50k (" + overallPolygonsCount + ") polygons in total. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Info);
            if (overallSkinnedMeshRenderer >= 5 && overallSkinnedMeshRenderer <= 10) EditorGUILayout.HelpBox("Info: This spawnable object contains more than 5 (" + overallSkinnedMeshRenderer + ") SkinnedMeshRenderer components. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Info);
            if (overallUniqueMaterials >= 10 && overallUniqueMaterials <= 20) EditorGUILayout.HelpBox("Info: This spawnable object utilizes more than 10 (" + overallUniqueMaterials + ") material slots. This can cause performance problems in game. This does not prevent you from uploading. ", MessageType.Info);

            if (errors <= 0 && overallMissingScripts <= 0) if (GUILayout.Button("Upload Spawnable Object (Prop)")) CCK_BuildUtility.BuildAndUploadSpawnable(spawnableObject);
            if (overallMissingScripts > 0) if (GUILayout.Button("Remove all missing scripts")) CCK_Tools.CleanMissingScripts(CCK_Tools.SearchType.Selection ,true, spawnableObject);
        }

        private List<String> getAllAssetMeshesInAvatar(GameObject avatar)
        {
            var assetPathList = new List<String>();

            foreach (var sMeshRenderer in avatar.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if(sMeshRenderer == null)
                {
                    continue;
                }
                
                var currentMesh = sMeshRenderer.sharedMesh;
                
                if(currentMesh == null)
                {
                    Debug.LogWarning("MeshFilter with missing Mesh detected: " + sMeshRenderer.transform.name);
                    continue;
                }
                
                if(!AssetDatabase.Contains(currentMesh))
                {
                    continue;
                }

                string meshAssetPath = AssetDatabase.GetAssetPath(currentMesh);
                if(string.IsNullOrEmpty(meshAssetPath))
                {
                    continue;
                }
                
                if (assetPathList.Contains(meshAssetPath))
                {
                    continue;
                }
                
                assetPathList.Add(meshAssetPath);
            }
            
            foreach (var meshFilter in avatar.GetComponentsInChildren<MeshFilter>(true))
            {
                if(meshFilter == null)
                {
                    continue;
                }
                
                var currentMesh = meshFilter.sharedMesh;
                
                if(currentMesh == null)
                {
                    Debug.LogWarning("MeshFilter with missing Mesh detected: " + meshFilter.transform.name);
                    continue;
                }
                
                if(!AssetDatabase.Contains(currentMesh))
                {
                    continue;
                }

                string meshAssetPath = AssetDatabase.GetAssetPath(currentMesh);
                if(string.IsNullOrEmpty(meshAssetPath))
                {
                    continue;
                }
                
                if (assetPathList.Contains(meshAssetPath))
                {
                    continue;
                }
                
                assetPathList.Add(meshAssetPath);
            }

            foreach (var pRenderer in avatar.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                if(pRenderer == null)
                {
                    continue;
                }

                var particleMeshes = new Mesh[pRenderer.meshCount];
                pRenderer.GetMeshes(particleMeshes);

                foreach (var particleMesh in particleMeshes)
                {
                    if(particleMesh == null)
                    {
                        Debug.LogWarning("MeshFilter with missing Mesh detected: " + pRenderer.transform.name);
                        continue;
                    }
                    
                    if(!AssetDatabase.Contains(particleMesh))
                    {
                        continue;
                    }

                    string meshAssetPath = AssetDatabase.GetAssetPath(particleMesh);
                    if(string.IsNullOrEmpty(meshAssetPath))
                    {
                        continue;
                    }
                
                    if (assetPathList.Contains(meshAssetPath))
                    {
                        continue;
                    }
                
                    assetPathList.Add(meshAssetPath);
                }
            }
            
            return assetPathList;
        }
        
        private bool CheckForLegacyBlendShapeNormals(List<String> assetPaths)
        {
            foreach (var assetPath in assetPaths)
            {

                var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if(modelImporter == null)
                {
                    continue;
                }

                if(modelImporter.importBlendShapeNormals != ModelImporterNormals.Calculate)
                {
                    continue;
                }
                
                if((bool)legacyBlendShapeImporter.GetValue(modelImporter))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void FixLegacyBlendShapeNormals(List<String> assetPaths)
        {
            foreach (var assetPath in assetPaths)
            {
                var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if(modelImporter == null)
                {
                    continue;
                }

                if(modelImporter.importBlendShapeNormals != ModelImporterNormals.Calculate)
                {
                    continue;
                }

                legacyBlendShapeImporter.SetValue(modelImporter, true);
                modelImporter.SaveAndReimport();
            }
        }
        
        private void EditorUpdate()
        {
            if (!_attemptingToLogin || !_webRequest.isDone) return;

            if (_webRequest.isNetworkError || _webRequest.isHttpError)
            {
                Debug.LogError("[ABI:CCK] Web Request Error while trying to authenticate.");
                return;
            }

            var result = _webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(result)) return;

            var apiValidateProfile = XDocument.Parse(result);
            var responseCode = apiValidateProfile.XPathSelectElement("IContentCreation/ValidateKey/Status");
            var message = apiValidateProfile.XPathSelectElement("IContentCreation/ValidateKey/Message");

            var isValidProfile = (int)responseCode;
            var apiMessage = (string)message;

            if (isValidProfile == 1)
            {
                _apiCreatorRank = (string) apiValidateProfile.XPathSelectElement("IContentCreation/ValidateKey/Meta/CreatorRank");
                _apiUserRank = (string) apiValidateProfile.XPathSelectElement("IContentCreation/ValidateKey/Meta/UserRank");
                Debug.Log("[ABI:CCK] Successfully authenticated as " + _username + " using AlphaLink Public API.");
                EditorPrefs.SetString("m_ABI_Username", _username);
                EditorPrefs.SetString("m_ABI_Key", _key);
                _loggedIn = true;
                _hasAttemptedToLogin = false;
            }
            else
            {
                Debug.Log("[ABI:CCK] Unable to authenticate using provided credentials. API responded with: " + apiMessage + ".");
                _loggedIn = false;
                _hasAttemptedToLogin = true;
                _username = _key = string.Empty;
            }

            _webRequest = null;
            _attemptingToLogin = false;
        }
        
        private void Logout()
        {
            _loggedIn = false;
            _username = _key = string.Empty;
            EditorPrefs.SetString("m_ABI_Username", _username);
            EditorPrefs.SetString("m_ABI_Key", _key);
        }

        public void Login()
        {
            if (_attemptingToLogin || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_key)) return;
            var values = new Dictionary<string, string> {{"user", _username}, {"accesskey", _key}};
            _webRequest = UnityWebRequest.Post("https://api.alphablend.cloud/IContentCreation/ValidateKey", values);
            _webRequest.SendWebRequest();
            _attemptingToLogin = true;
        }

    }
}