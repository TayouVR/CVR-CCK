using System.Collections;
using System.Collections.Generic;
using ABI.CCK.Components;
using UnityEngine;

namespace ABI.CCK.Components
{
    [RequireComponent(typeof(CVRAssetInfo))]
    [ExecuteInEditMode]
    public class CVRSpawnable : MonoBehaviour
    {
        public bool useAdditionalValues;
        
        public List<CVRSpawnableValue> syncValues = new List<CVRSpawnableValue>();
        
        private void OnEnable()
        {
            CVRAssetInfo info = gameObject.GetComponent<CVRAssetInfo>();
            info.type = CVRAssetInfo.AssetType.Spawnable;
        }
    }

    [System.Serializable]
    public class CVRSpawnableValue
    {
        public string name;
        public float startValue;
        
        public enum UpdatedBy
        {
            None = 0,
            SystemTime = 1,
            WorldTime = 2,
            SpawnerPositionX = 3,
            SpawnerPositionY = 4,
            SpawnerPositionZ = 5,
            SpawnerDistance = 6,
            SpawnerLookDirectionX = 7,
            SpawnerLookDirectionY = 8,
            SpawnerLookDirectionZ = 9,
            SpawnerLeftHandDirectionX = 10,
            SpawnerLeftHandDirectionY = 11,
            SpawnerLeftHandDirectionZ = 12,
            SpawnerRightHandDirectionX = 13,
            SpawnerRightHandDirectionY = 14,
            SpawnerRightHandDirectionZ = 15,
            SpawnerLeftGrip = 16,
            SpawnerRightGrip = 17,
            SpawnerLeftTrigger = 18,
            SpawnerRightTrigger = 19,
            OwnerLeftGrip = 20,
            OwnerRightGrip = 21,
            OwnerLeftTrigger = 22,
            OwnerRightTrigger = 23,
            OwnerCurrentGrip = 24,
            OwnerCurrentTrigger = 25
        }

        public UpdatedBy updatedBy = UpdatedBy.None;

        public enum UpdateMethod
        {
            Override = 1,
            AddToDefault = 2,
            AddToCurrent = 3,
            SubtractFromDefault = 4,
            SubtractFromCurrent = 5,
            MultiplyWithDefault = 6,
            DefaultDividedByCurrent = 7,
        }

        public UpdateMethod updateMethod = UpdateMethod.Override;

        public Animator animator;

        public string animatorParameterName;
    }
}