using System;
using System.Collections.Generic;
using UnityEngine;

namespace ABI.CCK.Components
{
    [System.Serializable]
    public class CVRInteractableAction
    {
        public enum ActionRegister
        {
            OnGrab = 1,
            OnDrop = 2,
            OnInteractDown = 3,
            OnInteractUp = 4,
            OnEnterTrigger = 5,
            OnExitTrigger = 6,
            OnEnterCollider = 7,
            OnExitCollider = 8,
            OnEnable = 9,
            OnDisable = 10,
            OnTimer = 11,
            OnParticleHit = 12,
            OnVariableBufferUpdate = 13,
            OnVariableBufferComparision = 14,
            OnCron = 15,
            OnPointerEnter = 16,
            OnWorldTrigger = 17
        }

        public enum ExecutionType
        {
            LocalNotNetworked = 1,
            GlobalNetworked = 2,
            GlobalInstanceOwnerOnly = 3,
        }

        public float delay = 0f;
        
        public List<CVRInteractableActionOperation> operations = new List<CVRInteractableActionOperation>();
        
        public ActionRegister actionType = ActionRegister.OnInteractDown;
        public ExecutionType execType = ExecutionType.GlobalNetworked;

        public LayerMask layerMask = 0;

        public float floatVal = 0;
        public float floatVal2 = 0;
        public float floatVal3 = 0;
        public bool boolVal;
        public CVRVariableBuffer varBufferVal;
        public CVRVariableBuffer varBufferVal2;
        public string stringVal = "";

        [HideInInspector]
        public string guid = "";

        private bool IsInLayerMask(GameObject obj, LayerMask inLayerMask)
        {
            return ((inLayerMask.value & (1 << obj.layer)) > 0);
        }
    }
}