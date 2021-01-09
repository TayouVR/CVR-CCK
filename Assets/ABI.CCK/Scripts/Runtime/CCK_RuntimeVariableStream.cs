using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace ABI.CCK.Scripts.Runtime
{
    public class CCK_RuntimeVariableStream : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(StreamVars());
        }

        private IEnumerator StreamVars()
        {
            
            OnGuiUpdater updater = gameObject.GetComponent<OnGuiUpdater>();
            string type = updater.asset.type.ToString();
            var values = new Dictionary<string, string>
            {
                #if UNITY_EDITOR
                {"user", EditorPrefs.GetString("m_ABI_Username")},
                {"accesskey", EditorPrefs.GetString("m_ABI_Key")},
                #endif
                {"type", type},
                {"guid", updater.asset.guid},
            };
            using (UnityWebRequest www = UnityWebRequest.Post("https://api.alphablend.cloud/IContentCreation/GetAssetInfoValues", values))
            {
                
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log("[CCK:RuntimeVariableStream] Unable to open api connection. Can not stream old asset info values.");
                    yield break;
                }

                string sd = www.downloadHandler.text;

                try
                {
                    XDocument doc = XDocument.Parse(sd);
                    
                    //General asset info
                    updater.assetName.text = (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AssetName");
                    updater.assetDesc.text = (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AssetDesc");
                    //SFW Level
                    if ((string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SfwLevel") == "nsfw")
                    {
                        updater.sfw.isOn = false;
                        updater.nsfw.isOn = true;
                    }
                    else
                    {
                        updater.sfw.isOn = true;
                        updater.nsfw.isOn = false;
                    }
                    
                    //Additional NSFW
                    if ((string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SfwSuggestive") == "YES") updater.nsfwSubSuggestive.isOn = true;
                    if ((string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SfwNudity") == "YES") updater.nsfwSubNudity.isOn = true;
                    
                    //Avatar
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarLoudAudio") == "YES") updater.avtrLoudAudio.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarLongRangeAudio") == "YES") updater.avtrLongRangeAudio.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarScreenEffects") == "YES") updater.avtrScreenFx.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarFlashingColors") == "YES") updater.avtrFlashingColors.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarFlashingLights") == "YES") updater.avtrFlashingLights.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarParticleSystems") == "YES") updater.avtrParticleSystems.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarViolence") == "YES") updater.avtrViolence.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarGore") == "YES") updater.avtrGore.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarExcessivelyHuge") == "YES") updater.avtrExcessivelyHuge.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarExcessivelySmall") == "YES") updater.avtrExcessivelySmall.isOn = true;
                    if (type == "Avatar" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/AvatarSpawnAudio") == "YES") updater.avtrSpawnAudio.isOn = true;
                    //World
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldScreenEffects") == "YES") updater.wrldScreenFx.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldFlashingColors") == "YES") updater.wrldFlashingColors.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldFlashingLights") == "YES") updater.wrldFlashingLights.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldParticleSystems") == "YES") updater.wrldParticleSystems.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldViolence") == "YES") updater.wrldViolence.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldGore") == "YES") updater.wrldGore.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldWeaponSystem") == "YES") updater.wrldWeaponSystem.isOn = true;
                    if (type == "World" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/WorldMinigames") == "YES") updater.wrldMinigame.isOn = true;
                    //Spawnable
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableLoudAudio") == "YES") updater.propLoudAudio.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableLongRangeAudio") == "YES") updater.propLongRangeAudio.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableScreenEffects") == "YES") updater.propScreenFx.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableFlashingColors") == "YES") updater.propFlashingColors.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableFlashingLights") == "YES") updater.propFlashingLights.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableParticleSystems") == "YES") updater.propParticleSystems.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableViolence") == "YES") updater.propViolence.isOn = true;
                    if (type == "Spawnable" && (string) doc.XPathSelectElement("IContentCreation/GetAssetInfoValues/Meta/SpawnableGore") == "YES") updater.propGore.isOn = true;
                }
                catch
                {
                    Debug.Log("[CCK:RuntimeVariableStream] Unable to stream variable entries. Received XML is invalid. Is Alpha Blend Interactive down?");
                }

            }
        }
        
    }
}