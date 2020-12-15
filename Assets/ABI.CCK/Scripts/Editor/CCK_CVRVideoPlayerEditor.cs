using System;
using ABI.CCK.Components;
using UnityEditor;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(ABI.CCK.Components.CVRVideoPlayer))]
    public class CCK_CVRVideoPlayerEditor : UnityEditor.Editor
    {

        private CVRVideoPlayer _player;
        
        public override void OnInspectorGUI()
        {
            if (_player == null) _player = (CVRVideoPlayer) target;
        }
        
        
    }
}