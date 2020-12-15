using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ABI.CCK.Components
{
    [RequireComponent(typeof(CVRVideoSyncSolver))]
    public class CVRVideoPlayer : MonoBehaviour
    {
        [HideInInspector]
        public string playerId;

        public bool Stereo360Experimental;
        public bool bypassSpatialAudio;
        [Range(0.5f,2)] public float localPlaybackSpeed = 1.0f;

        public bool advancedMemoryHandling;
        public bool advancedBandwidthHandling;

        public MeshRenderer projectionMesh;
        public bool interactiveUI = true;
        public bool autoplay;

        protected void OnEnable()
        {
            playerId = Guid.NewGuid() + gameObject.GetInstanceID().ToString();
        }
        
        public List<CVRVideoPlayerPlaylistEntity> entities = new List<CVRVideoPlayerPlaylistEntity>();

        public void Play()
        {
            
        }

        public void Pause()
        {
            
        }

        public void Previous()
        {
            
        }

        public void Next()
        {
            
        }

    }
    
    [System.Serializable]
    public class CVRVideoPlayerPlaylistEntity
    {
        public string videoUrl;
        public string videoTitle;
        public int introEndInSeconds;
        public int creditsStartInSeconds;
        public Texture2D thumbnail;
    }
    
}