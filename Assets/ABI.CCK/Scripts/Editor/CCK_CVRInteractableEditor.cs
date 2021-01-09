using System;
using System.Collections.Generic;
using ABI.CCK.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(ABI.CCK.Components.CVRInteractable))]
    [CanEditMultipleObjects]
    public class CCK_CVRInteractableEditor : UnityEditor.Editor
    {
        private CVRInteractable _interactable;

        private static string[] gameObjectStates = new string[] {"Enable", "Disable", "Toggle" };
        private static string[] variableComparisions = new string[] {"Buffer -> static", "Buffer -> Buffer" };
        private static string[] variableComparitors = new string[] {"==", ">=", ">", "<", "<=", "!=" };
        private static string[] arithmeticConstellations = new string[] {"Buffer -> static", "Buffer -> Buffer", "Buffer -> Random" };
        private static string[] arithmeticOperators = new string[] {"+", "-", "*", "÷", "mod", "pow", "log"};
        private static string[] timerModes = new string[] {"once on enable", "repeat", "deactivate self"};
        
        public override void OnInspectorGUI()
        {
            if (_interactable == null) _interactable = (CVRInteractable)target;

            _interactable.tooltip = EditorGUILayout.TextField("Tooltip:", _interactable.tooltip);
            
            EditorGUILayout.LabelField("Triggers:", EditorStyles.boldLabel);

            foreach (CVRInteractableAction trigger in _interactable.actions)
            {
                GUILayout.BeginVertical("HelpBox");
                GUILayout.BeginHorizontal ();

                trigger.actionType = (CVRInteractableAction.ActionRegister) EditorGUILayout.EnumPopup("Trigger:", trigger.actionType);
                
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal ();
                GUILayout.BeginVertical ("GroupBox");
                
                trigger.execType = (CVRInteractableAction.ExecutionType) EditorGUILayout.EnumPopup("Broadcast Type:", trigger.execType);

                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnEnterCollider ||
                    trigger.actionType == CVRInteractableAction.ActionRegister.OnExitCollider ||
                    trigger.actionType == CVRInteractableAction.ActionRegister.OnEnterTrigger ||
                    trigger.actionType == CVRInteractableAction.ActionRegister.OnExitTrigger)
                {
                    LayerMask tempMask = EditorGUILayout.MaskField("Layer:", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(trigger.layerMask), InternalEditorUtility.layers);
                    trigger.layerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
                }
                
                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnTimer)
                {
                    trigger.floatVal = EditorGUILayout.FloatField("Seconds:", trigger.floatVal);
                    trigger.floatVal2 = EditorGUILayout.Popup("Mode:", (int)trigger.floatVal2, timerModes);
                }
                
                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnInteractDown ||
                    trigger.actionType == CVRInteractableAction.ActionRegister.OnInteractUp)
                {
                    trigger.floatVal = EditorGUILayout.FloatField("Distance:", trigger.floatVal);
                }
                
                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnVariableBufferUpdate)
                {
                    trigger.varBufferVal = (CVRVariableBuffer) EditorGUILayout.ObjectField("Value:", trigger.varBufferVal, typeof(CVRVariableBuffer), true);
                }
                
                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnVariableBufferComparision)
                {
                    trigger.floatVal = EditorGUILayout.Popup("Type:", (int)trigger.floatVal, variableComparisions);
                    
                    trigger.varBufferVal = (CVRVariableBuffer) EditorGUILayout.ObjectField("Value 1:", trigger.varBufferVal, typeof(CVRVariableBuffer), true);

                    if (trigger.varBufferVal != null)
                    {
                        trigger.varBufferVal.AddInteracteable(_interactable);
                    }
                    
                    trigger.floatVal2 = EditorGUILayout.Popup("Comparitor:", (int)trigger.floatVal2, variableComparitors);
                    
                    if (trigger.floatVal == 0)
                    {
                        trigger.floatVal3 = EditorGUILayout.FloatField("Value 2:", trigger.floatVal3);
                    }
                    else
                    {
                        trigger.varBufferVal2 = (CVRVariableBuffer) EditorGUILayout.ObjectField("Value 2:", trigger.varBufferVal2, typeof(CVRVariableBuffer), true);
                        
                        if (trigger.varBufferVal2 != null)
                        {
                            trigger.varBufferVal2.AddInteracteable(_interactable);
                        }
                    }
                }

                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnCron)
                {
                    trigger.stringVal = EditorGUILayout.TextField("Cron String", trigger.stringVal);
                }
                
                if (trigger.actionType == CVRInteractableAction.ActionRegister.OnWorldTrigger)
                {
                    trigger.floatVal = EditorGUILayout.IntField("Index", (int) trigger.floatVal);
                }
                
                trigger.delay = EditorGUILayout.FloatField("Delay (Seconds):", trigger.delay);
                
                if(trigger.varBufferVal != null) trigger.varBufferVal.AddInteracteable(_interactable);
                if(trigger.varBufferVal2 != null) trigger.varBufferVal2.AddInteracteable(_interactable);
                
                EditorGUILayout.LabelField("Actions:", EditorStyles.boldLabel);

                for (int j = 0; j < trigger.operations.Count; j++)
                {
                    GUILayout.BeginVertical ("GroupBox");
                    
                    trigger.operations[j].type = (CVRInteractableActionOperation.ActionType) EditorGUILayout.EnumPopup("Action Type:", trigger.operations[j].type);

                    switch (trigger.operations[j].type)
                    {
                        case CVRInteractableActionOperation.ActionType.SetGameObjectActive:

                            if (trigger.operations[j].targets.Count == 0)
                            {
                                trigger.operations[j].targets.Add(null);
                            }

                            if (trigger.operations[j].floatVal > 2)
                            {
                                trigger.operations[j].floatVal = 0;
                            }

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].floatVal = EditorGUILayout.Popup("State:", (int)trigger.operations[j].floatVal, gameObjectStates);

                            break;
                        
                        case CVRInteractableActionOperation.ActionType.TeleportPlayer:

                            trigger.operations[j].gameObjectVal = (GameObject)EditorGUILayout.ObjectField(
                                "Target Location:", 
                                trigger.operations[j].gameObjectVal, 
                                typeof(GameObject), 
                                true
                            );
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorBoolValue:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].boolVal = EditorGUILayout.Toggle("Value", trigger.operations[j].boolVal);
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorFloatValue:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].floatVal = EditorGUILayout.FloatField("Value", trigger.operations[j].floatVal);
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorIntValue:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].floatVal = EditorGUILayout.FloatField("Value", trigger.operations[j].floatVal);
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.TriggerAnimatorTrigger:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Trigger Name:", trigger.operations[j].stringVal);

                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SpawnObject:

                            RenderTargets(trigger.operations[j].targets, "Object");

                            trigger.operations[j].gameObjectVal = (GameObject)EditorGUILayout.ObjectField(
                                "Target Location:", 
                                trigger.operations[j].gameObjectVal, 
                                typeof(GameObject), 
                                true
                            );
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.TeleportObject:

                            RenderTargets(trigger.operations[j].targets, "Object");

                            trigger.operations[j].gameObjectVal = (GameObject)EditorGUILayout.ObjectField(
                                "Target Location:", 
                                trigger.operations[j].gameObjectVal, 
                                typeof(GameObject), 
                                true
                            );
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.ToggleAnimatorBoolValue:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);

                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorFloatRandom:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].floatVal = EditorGUILayout.FloatField("Min:", trigger.operations[j].floatVal);
                            
                            trigger.operations[j].floatVal2 = EditorGUILayout.FloatField("Max:", trigger.operations[j].floatVal2);

                            break;

                        case CVRInteractableActionOperation.ActionType.SetAnimatorIntRandom:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].floatVal = EditorGUILayout.FloatField("Min:", trigger.operations[j].floatVal);
                            
                            trigger.operations[j].floatVal2 = EditorGUILayout.FloatField("Max:", trigger.operations[j].floatVal2);

                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorBoolRandom:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].floatVal = EditorGUILayout.FloatField("Chance (0-1):", trigger.operations[j].floatVal);

                            break;
                        
                        case CVRInteractableActionOperation.ActionType.VariableBufferArithmetic:

                            trigger.operations[j].floatVal = EditorGUILayout.Popup("Type:", (int)trigger.operations[j].floatVal, arithmeticConstellations);
                            
                            trigger.operations[j].varBufferVal = (CVRVariableBuffer) EditorGUILayout.ObjectField(
                                "Value 1:", 
                                trigger.operations[j].varBufferVal, 
                                typeof(CVRVariableBuffer), 
                                true
                            );
                            
                            trigger.operations[j].floatVal2 = EditorGUILayout.Popup("Operator:", (int)trigger.operations[j].floatVal2, arithmeticOperators);

                            switch ((int) trigger.operations[j].floatVal)
                            {
                                case 0:
                                    trigger.operations[j].floatVal3 = EditorGUILayout.FloatField("Value 2:", trigger.operations[j].floatVal3);
                                    break;
                                case 1:
                                    trigger.operations[j].varBufferVal2 = (CVRVariableBuffer) EditorGUILayout.ObjectField(
                                        "Value 2:", 
                                        trigger.operations[j].varBufferVal2, 
                                        typeof(CVRVariableBuffer), 
                                        true
                                    );
                                    break;
                                case 2:
                                    trigger.operations[j].floatVal3 = EditorGUILayout.FloatField("Min:", trigger.operations[j].floatVal3);
                                    trigger.operations[j].floatVal4 = EditorGUILayout.FloatField("Max:", trigger.operations[j].floatVal4);
                                    break;
                            }
                            
                            trigger.operations[j].varBufferVal3 = (CVRVariableBuffer) EditorGUILayout.ObjectField(
                                "Result:", 
                                trigger.operations[j].varBufferVal3, 
                                typeof(CVRVariableBuffer), 
                                true
                            );

                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorFloatByVar:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].varBufferVal = (CVRVariableBuffer) EditorGUILayout.ObjectField(
                                "Value:", 
                                trigger.operations[j].varBufferVal, 
                                typeof(CVRVariableBuffer), 
                                true
                            );
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SetAnimatorIntByVar:

                            RenderTargets(trigger.operations[j].targets);

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Parameter Name:", trigger.operations[j].stringVal);
                            
                            trigger.operations[j].varBufferVal = (CVRVariableBuffer) EditorGUILayout.ObjectField(
                                "Value:", 
                                trigger.operations[j].varBufferVal, 
                                typeof(CVRVariableBuffer), 
                                true
                            );
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.DisplayWorldDetailPage:

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("World GUID:", trigger.operations[j].stringVal);
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.DisplayInstanceDetailPage:

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Instance GUID:", trigger.operations[j].stringVal);
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.DisplayAvatarDetailPage:

                            trigger.operations[j].stringVal = EditorGUILayout.TextField("Avatar GUID:", trigger.operations[j].stringVal);
                            
                            break;
                        
                        case CVRInteractableActionOperation.ActionType.SitAtPosition:

                            trigger.operations[j].gameObjectVal = (GameObject)EditorGUILayout.ObjectField(
                                "Sitting Location:", 
                                trigger.operations[j].gameObjectVal, 
                                typeof(GameObject), 
                                true
                            );
                            
                            trigger.operations[j].targets[0] = (GameObject)EditorGUILayout.ObjectField(
                                "Exit Location:", 
                                trigger.operations[j].targets[0], 
                                typeof(GameObject), 
                                true
                            );
                            
                            trigger.operations[j].animationVal = (AnimationClip)EditorGUILayout.ObjectField(
                                "Overwrite Animation:", 
                                trigger.operations[j].animationVal, 
                                typeof(AnimationClip), 
                                true
                            );
                            
                            break;
                    }
                    
                    if(trigger.operations[j].varBufferVal != null) trigger.operations[j].varBufferVal.AddInteracteable(_interactable);
                    if(trigger.operations[j].varBufferVal2 != null) trigger.operations[j].varBufferVal2.AddInteracteable(_interactable);
                    if(trigger.operations[j].varBufferVal3 != null) trigger.operations[j].varBufferVal3.AddInteracteable(_interactable);

                    GUILayout.EndVertical ();
                }
                
                if (GUILayout.Button("Remove Action"))
                {
                    trigger.operations.RemoveAt(trigger.operations.Count - 1);
                }
                if (GUILayout.Button("Add Action"))
                {
                    trigger.operations.Add(new CVRInteractableActionOperation());
                }
                
                GUILayout.EndVertical ();
                GUILayout.EndHorizontal ();
                GUILayout.EndVertical ();
                
            }
            
            if (GUILayout.Button("Remove Trigger"))
            {
                _interactable.actions.RemoveAt(_interactable.actions.Count - 1);
            }
            
            if (GUILayout.Button("Add Trigger"))
            {
                _interactable.actions.Add(new CVRInteractableAction());
            }
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty (target);
            }
        }

        private void RenderTargets(List<GameObject> targets)
        {
            RenderTargets(targets, "Target");
        }

        private void RenderTargets(List<GameObject> targets, String caption)
        {
            if (targets != null)
            {
                for (int k = 0; k < targets.Count; k++)
                {
                    targets[k] = (GameObject) EditorGUILayout.ObjectField(
                        caption + ":",
                        targets[k],
                        typeof(GameObject),
                        true
                    );
                }
            }

            if (GUILayout.Button("Remove " + caption))
            {
                targets.RemoveAt(targets.Count - 1);
            }
            if (GUILayout.Button("Add " + caption))
            {
                targets.Add(null);
            }
        }
    }
}