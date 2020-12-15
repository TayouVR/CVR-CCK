using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABI.CCK.Components
{
    [System.Serializable]
    public class CVRInteractableActionOperation
    {
        public enum ActionType
        {
            SetGameObjectActive = 1,
            //SetComponentActive = 2,
            SetAnimatorFloatValue = 3,
            SetAnimatorBoolValue = 4,
            SetAnimatorIntValue = 17,
            TriggerAnimatorTrigger = 18,
            SpawnObject = 5,
            TeleportPlayer = 6,
            TeleportObject = 7,
            ToggleAnimatorBoolValue = 8,
            SetAnimatorFloatRandom = 9,
            SetAnimatorBoolRandom = 10,
            SetAnimatorIntRandom = 19,
            SetAnimatorFloatByVar = 11,
            SetAnimatorIntByVar = 20,
            VariableBufferArithmetic = 12,
            DisplayWorldDetailPage = 13,
            DisplayInstanceDetailPage = 14,
            DisplayAvatarDetailPage = 15,
            SitAtPosition = 16,
        }
        
        public ActionType type = ActionType.SetGameObjectActive;
        
        public List<GameObject> targets = new List<GameObject>();

        public float floatVal;
        public string stringVal;
        public bool boolVal;
        public GameObject gameObjectVal;
        public float floatVal2 = 0f;
        public float floatVal3 = 0f;
        public float floatVal4 = 0f;
        public CVRVariableBuffer varBufferVal;
        public CVRVariableBuffer varBufferVal2;
        public CVRVariableBuffer varBufferVal3;
        public AnimationClip animationVal;
    }
}