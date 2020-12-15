using System;
using UnityEngine;

namespace ABI.CCK.Components
{
    public class CVRPickupObject : MonoBehaviour
    {
        public enum GripType
        {
            Free = 1,
            Origin = 2
        }
        
        public GripType gripType = GripType.Free;
        public Transform gripOrigin;

        public bool disallowTheft;

        private Transform _originalParent;
    }
}