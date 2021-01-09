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
    private CVRVideoPlayerPlaylist entity = null;
    
    private const string TypeLabel = "Playlists";

    private void OnEnable()
    {
        if (_player == null) _player = (CVRVideoPlayer) target;
    
        reorderableList = new ReorderableList(_player.entities, typeof(CVRVideoPlayerPlaylist), true, true, true, true);
        reorderableList.drawHeaderCallback = OnDrawHeader;
        reorderableList.drawElementCallback = OnDrawElement;
        reorderableList.elementHeightCallback = OnHeightElement;
        reorderableList.onAddCallback = OnAdd;
        reorderableList.onChangedCallback = OnChanged;
    }

    private float OnHeightElement(int index)
    {
        var height = 0f;

        if (!_player.entities[index].isCollapsed) return EditorGUIUtility.singleLineHeight * 1.25f;

        height += EditorGUIUtility.singleLineHeight * (3f + 2.5f);

        if (_player.entities[index].playlistVideos.Count == 0) height += 1.25f * EditorGUIUtility.singleLineHeight;
        
        foreach (var entry in _player.entities[index].playlistVideos)
        {
            height += (entry.isCollapsed ? 6.25f : 1.25f) * EditorGUIUtility.singleLineHeight;
        }

        return height;
    }

    private void OnDrawHeader(Rect rect)
    {
        Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

        GUI.Label(_rect, TypeLabel);
    }

    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index > _player.entities.Count) return;
        
        rect.y += 2;
        rect.x += 12;
        rect.width -= 12;
        Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

        _player.entities[index].isCollapsed = EditorGUI.Foldout(_rect, _player.entities[index].isCollapsed, "Playlist Title", true);
        _rect.x += 80;
        _rect.width = rect.width - 80;
        _player.entities[index].playlistTitle = EditorGUI.TextField(_rect, _player.entities[index].playlistTitle);
        
        if (!_player.entities[index].isCollapsed) return;
        
        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
        _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(_rect, "Thumbnail Url");
        _rect.x += 80;
        _rect.width = rect.width - 80;
        _player.entities[index].playlistThumbnailUrl = EditorGUI.TextField(_rect, _player.entities[index].playlistThumbnailUrl);
            
        rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
        _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

        var videoList = _player.entities[index].GetReorderableList();
        videoList.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
    }
    
    private void OnAdd(ReorderableList list)
    {
        _player.entities.Add(null);
    }

    private void OnChanged(ReorderableList list)
    {
        //EditorUtility.SetDirty(target);
    }
    
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please note that experimental settings are not properly implemented yet and might break at anytime.", MessageType.Warning);
        EditorGUILayout.Space();
        
        _player.Stereo360Experimental = EditorGUILayout.Toggle("[Experimental] 360 Video ", _player.Stereo360Experimental);
        EditorGUILayout.Space();
        
        _player.AudioPlaybackMode = (CVRVideoPlayer.AudioMode) EditorGUILayout.EnumPopup("[Experimental] Audio Playback Mode ",  _player.AudioPlaybackMode);
        EditorGUILayout.Space();
        
        _player.localPlaybackSpeed = EditorGUILayout.Slider("Playback Speed", _player.localPlaybackSpeed, 0.5f, 2.0f);
        EditorGUILayout.Space();

        _player.ProjectionTexture = (RenderTexture) EditorGUILayout.ObjectField("Projection Texture", _player.ProjectionTexture, typeof(RenderTexture));
        EditorGUILayout.Space();
        if (_player.ProjectionTexture == null)
        {
            EditorGUILayout.HelpBox("The video player output texture is empty, please fill it or no video will be drawn.", MessageType.Warning);
            if (GUILayout.Button("Create Sample Render Texture"))
            {
                RenderTexture tex = new RenderTexture(1920,1080,24);
                if(!AssetDatabase.IsValidFolder("Assets/ABI.Generated"))
                    AssetDatabase.CreateFolder("Assets", "ABI.Generated");
                if(!AssetDatabase.IsValidFolder("Assets/ABI.Generated/VideoPlayer"))
                    AssetDatabase.CreateFolder("Assets/ABI.Generated", "VideoPlayer");
                if(!AssetDatabase.IsValidFolder("Assets/ABI.Generated/VideoPlayer/RenderTextures"))
                    AssetDatabase.CreateFolder("Assets/ABI.Generated/VideoPlayer", "RenderTextures");
                AssetDatabase.CreateAsset(tex, "Assets/ABI.Generated/VideoPlayer/RenderTextures/"+_player.gameObject.GetInstanceID()+".renderTexture");
                _player.ProjectionTexture = tex;
            }
        }

        EditorGUILayout.HelpBox("To use this feature, you have to use the prefab provided with the CCK.", MessageType.Info);
        _player.interactiveUI = EditorGUILayout.Toggle("Use Default Interactive Library UI", _player.interactiveUI);
        EditorGUILayout.Space();
        
        _player.autoplay = EditorGUILayout.Toggle("Play on Startup", _player.autoplay);
        EditorGUILayout.Space();
        
        reorderableList.DoLayoutList();
    }

}
