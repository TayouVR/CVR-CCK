using UnityEngine;

namespace ABI.CCK.Components
{
    public class CVRWorldModifiers : MonoBehaviour
    {
        [Space] [Header("World modification")] [Space]
        [Range(1,5)] public float voiceCommsMinDistance = 1.5f;
        [Range(3,50)] public float voiceCommsMaxDistance = 10f;
        public bool disableJumping;
        public bool disableTeleportRequests;

        [Space] [Header("Newton Runtime")] [Space]
        public bool newtonEnabled;
        public bool autoRegisterFlavors;
        public bool useGameObjectRegistry;
    }
}
