using System.Collections.Generic;
using ABI.CCK.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace ABI.CCK.Scripts.Editor
{
    [CustomEditor(typeof(ABI.CCK.Components.CVRSpawnable), true)]
    public class CCK_CVRSpawnableEditor : UnityEditor.Editor
    {
        private CVRSpawnable _spawnable;
        private ReorderableList reorderableList;
        private CVRSpawnableValue entity;
        
        private void InitializeList()
        {
            if (!_spawnable.useAdditionalValues) return;
            
            reorderableList = new ReorderableList(_spawnable.syncValues, typeof(CVRAdvancedSettingsEntry), true, true, true, true);
            reorderableList.drawHeaderCallback = OnDrawHeader;
            reorderableList.drawElementCallback = OnDrawElement;
            reorderableList.elementHeightCallback = OnHeightElement;
            reorderableList.onAddCallback = OnAdd;
            reorderableList.onChangedCallback = OnChanged; 
        }

        private void OnChanged(ReorderableList list)
        {
            //EditorUtility.SetDirty(target);
        }

        private void OnAdd(ReorderableList list)
        {
            if (_spawnable.syncValues.Count < 9)
            {
                _spawnable.syncValues.Add(new CVRSpawnableValue());
                Repaint();
            }
        }

        private float OnHeightElement(int index)
        {
            return EditorGUIUtility.singleLineHeight * 7.5f;
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > _spawnable.syncValues.Count) return;
            entity = _spawnable.syncValues[index];
            
            rect.y += 2;
            Rect _rect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Name");
            _rect.x += 120;
            _rect.width = rect.width - 120;
            entity.name = EditorGUI.TextField(_rect, entity.name);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Start Value");
            _rect.x += 120;
            _rect.width = rect.width - 120;
            entity.startValue = EditorGUI.FloatField(_rect, entity.startValue);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Update Type");
            _rect.x += 120;
            _rect.width = rect.width - 120;
            entity.updatedBy = (CVRSpawnableValue.UpdatedBy) EditorGUI.EnumPopup(_rect, entity.updatedBy);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Update Method");
            _rect.x += 120;
            _rect.width = rect.width - 120;
            entity.updateMethod = (CVRSpawnableValue.UpdateMethod) EditorGUI.EnumPopup(_rect, entity.updateMethod);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Connected Animator");
            _rect.x += 120;
            _rect.width = rect.width - 120;
            entity.animator = (Animator) EditorGUI.ObjectField(_rect, entity.animator, typeof(Animator), true);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Animator Parameter");
            _rect.x += 120;
            _rect.width = rect.width - 120;

            var parameters = new List<string>();
            parameters.Add("-none-");
            var parameterIndex = 0;
            if (entity.animator != null)
            {
                if (entity.animator.runtimeAnimatorController != null)
                {
                    foreach (var parameter in ((UnityEditor.Animations.AnimatorController) entity.animator.runtimeAnimatorController).parameters)
                    {
                        if (parameter.type == AnimatorControllerParameterType.Float)
                        {
                            parameters.Add(parameter.name);
                        }
                    }

                    parameterIndex = parameters.FindIndex(match => match == entity.animatorParameterName);
                }
            }
            
            parameterIndex = EditorGUI.Popup(_rect, parameterIndex, parameters.ToArray());
            entity.animatorParameterName = parameters[parameterIndex];
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            GUI.Label(_rect, "Values");
        }

        public override void OnInspectorGUI()
        {
            if (_spawnable == null) _spawnable = (CVRSpawnable) target;
            
            GUILayout.BeginVertical("HelpBox");
            
            GUILayout.BeginHorizontal ();
            _spawnable.useAdditionalValues = EditorGUILayout.Toggle (_spawnable.useAdditionalValues, GUILayout.Width(16));
            EditorGUILayout.LabelField ("Enable Sync Values", GUILayout.Width(250));
            GUILayout.EndHorizontal ();

            if (_spawnable.useAdditionalValues)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("GroupBox");
                
                Rect _rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.ProgressBar(_rect, _spawnable.syncValues.Count / 9f, _spawnable.syncValues.Count + " of 9 Synced Parameter Slots used");
                
                if (reorderableList == null) InitializeList();
                reorderableList.DoLayoutList();
                
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndVertical();
        }
    }
}