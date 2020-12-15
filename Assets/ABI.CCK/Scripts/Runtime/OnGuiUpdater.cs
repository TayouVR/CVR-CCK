using System;
using System.IO;
using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ABI.CCK.Scripts.Runtime
{
    public class OnGuiUpdater : MonoBehaviour
    {
        [Space] [Header("Object details")] [Space]
        public Text uiTitle;
        public InputField assetName;
        public InputField assetDesc;
        
        [Space] [Header("Object tags")] [Space]
        public Toggle sfw;
        public Toggle nsfw;
        public Toggle nsfwSubSuggestive;
        public Toggle nsfwSubNudity;
        //Avatar
        public Toggle avtrLoudAudio;
        public Toggle avtrLongRangeAudio;
        public Toggle avtrSpawnAudio;
        public Toggle avtrScreenFx;
        public Toggle avtrFlashingColors;
        public Toggle avtrFlashingLights;
        public Toggle avtrParticleSystems;
        public Toggle avtrViolence;
        public Toggle avtrGore;
        public Toggle avtrExcessivelyHuge;
        public Toggle avtrExcessivelySmall;
        //World
        public Toggle wrldScreenFx;
        public Toggle wrldFlashingColors;
        public Toggle wrldFlashingLights;
        public Toggle wrldParticleSystems;
        public Toggle wrldViolence;
        public Toggle wrldGore;
        public Toggle wrldWeaponSystem;
        public Toggle wrldMinigame;

        public Toggle dontOverridePicture;
        
        //Regulatory
        public Toggle contentOwnership;
        public Toggle tagsCorrect;
        
        [Space] [Header("Object reference")] [Space]
        public CVRAssetInfo asset;

        [Space] [Header("UIHelper objects")] [Space]
        public GameObject wrldSafety;
        public GameObject avtrSafety;
        public GameObject camObj;
        public RenderTexture tex;
        public RawImage texView;
        public GameObject tagsObject;
        public GameObject detailsObject;
        public GameObject uploadObject;
        public Image stepOne;
        public Image stepTwo;
        public Image tagsImage;
        public Image detailsImage;
        public Image uploadImage;
        public Text tagsText;
        public Text detailsText;
        public Text uploadText;
        public Image uploadProgress;
        public Text uploadProgressText;
        public Text fileSizeText;

        public void ToggleObject(GameObject obj)
        {
            obj.SetActive(!obj.activeInHierarchy);
        }
        
        public static string GetHumanReadableFileSize(long fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1) {
                order++;
                fileSize = fileSize/1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", fileSize, sizes[order]);
        }
        
        void Start()
        {
            SwitchPage(0);
            CVRAssetInfo.AssetType type = asset.GetComponent<CVRAssetInfo>().type;
            
            if (type == CVRAssetInfo.AssetType.Avatar) avtrSafety.SetActive(true);
            if (type == CVRAssetInfo.AssetType.World)
            {
                wrldSafety.SetActive(true);
                Scripts.Editor.CCK_WorldPreviewCapture.CreatePanoImages();
            }

            tex = new RenderTexture(512,512,1, RenderTextureFormat.ARGB32);
            tex.Create();
            
            camObj = new GameObject();
            camObj.name = "ShotCam for CVR CCK";
            camObj.transform.rotation = new Quaternion(0,180,0,0);
            CVRAvatar avatar = asset.GetComponent<CVRAvatar>();
            if (asset.type == CVRAssetInfo.AssetType.Avatar) camObj.transform.position = new Vector3(avatar.viewPosition.x, avatar.viewPosition.y, avatar.viewPosition.z *= 5f);
            var cam = camObj.AddComponent<Camera>();
            cam.aspect = 1f;
            cam.nearClipPlane = 0.01f;
            cam.targetTexture = tex;
            texView.texture = tex;
            
            #if UNITY_EDITOR

#endif

            string handle;
            if (type == CVRAssetInfo.AssetType.Avatar) fileSizeText.text = "Asset Bundle File: " + GetHumanReadableFileSize(new FileInfo(Application.persistentDataPath + "/bundle.cvravatar").Length)
                                                                                                 + "\nAsset Bundle Manifest File: " + GetHumanReadableFileSize(new FileInfo(Application.persistentDataPath + "/bundle.cvravatar.manifest").Length)
                                                                                                 + "\nThumbnail File: creation pending";
            
            if (type == CVRAssetInfo.AssetType.World) fileSizeText.text = "Asset Bundle File: " + GetHumanReadableFileSize(new FileInfo(Application.persistentDataPath + "/bundle.cvrworld").Length) 
                                                                                                + "\nAsset Bundle Manifest File: " + GetHumanReadableFileSize(new FileInfo(Application.persistentDataPath + "/bundle.cvrworld.manifest").Length)
                                                                                                + "\n1K Pano File: " + GetHumanReadableFileSize(new FileInfo(Application.persistentDataPath + "/bundle_pano_1024.png").Length)
                                                                                                + "\n4K Pano File: " + GetHumanReadableFileSize(new FileInfo(Application.persistentDataPath + "/bundle_pano_4096.png").Length)
                                                                                                + "\nThumbnail File: creation pending";
            
        }

        public void SwitchPage(int index)
        {
            switch (index)
            {
                case 0:
                    tagsObject.SetActive(true);
                    detailsObject.SetActive(false);
                    uploadObject.SetActive(false);
                    stepOne.color = Color.white;
                    stepTwo.color = Color.white;
                    tagsImage.color = Color.yellow;
                    detailsImage.color = Color.white;
                    uploadImage.color = Color.white;
                    tagsText.color = Color.yellow;
                    detailsText.color = Color.white;
                    uploadText.color = Color.white;
                    break;
                case 1:
                    tagsObject.SetActive(false);
                    detailsObject.SetActive(true);
                    uploadObject.SetActive(false);
                    stepOne.color = Color.green;
                    stepTwo.color = Color.white;
                    tagsImage.color = Color.green;
                    detailsImage.color = Color.yellow;
                    uploadImage.color = Color.white;
                    tagsText.color = Color.green;
                    detailsText.color = Color.yellow;
                    uploadText.color = Color.white;
                    break;
                case 2:
                    tagsObject.SetActive(false);
                    detailsObject.SetActive(false);
                    uploadObject.SetActive(true);
                    stepOne.color = Color.green;
                    stepTwo.color = Color.green;
                    tagsImage.color = Color.green;
                    detailsImage.color = Color.green;
                    uploadImage.color = Color.yellow;
                    tagsText.color = Color.green;
                    detailsText.color = Color.green;
                    uploadText.color = Color.yellow;
                    if (string.IsNullOrEmpty(assetName.text) || string.IsNullOrEmpty(assetDesc.text) || !contentOwnership.isOn || !tagsCorrect.isOn) SwitchPage(1);
                    break;
            }
        }
        
        

    }
}
