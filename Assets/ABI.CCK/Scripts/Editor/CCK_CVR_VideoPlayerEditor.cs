using System;
using ABI.CCK.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


    [CustomEditor(typeof(ABI.CCK.Components.CVRVideoPlayer))]
    public class CCK_CVR_VideoPlayerEditor : UnityEditor.Editor
    {
        
        private ReorderableList reorderableList;
        private CVRVideoPlayer _player;
        private CVRVideoPlayerPlaylistEntity entity = null;
        
        private const string TypeLabel = "Playlist - Videos";

        private void OnEnable()
        {
            if (_player == null) _player = (CVRVideoPlayer) target;
        
            reorderableList = new ReorderableList(_player.entities, typeof(CVRObjectSyncTask), true, true, true, true);
            reorderableList.drawHeaderCallback = OnDrawHeader;
            reorderableList.drawElementCallback = OnDrawElement;
            reorderableList.elementHeightCallback = OnHeightElement;
            reorderableList.onAddCallback = OnAdd;
            reorderableList.onChangedCallback = OnChanged;
        }

        private float OnHeightElement(int index)
        {
            return EditorGUIUtility.singleLineHeight * 7f;
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            GUI.Label(_rect, TypeLabel);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            entity = _player.entities[index];
       
            rect.y += 2;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Title");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.videoTitle = EditorGUI.TextField(_rect, entity.videoTitle);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Url");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.videoUrl = EditorGUI.TextField(_rect, entity.videoUrl);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Start");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.introEndInSeconds = Convert.ToInt32(EditorGUI.TextField(_rect, entity.introEndInSeconds.ToString()));
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "End");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.creditsStartInSeconds = Convert.ToInt32(EditorGUI.TextField(_rect, entity.creditsStartInSeconds.ToString()));
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Thumbnail");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.thumbnail = (Texture2D) EditorGUI.ObjectField(_rect, entity.thumbnail, typeof(Texture2D));

        }
        
        private void OnAdd(ReorderableList list)
        {
            _player.entities.Add(null);
        }
   
        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Please note that experimental settings are not properly implemented yet and might break at anytime.", MessageType.Warning);
            EditorGUILayout.Space();
            
            _player.Stereo360Experimental = EditorGUILayout.Toggle("[Experimental] 360 Video ", _player.Stereo360Experimental);
            EditorGUILayout.Space();
            
            _player.bypassSpatialAudio = EditorGUILayout.Toggle("[Experimental] Bypass Spatializer ", _player.bypassSpatialAudio);
            EditorGUILayout.Space();
            
            _player.advancedMemoryHandling = EditorGUILayout.Toggle("[Experimental] Memory Optimizations ", _player.advancedMemoryHandling);
            EditorGUILayout.Space();
            
            _player.advancedBandwidthHandling = EditorGUILayout.Toggle("[Experimental] Network Optimizations ", _player.advancedBandwidthHandling);
            EditorGUILayout.Space();
            
            _player.localPlaybackSpeed = EditorGUILayout.Slider("Playback Speed", _player.localPlaybackSpeed, 0.5f, 2.0f);
            EditorGUILayout.Space();

            _player.projectionMesh = (MeshRenderer) EditorGUILayout.ObjectField("Projection Mesh", _player.projectionMesh, typeof(MeshRenderer));
            EditorGUILayout.Space();
            
            _player.interactiveUI = EditorGUILayout.Toggle("Use Default Interactive Library UI", _player.interactiveUI);
            EditorGUILayout.Space();
            
            _player.autoplay = EditorGUILayout.Toggle("Use Autoplay", _player.autoplay);
            EditorGUILayout.Space();
            
            reorderableList.DoLayoutList();
        }

    }
