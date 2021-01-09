using System;
using System.Collections.Generic;
using ABI.CCK.Components;
using ABI.CCK.Scripts.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ABI.CCK.Scripts.Editor
{
    public class CCK_BuildUtility
    {
        public static void BuildAndUploadAvatar(GameObject avatarObject)
        {
            GameObject avatarCopy = null;
            var origInfo = avatarObject.GetComponent<CVRAssetInfo>();
            
            try
            {
                avatarCopy = GameObject.Instantiate(avatarObject);
                PrefabUtility.UnpackPrefabInstance(avatarCopy, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                Debug.Log("[CCK:BuildUtility] To prevent problems, the prefab has been unpacked. Your game object is no longer linked to the prefab instance.");
            }
            catch
            {
                Debug.Log("[CCK:BuildUtility] Object is not a prefab. No need to unpack.");
            }

            CVRAssetInfo info = avatarCopy.GetComponent<CVRAssetInfo>();
            if (string.IsNullOrEmpty(info.guid))
            {
                info.guid = Guid.NewGuid().ToString();
                origInfo.guid = info.guid;
                try
                {
                    PrefabUtility.ApplyPrefabInstance(avatarObject, InteractionMode.UserAction);
                }
                catch
                {
                    Debug.Log("[CCK:BuildUtility] Object is not a prefab. No need to Apply To Instance.");
                }
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            PrefabUtility.SaveAsPrefabAsset(avatarCopy, "Assets/ABI.CCK/Resources/Cache/_CVRAvatar.prefab");
            GameObject.DestroyImmediate(avatarCopy);
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetNames = new[] {"Assets/ABI.CCK/Resources/Cache/_CVRAvatar.prefab"};
            assetBundleBuild.assetBundleName = "bundle.cvravatar";

            BuildPipeline.BuildAssetBundles(Application.persistentDataPath, new[] {assetBundleBuild},
                BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            
            AssetDatabase.Refresh();
            
            EditorPrefs.SetBool("m_ABI_isBuilding", true);
            EditorApplication.isPlaying = true;
        }
        
        public static void BuildAndUploadSpawnable(GameObject s)
        {
            GameObject sCopy = null;
            var origInfo = s.GetComponent<CVRAssetInfo>();
            
            try
            {
                sCopy = GameObject.Instantiate(s);
                PrefabUtility.UnpackPrefabInstance(sCopy, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                Debug.Log("[CCK:BuildUtility] To prevent problems, the prefab has been unpacked. Your game object is no longer linked to the prefab instance.");
            }
            catch
            {
                Debug.Log("[CCK:BuildUtility] Object is not a prefab. No need to unpack.");
            }

            CVRAssetInfo info = sCopy.GetComponent<CVRAssetInfo>();
            if (string.IsNullOrEmpty(info.guid))
            {
                info.guid = Guid.NewGuid().ToString();
                origInfo.guid = info.guid;
                try
                {
                    PrefabUtility.ApplyPrefabInstance(s, InteractionMode.UserAction);
                }
                catch
                {
                    Debug.Log("[CCK:BuildUtility] Object is not a prefab. No need to Apply To Instance.");
                }
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            PrefabUtility.SaveAsPrefabAsset(sCopy, "Assets/ABI.CCK/Resources/Cache/_CVRSpawnable.prefab");
            GameObject.DestroyImmediate(sCopy);
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetNames = new[] {"Assets/ABI.CCK/Resources/Cache/_CVRSpawnable.prefab"};
            assetBundleBuild.assetBundleName = "bundle.cvrprop";

            BuildPipeline.BuildAssetBundles(Application.persistentDataPath, new[] {assetBundleBuild},
                BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            
            AssetDatabase.Refresh();
            
            EditorPrefs.SetBool("m_ABI_isBuilding", true);
            EditorApplication.isPlaying = true;
        }

        public static void BuildAndUploadMapAsset(Scene scene, GameObject descriptor)
        {
            SetupNetworkUUIDs();

            EditorSceneManager.MarkSceneDirty(scene);
            
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            CVRAssetInfo info = descriptor.GetComponent<CVRAssetInfo>();
            if (string.IsNullOrEmpty(info.guid)) info.guid = Guid.NewGuid().ToString();
            
            PrefabUtility.SaveAsPrefabAsset(descriptor, "Assets/ABI.CCK/Resources/Cache/_CVRWorld.prefab");
            
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetNames = new[] {scene.path};
            assetBundleBuild.assetBundleName = "bundle.cvrworld";
            
            BuildPipeline.BuildAssetBundles(Application.persistentDataPath, new[] {assetBundleBuild},
                BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();
            
            EditorPrefs.SetBool("m_ABI_isBuilding", true);
            EditorApplication.isPlaying = true;
        }

        public static void SetupNetworkUUIDs()
        {
            CVRInteractable[] interactables = Resources.FindObjectsOfTypeAll<CVRInteractable>();
            CVRObjectSync[] objectSyncs = Resources.FindObjectsOfTypeAll<CVRObjectSync>();
            
            List<string> UsedGuids = new List<string>();

            foreach (var interactable in interactables)
            {
                foreach (var action in interactable.actions)
                {
                    string guid;
                    do
                    {
                        guid = Guid.NewGuid().ToString();
                    } while (!string.IsNullOrEmpty(UsedGuids.Find(match => match == guid)));

                    action.guid = guid;
                }
            }
            
            foreach (var objectSync in objectSyncs)
            {
                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString();
                } while (!string.IsNullOrEmpty(UsedGuids.Find(match => match == guid)));

                objectSync.guid = guid;
            }
        }
    }
}
