using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using ABI.CCK.Components;
using ABI.CCK.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace ABI.CCK.Scripts.Runtime
{
    public class CCK_RuntimeUploaderMaster : MonoBehaviour
    {
        public OnGuiUpdater updater;
        private UnityWebRequest req;

        public bool isUploading;
        public float progress = 0f;

        public string encryption;


        public void StartUpload()
        {
            //Build string
            var type = updater.asset.type;
            var sfwLevel = string.Empty;
            var overwritePic = string.Empty;
            
            if (updater.dontOverridePicture.isOn) overwritePic = "yes";
            else overwritePic = "no";

            #if UNITY_EDITOR
            if (EditorPrefs.GetBool("m_ABI_SSL", true) == false) encryption = "http";
            else encryption = "https";
            #endif

            if (updater.sfw.isOn) sfwLevel = "true";
            else sfwLevel = "false";
            
            if (updater.contentOwnership.isOn && updater.tagsCorrect.isOn && !string.IsNullOrEmpty(updater.assetName.text) && !string.IsNullOrEmpty(updater.assetDesc.text))
            {
                Debug.Log("[CCK:RuntimeUploader] Uploader parameters OK. Proceeding with upload.");
                gameObject.GetComponent<CCK_TexImageCreation>().SaveTexture(updater.camObj.GetComponent<Camera>(), updater.tex);
                StartCoroutine(UploadAssetAndSendInformation(updater.asset.GetComponent<CVRAssetInfo>().guid, type.ToString(), sfwLevel, updater.assetName.text, updater.assetDesc.text, overwritePic));
            }
            else
            {
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK", "Please make sure, that all necessary fields are filled before uploading. Necessary fields are: Name, Description, Regulatory boxes", "Okay");
                #endif
                Debug.LogError("[CCK:RuntimeUploader] Some necessary fields are not filled. Please fill all fields and accept the regulatory notes before uploading content to our cloud.");
            }

        }

        private IEnumerator UploadAssetAndSendInformation(string guid, string type, string sfwLevel, string assetName, string assetDesc, string overwritePic)
        {
            string[] path = null;
            if (type == "Avatar")
            {
                path = new string[3];
                path[0] = Application.persistentDataPath + "/bundle.cvravatar";
                path[1] = Application.persistentDataPath + "/bundle.cvravatar.manifest";
                path[2] = Application.persistentDataPath + "/bundle.png";
            }
            if (type == "World")
            {
                path = new string[5];
                path[0] = Application.persistentDataPath + "/bundle.cvrworld";
                path[1] = Application.persistentDataPath + "/bundle.cvrworld.manifest";
                path[2] = Application.persistentDataPath + "/bundle.png";
                path[3] = Application.persistentDataPath + "/bundle_pano_1024.png";
                path[4] = Application.persistentDataPath + "/bundle_pano_4096.png";
            }

            UnityWebRequest[] files = new UnityWebRequest[path.Length];
            WWWForm form = new WWWForm();
            #if UNITY_EDITOR
            form.AddField("user", EditorPrefs.GetString("m_ABI_Username"));
            form.AddField("accesskey", EditorPrefs.GetString("m_ABI_Key"));
            #endif
            form.AddField("guid", guid);
            form.AddField("assetname", assetName);
            form.AddField("assetdesc", assetDesc);
            form.AddField("sfwlevel", sfwLevel);
            form.AddField("type", type);
            form.AddField("cckbuild", "59");
            form.AddField("updatepic", overwritePic);
            form.AddField("flagSuggestive", updater.nsfwSubSuggestive.isOn.ToString());
            form.AddField("flagNudity", updater.nsfwSubNudity.isOn.ToString());
            form.AddField("avtrLoudAudio", updater.avtrLoudAudio.isOn.ToString());
            form.AddField("avtrLongRangeAudio", updater.avtrLongRangeAudio.isOn.ToString());
            form.AddField("avtrSpawnAudio", updater.avtrSpawnAudio.isOn.ToString());
            form.AddField("avtrScreenFx", updater.avtrScreenFx.isOn.ToString());
            form.AddField("avtrFlashingColors", updater.avtrFlashingColors.isOn.ToString());
            form.AddField("avtrFlashingLights", updater.avtrFlashingLights.isOn.ToString());
            form.AddField("avtrParticleSystems", updater.avtrParticleSystems.isOn.ToString());
            form.AddField("avtrViolence", updater.avtrViolence.isOn.ToString());
            form.AddField("avtrGore", updater.avtrGore.isOn.ToString());
            form.AddField("avtrExcessivelyHuge", updater.avtrExcessivelyHuge.isOn.ToString());
            form.AddField("avtrExcessivelySmall", updater.avtrExcessivelySmall.isOn.ToString());
            form.AddField("wrldScreenFx", updater.wrldScreenFx.isOn.ToString());
            form.AddField("wrldFlashingColors", updater.wrldFlashingColors.isOn.ToString());
            form.AddField("wrldFlashingLights", updater.wrldFlashingLights.isOn.ToString());
            form.AddField("wrldParticleSystems", updater.wrldParticleSystems.isOn.ToString());
            form.AddField("wrldViolence", updater.wrldViolence.isOn.ToString());
            form.AddField("wrldGore", updater.wrldGore.isOn.ToString());
            form.AddField("wrldWeaponSystem", updater.wrldWeaponSystem.isOn.ToString());
            form.AddField("wrldMinigame", updater.wrldMinigame.isOn.ToString());

            for (int i = 0; i < files.Length; i++)
            {
                string fieldName = string.Empty;
                switch (i)
                {
                    case 0: fieldName = "asset"; break;
                    case 1: fieldName = "assetmanifest"; break;
                    case 2: fieldName = "screenshot"; break;
                    case 3: fieldName = "pano1024"; break;
                    case 4: fieldName = "pano4096"; break;
                }
                files[i] = UnityWebRequest.Get(path[i]);
                yield return files[i].SendWebRequest();
                form.AddBinaryData(fieldName, files[i].downloadHandler.data, Path.GetFileName(path[i]));
            }
            #if UNITY_EDITOR
            req = UnityWebRequest.Post($"{encryption}://push.{EditorPrefs.GetString("m_ABI_HOST", "EU")}.abidata.io", form);
            #endif
            isUploading = true;
            yield return req.SendWebRequest();
            
            if (req.isHttpError || req.isNetworkError)
                Debug.LogError(req.error);
            else
            {
                var response = req.downloadHandler.text;
                try
                {
                    var doc = XDocument.Parse(response);

                    var code = doc.XPathSelectElement("IContentCreation/UploadAsset/Status");

                    if ((string) code == "OK")
                    {
                        Debug.Log("[CCK:RuntimeUploader] Api responded with:" + (string) doc.XPathSelectElement("IContentCreation/UploadAsset/Message"));
                        UploadCompleteDialog();
                    }

                    if ((string) code == "LIMIT_REACHED")
                    {
                        Debug.Log("[CCK:RuntimeUploader] Api responded with:" + (string) doc.XPathSelectElement("IContentCreation/UploadAsset/Message"));
                        AccountLimitReached();
                    }
                    
                    if ((string) code == "SCHEDULED_SECURITY_CHECK")
                    {
                        Debug.Log("[CCK:RuntimeUploader] Api responded with:" + (string) doc.XPathSelectElement("IContentCreation/UploadAsset/Message"));
                        SecurityCheckRequested();
                    }

                    if ((string) code != "OK" && (string) code != "LIMIT_REACHED" && (string) code != "SCHEDULED_SECURITY_CHECK") UploadFailedDialog((string) doc.XPathSelectElement("IContentCreation/UploadAsset/Message"));
                }
                catch (Exception e)
                {
                    UploadFailedApiError(e.Message);
                    Debug.Log(e);
                    Debug.Log(response);
                }
            }
        }

        void Update()
        {
            if (isUploading)
            {
                #if UNITY_EDITOR
                //EditorUtility.DisplayCancelableProgressBar("Alpha Blend Interactive CCK", "Now uploading your content to the Alpha Blend Interactive cloud.", req.uploadProgress);
                updater.uploadProgress.fillAmount = req.uploadProgress;
                updater.uploadProgressText.text = (req.uploadProgress * 100f) + "%";
                #endif
            }
        }
        
        private void UploadCompleteDialog()
        {
            isUploading = false;
            #if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
            if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK","Upload finished. You can manage your uploaded content using our website or launch ChilloutVR to see it in-game.","Okay"))
            {
                EditorApplication.isPlaying = false;
            }
            #endif
        }
        private void SecurityCheckRequested()
        {
            isUploading = false;
            #if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
            if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK","Upload finished. Your upload is now being scanned by our security system and will be available in-game shortly.","Okay"))
            {
                EditorApplication.isPlaying = false;
            }
            #endif
        }
        private void UploadFailedDialog(string apiMessage)
        {
            isUploading = false;
            #if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
            if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK","Upload failed. Response from API: " + apiMessage,"Okay"))
            {
                EditorApplication.isPlaying = false;
            }
            #endif
        }
        private void UploadFailedApiError(string e)
        {
            isUploading = false;
            #if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
            if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK","Upload failed. Error: " + e,"Okay"))
            {
                EditorApplication.isPlaying = false;
            }
            #endif
        }
        private void AccountLimitReached()
        {
            isUploading = false;
            #if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
            if (UnityEditor.EditorUtility.DisplayDialog("Alpha Blend Interactive CCK","Upload rejected. Your account has reached its free limits. You can not upload any more content, unless you delete some of your uploaded content. You can also consider buying our one-time account unlock to have this limit removed.","Okay"))
            {
                EditorApplication.isPlaying = false;
            }
            #endif
        }

    }
}
