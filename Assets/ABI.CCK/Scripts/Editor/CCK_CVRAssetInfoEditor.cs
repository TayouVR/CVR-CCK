using System;
using ABI.CCK.Components;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    
    [CustomEditor(typeof(ABI.CCK.Components.CVRAssetInfo))]
    public class CCK_CVRAssetInfoEditor : UnityEditor.Editor
    {
        private CVRAssetInfo _info;
        private string _newGuid;

        public override void OnInspectorGUI()
        {
            if (_info == null) _info = (CVRAssetInfo)target;

            EditorGUILayout.HelpBox("This script is used to store object metadata. Please do not modify the data on it unless you know what you are doing. To reupload an avatar, detach the Guid and reupload.", MessageType.Info);

            if (!string.IsNullOrEmpty(_info.guid))
            {
                EditorGUILayout.HelpBox("The currently stored Guid is: " + _info.guid, MessageType.Info);
                if (GUILayout.Button("Detach asset unique identifier"))
                {
                    bool detach = EditorUtility.DisplayDialog("Detach Guid from Asset Info Manager",
                        "The asset unique identifier will be detached. This means that your content will most likely be uploaded as new on runtime. Continue?",
                        "Yes!", "No!");
                    if (detach) DetachGuid();
                }
            }
            else
            {
                _newGuid = EditorGUILayout.TextField("Unique identifier", _newGuid);
                EditorGUILayout.HelpBox("You do not need to re-attach a Guid if you do not plan to overwrite any old upload. A new one will be generated on upload if none is attached.", MessageType.Warning);
                if (GUILayout.Button("Re-Attach guid")) ReattachGuid(_newGuid);
            }
            
        }

        private void DetachGuid()
        {
            if (!string.IsNullOrEmpty(_info.guid)) _info.guid = string.Empty;
        }
        private void ReattachGuid(string Guid)
        {
            _info.guid = Guid;
        }

    }
}
