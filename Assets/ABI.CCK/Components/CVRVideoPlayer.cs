using System;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace ABI.CCK.Components
{
    [RequireComponent(typeof(CVRVideoSyncSolver))]
    public class CVRVideoPlayer : MonoBehaviour
    {
        public enum AudioMode
        {
            Direct2D = 1,
            Direct3D = 2,
            RoomScale3D = 3,
        }
        
        [HideInInspector]
        public string playerId;

        public bool Stereo360Experimental;
        public AudioMode AudioPlaybackMode = AudioMode.Direct3D;
        [Range(0.5f,2)] public float localPlaybackSpeed = 1.0f;

        public RenderTexture ProjectionTexture;
        public bool interactiveUI = true;
        public bool autoplay;

        public List<CVRVideoPlayerPlaylist> entities = new List<CVRVideoPlayerPlaylist>();
        
        protected void OnEnable()
        {
            playerId = Guid.NewGuid() + gameObject.GetInstanceID().ToString();
        }

        public void Play() {}
        public void Pause() {}
        public void Previous() {}
        public void Next() {}
        public void Resync() {}

    }

    [System.Serializable]
    public class CVRVideoPlayerPlaylist
    {
        public string playlistThumbnailUrl;
        public string playlistTitle;
        public List<CVRVideoPlayerPlaylistEntity> playlistVideos = new List<CVRVideoPlayerPlaylistEntity>();
        
        #if UNITY_EDITOR
        private CVRVideoPlayerPlaylistEntity entity;
        public ReorderableList list;
        public bool isCollapsed;

        public void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            GUI.Label(_rect, "Playlist - Videos");
        }

        public void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > playlistVideos.Count) return;
            entity = playlistVideos[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isCollapsed = EditorGUI.Foldout(_rect, entity.isCollapsed, "Title", true);
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.videoTitle = EditorGUI.TextField(_rect, entity.videoTitle);
            
            if (!entity.isCollapsed) return;
        
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Video Url");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.videoUrl = EditorGUI.TextField(_rect, entity.videoUrl);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
        
            EditorGUI.LabelField(_rect, "Thumbnail Url");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.thumbnailUrl = EditorGUI.TextField(_rect, entity.thumbnailUrl);
        
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Start");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.introEndInSeconds = EditorGUI.IntField(_rect, entity.introEndInSeconds);
        
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "End");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.creditsStartInSeconds = EditorGUI.IntField(_rect, entity.creditsStartInSeconds);
        }

        public float OnHeightElement(int index)
        {
            if (index > playlistVideos.Count) return EditorGUIUtility.singleLineHeight * 1.25f;
            entity = playlistVideos[index];
            
            if(!entity.isCollapsed) return EditorGUIUtility.singleLineHeight * 1.25f;
            
            return EditorGUIUtility.singleLineHeight * 6.25f;
        }

        public void OnAdd(ReorderableList reorderableList)
        {
            playlistVideos.Add(null);
        }

        public void OnChanged(ReorderableList reorderableList)
        {
            //EditorUtility.SetDirty(target);
        }

        public ReorderableList GetReorderableList()
        {
            if (list == null)
            {
                list = new ReorderableList(playlistVideos, typeof(CVRVideoPlayerPlaylistEntity), true, true, true,
                    true);
                list.drawHeaderCallback = OnDrawHeader;
                list.drawElementCallback = OnDrawElement;
                list.elementHeightCallback = OnHeightElement;
                list.onAddCallback = OnAdd;
                list.onChangedCallback = OnChanged;
            }

            return list;
        }
        #endif
    } 
    
    [System.Serializable]
    public class CVRVideoPlayerPlaylistEntity
    {
        public string videoUrl;
        public string videoTitle;
        public int introEndInSeconds;
        public int creditsStartInSeconds;
        public string thumbnailUrl;
        public bool isCollapsed;
    }
    
}