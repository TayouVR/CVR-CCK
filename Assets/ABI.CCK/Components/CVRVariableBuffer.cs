using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable

namespace ABI.CCK.Components
{
    public class CVRVariableBuffer : MonoBehaviour
    {
        public float defaultValue = 0f;
        
        [HideInInspector]
        public float value = 0f;
        
        [HideInInspector]
        public List<CVRInteractable> affectedInteractables = new List<CVRInteractable>();
        private bool sendUpdate = true;

        public void Start()
        {
            value = defaultValue;
        }

        public void AddInteracteable(CVRInteractable interactable)
        {
            if (!affectedInteractables.Contains(interactable))
            {
                affectedInteractables.Add(interactable);
            }

            RemoveOrphans();
        }

        private void RemoveOrphans()
        {
            foreach (var interactable in affectedInteractables)
            {
                var included = false;
                foreach (var action in interactable.actions)
                {
                    if (action.varBufferVal == this) included = true;
                    if (action.varBufferVal2 == this) included = true;
                    foreach (var operation in action.operations)
                    {
                        if (operation.varBufferVal == this) included = true;
                        if (operation.varBufferVal2 == this) included = true;
                        if (operation.varBufferVal3 == this) included = true;
                    }
                }

                if (!included) affectedInteractables.Remove(interactable);
            }
        }
    }
}