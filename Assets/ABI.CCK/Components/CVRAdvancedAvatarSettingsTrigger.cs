using System.Collections.Generic;

using UnityEngine;

namespace ABI.CCK.Components
{
    public class CVRAdvancedAvatarSettingsTrigger : MonoBehaviour
    {
        public Vector3 areaSize = new Vector3(0.05f, 0.05f, 0.05f);
        public Vector3 areaOffset = Vector3.zero;
        public string settingName;
        public float settingValue = 0;

        public bool useAdvancedTrigger = false;
        public bool isNetworkInteractable = true;
        [SerializeField]
        public List<CVRPointer> allowedPointer = new List<CVRPointer>();

        public List<CVRAdvancedAvatarSettingsTriggerTask> enterTasks = new List<CVRAdvancedAvatarSettingsTriggerTask>();
        public List<CVRAdvancedAvatarSettingsTriggerTask> exitTasks = new List<CVRAdvancedAvatarSettingsTriggerTask>();

        public void Trigger()
        {
            
        }

        public void EnterTrigger()
        {
            
        }

        public void ExitTrigger()
        {
            
        }

        private void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.cyan;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawCube(areaOffset, areaSize);
            }
        }
    }

    [System.Serializable]
    public class CVRAdvancedAvatarSettingsTriggerTask
    {
        public string settingName;
        public float settingValue = 0;
        public float delay = 0;
        
        public enum UpdateMethod
        {
            Override = 1,
            Add = 2,
            Subtract = 3
        }

        public CVRAdvancedAvatarSettingsTriggerTask.UpdateMethod updateMethod = UpdateMethod.Override;
    }
}