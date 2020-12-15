using UnityEngine;
using System.Collections.Generic;

namespace ABI.CCK.Components
{
    [RequireComponent(typeof(CVRAssetInfo))]
    [ExecuteInEditMode]
    public class CVRWorld : MonoBehaviour
    {

        public enum SpawnRule
        {
            InOrder = 1,
            Random = 2,
        }
        public enum RespawnBehaviour
        {
            Respawn = 1,
            Destroy = 2,
        }
    
        [Space] [Header("World settings")] [Space]
        public GameObject[] spawns = new GameObject[0];
        public SpawnRule spawnRule = SpawnRule.Random;
        public GameObject referenceCamera;
        public int respawnHeightY = -100;
        public RespawnBehaviour objectRespawnBehaviour = RespawnBehaviour.Destroy;
        
        [Space] [Header("Optional settings")] [Space]
        public CVRWarpPoint[] warpPoints = new CVRWarpPoint[0];

        [HideInInspector]
        public List <GameObject> dynamicPrefabs = new List<GameObject>();
        
        private void OnEnable()
        {
            CVRAssetInfo info = gameObject.GetComponent<CVRAssetInfo>();
            info.type = CVRAssetInfo.AssetType.World;
        }
    }
}
