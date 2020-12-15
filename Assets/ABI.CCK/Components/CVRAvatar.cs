using ABI.CCK.Scripts;
using UnityEngine;

namespace ABI.CCK.Components
{
    [RequireComponent(typeof(CVRAssetInfo))]
    [RequireComponent(typeof(Animator))]
    [ExecuteInEditMode]
    public class CVRAvatar : MonoBehaviour
    {
        public enum CVRAvatarVoiceParent
        {
            Head = 0,
            LeftHand = 2,
            RightHand = 3,
            Hips = 4
        }
        
        [Space] [Header("General avatar settings")] [Space]
        public Vector3 viewPosition = new Vector3(0, 0.1f, 0);
        public Vector3 voicePosition = new Vector3(0, 0.1f, 0);
        public CVRAvatarVoiceParent voiceParent = CVRAvatarVoiceParent.Head;
        public bool useEyeMovement = true;
        public bool useBlinkBlendshapes;
        public bool useVisemeLipsync;
        public SkinnedMeshRenderer bodyMesh;

        public string[] blinkBlendshape = new string[4];
        public string[] visemeBlendshapes = new string[15];

        [Space] [Header("Avatar customization")] [Space]
        public AnimatorOverrideController overrides;

        public bool avatarUsesAdvancedSettings = false;
        public CVRAdvancedAvatarSettings avatarSettings = null;
        
        void OnDrawGizmosSelected()
        {
            var scale = transform.localScale;
            scale.x = 1 / scale.x;
            scale.y = 1 / scale.y;
            scale.z = 1 / scale.z;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.TransformPoint(Vector3.Scale(viewPosition, scale)), 0.01f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(Vector3.Scale(voicePosition, scale)), 0.01f);
        }
        
        private void OnEnable()
        {
            CVRAssetInfo info = gameObject.GetComponent<CVRAssetInfo>();
            info.type = CVRAssetInfo.AssetType.Avatar;
        }

    }
}
