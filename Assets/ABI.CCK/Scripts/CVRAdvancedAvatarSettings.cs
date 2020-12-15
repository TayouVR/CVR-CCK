﻿using System;
using System.Collections.Generic;
using ABI.CCK.Components;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine.Rendering;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;
using BlendTree = UnityEditor.Animations.BlendTree;
#endif

namespace ABI.CCK.Scripts
{
    [System.Serializable]
    public class CVRAdvancedAvatarSettings
    {
        public bool initialized = false;
        
        public RuntimeAnimatorController baseController;
        
        #if UNITY_EDITOR
        public AnimatorController animator;
        public AnimatorOverrideController overrides;
        #endif

        public List<CVRAdvancedSettingsEntry> settings = new List<CVRAdvancedSettingsEntry>();
    }

    [System.Serializable]
    public class CVRAdvancedSettingsEntry
    {
        public enum SettingsType
        {
            GameObjectToggle,
            GameObjectDropdown,
            MaterialColor,
            Slider,
            Joystick2D,
            Joystick3D,
            InputSingle,
            InputVector2,
            InputVector3
        }
        
        public SettingsType type = SettingsType.GameObjectToggle;
        
        public CVRAdvancesAvatarSettingBase setting
        {
            get
            {
                switch (type)
                {
                    case SettingsType.MaterialColor:
                        return materialColorSettings;
                    case SettingsType.GameObjectDropdown:
                        return dropDownSettings;
                    case SettingsType.Slider:
                        return sliderSettings;
                    case SettingsType.Joystick2D:
                        return joystick2DSetting;
                    case SettingsType.Joystick3D:
                        return joystick3DSetting;
                    case SettingsType.InputSingle:
                        return inputSingleSettings;
                    case SettingsType.InputVector2:
                        return inputVector2Settings;
                    case SettingsType.InputVector3:
                        return inputVector3Settings;
                    default:
                        return toggleSettings;
                }
            }
            set
            {
                switch (type)
                {
                    case SettingsType.MaterialColor:
                        materialColorSettings = (CVRAdvancedAvatarSettingMaterialColor) value;
                        break;
                    case SettingsType.GameObjectDropdown:
                        dropDownSettings = (CVRAdvancesAvatarSettingGameObjectDropdown) value;
                        break;
                    case SettingsType.Slider:
                        sliderSettings = (CVRAdvancesAvatarSettingSlider) value;
                        break;
                    case SettingsType.Joystick2D:
                        joystick2DSetting = (CVRAdvancesAvatarSettingJoystick2D) value;
                        break;
                    case SettingsType.Joystick3D:
                        joystick3DSetting = (CVRAdvancesAvatarSettingJoystick3D) value;
                        break;
                    case SettingsType.InputSingle:
                        inputSingleSettings = (CVRAdvancesAvatarSettingInputSingle) value;
                        break;
                    case SettingsType.InputVector2:
                        inputVector2Settings = (CVRAdvancesAvatarSettingInputVector2) value;
                        break;
                    case SettingsType.InputVector3:
                        inputVector3Settings = (CVRAdvancesAvatarSettingInputVector3) value;
                        break;
                    default:
                        toggleSettings = (CVRAdvancesAvatarSettingGameObjectToggle) value;
                        break;
                }
            }
        }

        [SerializeField]
        private CVRAdvancesAvatarSettingGameObjectToggle toggleSettings;
        [SerializeField]
        private CVRAdvancedAvatarSettingMaterialColor materialColorSettings;
        [SerializeField]
        private CVRAdvancesAvatarSettingGameObjectDropdown dropDownSettings;
        [SerializeField] 
        private CVRAdvancesAvatarSettingSlider sliderSettings;
        [SerializeField] 
        private CVRAdvancesAvatarSettingJoystick2D joystick2DSetting;
        [SerializeField] 
        private CVRAdvancesAvatarSettingJoystick3D joystick3DSetting;
        [SerializeField] 
        private CVRAdvancesAvatarSettingInputSingle inputSingleSettings;
        [SerializeField] 
        private CVRAdvancesAvatarSettingInputVector2 inputVector2Settings;
        [SerializeField] 
        private CVRAdvancesAvatarSettingInputVector3 inputVector3Settings;
        
        public string name;
        public string machineName;
    }

    [System.Serializable]
    public class CVRAdvancesAvatarSettingBase
    {
        #if UNITY_EDITOR

        [NonSerialized] public bool isCollapsed;
        public virtual void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            
        }
        #endif
    }

    [System.Serializable]
    public class CVRAdvancesAvatarSettingGameObjectToggle : CVRAdvancesAvatarSettingBase 
    {
        public bool defaultValue;

        #if UNITY_EDITOR
        
        public List<CVRAdvancedSettingsTargetEntryGameObject> gameObjectTargets =
            new List<CVRAdvancedSettingsTargetEntryGameObject>();

        private ReorderableList gameObjectList;
        private CVRAvatar target;
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };

            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameter = new AnimatorControllerParameter
            {
                name = machineName,
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue ? 1f : 0f
            };

            controller.AddParameter(animatorParameter);

            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTree = new BlendTree();
            blendTree.name = machineName + " Blend Tree";
            blendTree.blendParameter = machineName;

            var onClip = new AnimationClip();
            var offClip = new AnimationClip();
            
            var animationCurveOn = new AnimationCurve();
            var keyframe = new Keyframe(0f, 1);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOn.AddKey(keyframe);
            keyframe = new Keyframe(1f / 60f, 1);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOn.AddKey(keyframe);
            
            var animationCurveOff = new AnimationCurve();
            keyframe = new Keyframe(0f, 0);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOff.AddKey(keyframe);
            keyframe = new Keyframe(1f / 60f, 0);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOff.AddKey(keyframe);

            foreach (var target in gameObjectTargets)
            {
                if(target.gameObject == null || target.treePath == null) continue;
                
                onClip.SetCurve(target.treePath, typeof(GameObject), "m_IsActive", target.onState ? animationCurveOn : animationCurveOff);
                
                offClip.SetCurve(target.treePath, typeof(GameObject), "m_IsActive", !target.onState ? animationCurveOn : animationCurveOff);
            }
            
            blendTree.AddChild(offClip, 0f);
            AssetDatabase.CreateAsset(offClip, folderPath + "/Anim_" + machineName + "_Toggle_Off.anim");
            blendTree.AddChild(onClip, 1f);
            AssetDatabase.CreateAsset(onClip, folderPath + "/Anim_" + machineName + "_Toggle_On.anim");

            animatorState.motion = blendTree;
            
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));
            blendTree.hideFlags = HideFlags.HideInHierarchy;
        }

        private void generateReorderableList()
        {
            gameObjectList = new ReorderableList(gameObjectTargets, typeof(CVRAdvancedSettingsTargetEntryGameObject), 
                true, true, true, true);
            gameObjectList.drawHeaderCallback = OnDrawHeader;
            gameObjectList.drawElementCallback = OnDrawElement;
            gameObjectList.elementHeightCallback = OnHeightElement;
            gameObjectList.onAddCallback = OnAdd;
            gameObjectList.onChangedCallback = OnChanged;
        }
        
        public ReorderableList GetReorderableList(CVRAvatar avatar)
        {
            target = avatar;
            
            if (gameObjectList == null) generateReorderableList();

            return gameObjectList;
        }

        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }

        private void OnAdd(ReorderableList list)
        {
            gameObjectTargets.Add(new CVRAdvancedSettingsTargetEntryGameObject());
        }

        private float OnHeightElement(int index)
        {
            CVRAdvancedSettingsTargetEntryGameObject entity = gameObjectTargets[index];
            if (entity.isCollapsed) {
                return EditorGUIUtility.singleLineHeight * 1.25f;
            } else {
                return EditorGUIUtility.singleLineHeight * 3.75f;
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > gameObjectTargets.Count) return;
            CVRAdvancedSettingsTargetEntryGameObject entity = gameObjectTargets[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isCollapsed = !EditorGUI.Foldout(_rect, !entity.isCollapsed, "Name", true);
            _rect.x += 80;
            _rect.width = rect.width - 80;
            var targetGameObject = (GameObject) EditorGUI.ObjectField(_rect, entity.gameObject, typeof(GameObject), true);

            if (!entity.isCollapsed) {
                if (targetGameObject != null &&
                    targetGameObject.transform.GetComponentInParent(typeof(CVRAvatar)) == target) {
                    entity.gameObject = targetGameObject;
                    entity.treePath =
                        AnimationUtility.CalculateTransformPath(targetGameObject.transform, target.transform);
                }
                else if (entity.gameObject != targetGameObject) {
                    entity.gameObject = null;
                    entity.treePath = "";
                }

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(_rect, "Path");
                _rect.x += 80;
                _rect.width = rect.width - 80;
                EditorGUI.LabelField(_rect, entity.treePath);

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(_rect, new GUIContent("Set to",
                                                           "If checked, the object will be active once the toggle was pressed. If unchecked the object " +
                                                           "will be deactivated when the toggle is pressed."));
                _rect.x += 80;
                _rect.width = rect.width - 80;
                entity.onState = EditorGUI.Toggle(_rect, entity.onState);
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "GameObjects");
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingGameObjectDropdown : CVRAdvancesAvatarSettingBase 
    {
        public int defaultValue;
        public List<CVRAdvancedSettingsDropDownEntry> options = new List<CVRAdvancedSettingsDropDownEntry>();
        
        #if UNITY_EDITOR
        
        private ReorderableList gameObjectList;
        private CVRAvatar target;
        
        #endif

        public string[] getOptionsList()
        {
            var list = new string[options.Count];
            var i = 0;
            foreach (var option in options)
            {
                list[i] = option.name;
                i++;
            }

            return list;
        }
        
        #if UNITY_EDITOR
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameter = new AnimatorControllerParameter
            {
                name = machineName,
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue
            };

            controller.AddParameter(animatorParameter);

            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTree = new BlendTree();
            blendTree.name = machineName + " Blend Tree";
            blendTree.blendParameter = machineName;
            blendTree.useAutomaticThresholds = false;

            var animationCurveOn = new AnimationCurve();
            var keyframe = new Keyframe(0f, 1);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOn.AddKey(keyframe);
            keyframe = new Keyframe(1f / 60f, 1);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOn.AddKey(keyframe);
            
            var animationCurveOff = new AnimationCurve();
            keyframe = new Keyframe(0f, 0);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOff.AddKey(keyframe);
            keyframe = new Keyframe(1f / 60f, 0);
            keyframe.outTangent = Mathf.Infinity;
            animationCurveOff.AddKey(keyframe);

            var i = 0;
            AnimationClip animation;
            foreach (var option in options)
            {
                animation = new AnimationClip();
                var j = 0;
                foreach (var activeOption in options)
                {
                    foreach (var gameObjectTarget in activeOption.gameObjectList.gameObjectTargets)
                    {
                        if (gameObjectTarget == null || gameObjectTarget.gameObject == null || gameObjectTarget.treePath == null) continue;
                        animation.SetCurve(
                            gameObjectTarget.treePath, 
                            typeof(GameObject), 
                            "m_IsActive", 
                            i == j ? animationCurveOn : animationCurveOff
                        );
                    }
                    j++;
                }
                blendTree.AddChild(animation, i);
                AssetDatabase.CreateAsset(animation, folderPath + "/Anim_" + machineName + "_Dropdown_" + i + ".anim");
                i++;
            }

            animatorState.motion = blendTree;
            
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));
            blendTree.hideFlags = HideFlags.HideInHierarchy;
        }

        private void generateReorderableList()
        {
            gameObjectList = new ReorderableList(options, typeof(CVRAdvancedSettingsDropDownEntry), 
                true, true, true, true);
            gameObjectList.drawHeaderCallback = OnDrawHeader;
            gameObjectList.drawElementCallback = OnDrawElement;
            gameObjectList.elementHeightCallback = OnHeightElement;
            gameObjectList.onAddCallback = OnAdd;
            gameObjectList.onChangedCallback = OnChanged;
        }
        
        public ReorderableList GetReorderableList(CVRAvatar avatar)
        {
            target = avatar;
            
            if (gameObjectList == null) generateReorderableList();

            return gameObjectList;
        }

        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }

        private void OnAdd(ReorderableList list)
        {
            options.Add(new CVRAdvancedSettingsDropDownEntry());
        }

        private float OnHeightElement(int index)
        {
            CVRAdvancedSettingsDropDownEntry entity = options[index];
            if (entity.isCollapsed) {
                return EditorGUIUtility.singleLineHeight * 1.25f;
            } else {
                if (index > options.Count) return 0f;
                float height = 5.25f;
                foreach (var target in entity.gameObjectList.gameObjectTargets) {
                    if (target.isCollapsed) {
                        height += 1.25f;
                    } else {
                        height += 3.75f;
                    }
                }
                return EditorGUIUtility.singleLineHeight * height;
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > options.Count) return;
            CVRAdvancedSettingsDropDownEntry entity = options[index];
        
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isCollapsed = !EditorGUI.Foldout(_rect, !entity.isCollapsed, "Name", true);
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.name = EditorGUI.TextField(_rect, entity.name);

            if (!entity.isCollapsed) {
                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                var gameObjects = entity.GetReorderableList(target);
                gameObjects.DoList(new Rect(rect.x, rect.y, rect.width, 20f));
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Options");
        }
        
        #endif
    }

    [System.Serializable]
    public class CVRAdvancedAvatarSettingMaterialColor : CVRAdvancesAvatarSettingBase
    {
        public Color defaultValue = Color.white;

        #if UNITY_EDITOR
        
        public List<CVRAdvancedSettingsTargetEntryMaterialColor> materialColorTargets =
            new List<CVRAdvancedSettingsTargetEntryMaterialColor>();

        private CVRAdvancedSettingsTargetEntryMaterialColor entity;
        private ReorderableList gameObjectList;
        private CVRAvatar target;
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameterR = new AnimatorControllerParameter
            {
                name = machineName + "-r",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.r
            };

            controller.AddParameter(animatorParameterR);
            
            var animatorParameterG = new AnimatorControllerParameter
            {
                name = machineName + "-g",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.g
            };

            controller.AddParameter(animatorParameterG);
            
            var animatorParameterB = new AnimatorControllerParameter
            {
                name = machineName + "-b",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.b
            };

            controller.AddParameter(animatorParameterB);
            
            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTreeRed = new BlendTree();
            blendTreeRed.name = machineName + " Blend Tree Red";
            blendTreeRed.blendParameter = machineName + "-r";
            
            var blendTreeRedMin = new BlendTree();
            blendTreeRedMin.name = machineName + " Blend Tree Red Min";
            blendTreeRedMin.blendParameter = machineName + "-g";
            blendTreeRedMin.useAutomaticThresholds = false;
            
            var blendTreeRedMax = new BlendTree();
            blendTreeRedMax.name = machineName + " Blend Tree Red Max";
            blendTreeRedMax.blendParameter = machineName + "-g";
            blendTreeRedMax.useAutomaticThresholds = false;
            
            blendTreeRed.AddChild(blendTreeRedMin, 0f);
            blendTreeRed.AddChild(blendTreeRedMax, 1f);

            var blendTreeRedMinGreenMin = new BlendTree();
            blendTreeRedMinGreenMin.name = machineName + " Blend Tree Red Min Green Min";
            blendTreeRedMinGreenMin.blendParameter = machineName + "-b";
            blendTreeRedMinGreenMin.useAutomaticThresholds = false;
            
            var blendTreeRedMinGreenMax = new BlendTree();
            blendTreeRedMinGreenMax.name = machineName + " Blend Tree Red Min Green Max";
            blendTreeRedMinGreenMax.blendParameter = machineName + "-b";
            blendTreeRedMinGreenMax.useAutomaticThresholds = false;
            
            blendTreeRedMin.AddChild(blendTreeRedMinGreenMin, 0f);
            blendTreeRedMin.AddChild(blendTreeRedMinGreenMax, 1f);
            
            var blendTreeRedMaxGreenMin = new BlendTree();
            blendTreeRedMaxGreenMin.name = machineName + " Blend Tree Red MaRed Green Min";
            blendTreeRedMaxGreenMin.blendParameter = machineName + "-b";
            blendTreeRedMaxGreenMin.useAutomaticThresholds = false;
            
            var blendTreeRedMaxGreenMax = new BlendTree();
            blendTreeRedMaxGreenMax.name = machineName + " Blend Tree Red MaRed Green Max";
            blendTreeRedMaxGreenMax.blendParameter = machineName + "-b";
            blendTreeRedMaxGreenMax.useAutomaticThresholds = false;
            
            blendTreeRedMax.AddChild(blendTreeRedMaxGreenMin, 0f);
            blendTreeRedMax.AddChild(blendTreeRedMaxGreenMax, 1f);
            
            var clipR0G0B0 = new AnimationClip();
            var clipR0G0B1 = new AnimationClip();
            var clipR0G1B0 = new AnimationClip();
            var clipR0G1B1 = new AnimationClip();
            var clipR1G0B0 = new AnimationClip();
            var clipR1G0B1 = new AnimationClip();
            var clipR1G1B0 = new AnimationClip();
            var clipR1G1B1 = new AnimationClip();
            
            AnimationCurve animationCurve0 = new AnimationCurve();
            animationCurve0.AddKey(0, 0);
            animationCurve0.AddKey(1f/60, 0);
            AnimationCurve animationCurve1 = new AnimationCurve();
            animationCurve1.AddKey(0, 1);
            animationCurve1.AddKey(1f/60, 1);

            foreach (var target in materialColorTargets)
            {
                if(target.gameObject == null || target.propertyName == "" || target.treePath == null) continue;
                
                clipR0G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve0);
                clipR0G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve0);
                clipR0G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve0);
                clipR0G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve0);
                clipR1G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve1);
                clipR1G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve1);
                clipR1G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve1);
                clipR1G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".r", animationCurve1);
                
                clipR0G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve0);
                clipR0G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve0);
                clipR0G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve1);
                clipR0G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve1);
                clipR1G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve0);
                clipR1G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve0);
                clipR1G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve1);
                clipR1G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".g", animationCurve1);
                
                clipR0G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve0);
                clipR0G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve1);
                clipR0G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve0);
                clipR0G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve1);
                clipR1G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve0);
                clipR1G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve1);
                clipR1G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve0);
                clipR1G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".b", animationCurve1);
                
                clipR0G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR0G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR0G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR0G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR1G0B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR1G0B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR1G1B0.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
                clipR1G1B1.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName + ".a", animationCurve1);
            }
            
            blendTreeRedMinGreenMin.AddChild(clipR0G0B0, 0);
            AssetDatabase.CreateAsset(clipR0G0B0, folderPath + "/Anim_" + machineName + "_Color_R0G0B0.anim");
            blendTreeRedMinGreenMin.AddChild(clipR0G0B1, 1);
            AssetDatabase.CreateAsset(clipR0G0B1, folderPath + "/Anim_" + machineName + "_Color_R0G0B1.anim");
            
            blendTreeRedMinGreenMax.AddChild(clipR0G1B0, 0);
            AssetDatabase.CreateAsset(clipR0G1B0, folderPath + "/Anim_" + machineName + "_Color_R0G1B0.anim");
            blendTreeRedMinGreenMax.AddChild(clipR0G1B1, 1);
            AssetDatabase.CreateAsset(clipR0G1B1, folderPath + "/Anim_" + machineName + "_Color_R0G1B1.anim");
            
            blendTreeRedMaxGreenMin.AddChild(clipR1G0B0, 0);
            AssetDatabase.CreateAsset(clipR1G0B0, folderPath + "/Anim_" + machineName + "_Color_R1G0B0.anim");
            blendTreeRedMaxGreenMin.AddChild(clipR1G0B1, 1);
            AssetDatabase.CreateAsset(clipR1G0B1, folderPath + "/Anim_" + machineName + "_Color_R1G0B1.anim");
            
            blendTreeRedMaxGreenMax.AddChild(clipR1G1B0, 0);
            AssetDatabase.CreateAsset(clipR1G1B0, folderPath + "/Anim_" + machineName + "_Color_R1G1B0.anim");
            blendTreeRedMaxGreenMax.AddChild(clipR1G1B1, 1);
            AssetDatabase.CreateAsset(clipR1G1B1, folderPath + "/Anim_" + machineName + "_Color_R1G1B1.anim");

            animatorState.motion = blendTreeRed;
            
            AssetDatabase.AddObjectToAsset(blendTreeRed, AssetDatabase.GetAssetPath(controller));
            blendTreeRed.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeRedMin, AssetDatabase.GetAssetPath(controller));
            blendTreeRedMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeRedMax, AssetDatabase.GetAssetPath(controller));
            blendTreeRedMax.hideFlags = HideFlags.HideInHierarchy;
            
            AssetDatabase.AddObjectToAsset(blendTreeRedMinGreenMin, AssetDatabase.GetAssetPath(controller));
            blendTreeRedMinGreenMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeRedMinGreenMax, AssetDatabase.GetAssetPath(controller));
            blendTreeRedMinGreenMax.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeRedMaxGreenMin, AssetDatabase.GetAssetPath(controller));
            blendTreeRedMaxGreenMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeRedMaxGreenMax, AssetDatabase.GetAssetPath(controller));
            blendTreeRedMaxGreenMax.hideFlags = HideFlags.HideInHierarchy;
        }

        private void generateReorderableList()
        {
            gameObjectList = new ReorderableList(materialColorTargets, typeof(CVRAdvancedSettingsTargetEntryMaterialColor), 
                true, true, true, true);
            gameObjectList.drawHeaderCallback = OnDrawHeader;
            gameObjectList.drawElementCallback = OnDrawElement;
            gameObjectList.elementHeightCallback = OnHeightElement;
            gameObjectList.onAddCallback = OnAdd;
            gameObjectList.onChangedCallback = OnChanged;
        }
        
        public ReorderableList GetReorderableList(CVRAvatar avatar)
        {
            target = avatar;
            
            if (gameObjectList == null) generateReorderableList();

            return gameObjectList;
        }

        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }

        private void OnAdd(ReorderableList list)
        {
            materialColorTargets.Add(new CVRAdvancedSettingsTargetEntryMaterialColor());
        }

        private float OnHeightElement(int index)
        {
            if (isCollapsed) {
                return EditorGUIUtility.singleLineHeight * 1.25f;
            } else {
                return EditorGUIUtility.singleLineHeight * 3.75f;
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > materialColorTargets.Count) return;
            entity = materialColorTargets[index];
            
            rect.y += 2;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Game Object");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            
            var propertyList = new Dictionary<string, string>();
            var targetGameObject = (GameObject) EditorGUI.ObjectField(_rect, entity.gameObject, typeof(GameObject), true);
            
            if (targetGameObject != null && targetGameObject.transform.GetComponentInParent(typeof(CVRAvatar)) == target)
            {
                var meshRenderer = (MeshRenderer) targetGameObject.GetComponent(typeof(MeshRenderer));
                var skinnedMeshRenderer = (SkinnedMeshRenderer) targetGameObject.GetComponent(typeof(SkinnedMeshRenderer));
                var particleRenderer = (ParticleSystemRenderer) targetGameObject.GetComponent(typeof(ParticleSystemRenderer));
                var lineRenderer = (LineRenderer) targetGameObject.GetComponent(typeof(LineRenderer));
                var trailRenderer = (TrailRenderer) targetGameObject.GetComponent(typeof(TrailRenderer));
                var rendererFound = false;

                if (meshRenderer != null)
                {
                    for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                    {
                        var material = meshRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Color)
                            {
                                var propertyKey = "MeshRenderer: " + shader.GetPropertyDescription(j) + "(" + shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "MSR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }

                if (skinnedMeshRenderer != null)
                {
                    for (var i = 0; i < skinnedMeshRenderer.sharedMaterials.Length; i++)
                    {
                        var material = skinnedMeshRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Color)
                            {
                                var propertyKey = "SkinnedMeshRenderer: " + shader.GetPropertyDescription(j) + "(" + shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "SMR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }
                
                if (particleRenderer != null)
                {
                    for (var i = 0; i < particleRenderer.sharedMaterials.Length; i++)
                    {
                        var material = particleRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Color)
                            {
                                var propertyKey = "ParticleRenderer: " + shader.GetPropertyDescription(j) + "(" +
                                                  shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "PTR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }
                
                if (lineRenderer != null)
                {
                    for (var i = 0; i < lineRenderer.sharedMaterials.Length; i++)
                    {
                        var material = lineRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Color)
                            {
                                var propertyKey = "LineRenderer: " + shader.GetPropertyDescription(j) + "(" +
                                                  shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "LNR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }
                
                if (trailRenderer != null)
                {
                    for (var i = 0; i < trailRenderer.sharedMaterials.Length; i++)
                    {
                        var material = trailRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Color)
                            {
                                var propertyKey = "TrailRenderer: " + shader.GetPropertyDescription(j) + "(" +
                                                  shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "TLR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }

                if (rendererFound)
                {
                    entity.gameObject = targetGameObject;
                    entity.treePath = AnimationUtility.CalculateTransformPath(targetGameObject.transform, target.transform);
                }
                else if (entity.gameObject != targetGameObject)
                {
                    entity.gameObject = null;
                    entity.treePath = "";
                }
            }
            else if (entity.gameObject != targetGameObject)
            {
                entity.gameObject = null;
                entity.treePath = "";
            }

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Path");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            EditorGUI.LabelField(_rect, entity.treePath);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            var propertyNames = new string[propertyList.Count];
            propertyList.Values.CopyTo(propertyNames, 0);
            var propertyDescriptions = new string[propertyList.Count];
            propertyList.Keys.CopyTo(propertyDescriptions, 0);
            
            EditorGUI.LabelField(_rect, "Property");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            var propertyIndex = EditorGUI.Popup(_rect, Array.IndexOf(propertyNames, entity.propertyTypeIdentifier + ":" + entity.propertyName), propertyDescriptions);
            if (propertyIndex >= 0)
            {
                var property = propertyNames[propertyIndex];
                entity.propertyName = property.Substring(4);
                entity.propertyTypeIdentifier = property.Substring(0, 3);
                switch (entity.propertyTypeIdentifier)
                {
                    case "SMR":
                        entity.propertyType = typeof(SkinnedMeshRenderer);
                        break;
                    case "MSR":
                        entity.propertyType = typeof(MeshRenderer);
                        break;
                    case "PTR":
                        entity.propertyType = typeof(ParticleSystemRenderer);
                        break;
                    case "LNR":
                        entity.propertyType = typeof(LineRenderer);
                        break;
                    case "TLR":
                        entity.propertyType = typeof(TrailRenderer);
                        break;
                    default:
                        entity.propertyType = typeof(SkinnedMeshRenderer);
                        break;
                }
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Material Properties");
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingSlider : CVRAdvancesAvatarSettingBase 
    {
        public float defaultValue;
        
        public List<CVRAdvancedSettingsTargetEntryMaterialProperty> materialPropertyTargets =
            new List<CVRAdvancedSettingsTargetEntryMaterialProperty>();
        
        private CVRAdvancedSettingsTargetEntryMaterialProperty entity;
#if UNITY_EDITOR
        private ReorderableList gameObjectList;
        private CVRAvatar target;
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameter = new AnimatorControllerParameter
            {
                name = machineName,
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue
            };

            controller.AddParameter(animatorParameter);

            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTree = new BlendTree();
            blendTree.name = machineName + " Blend Tree";
            blendTree.blendParameter = machineName;

            var minClip = new AnimationClip();
            var maxClip = new AnimationClip();
            
            AnimationCurve animationCurve0 = new AnimationCurve();
            animationCurve0.AddKey(0, 0);
            animationCurve0.AddKey(1f/60, 0);
            AnimationCurve animationCurve1 = new AnimationCurve();
            animationCurve1.AddKey(0, 1);
            animationCurve1.AddKey(1f/60, 1);
            
            foreach (var target in materialPropertyTargets)
            {
                if(target.gameObject == null || target.propertyName == "" || target.treePath == null) continue;
                
                minClip.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName, animationCurve0);
                maxClip.SetCurve(target.treePath, target.propertyType, "material." + target.propertyName, animationCurve1);
            }

            blendTree.AddChild(minClip, 0f);
            AssetDatabase.CreateAsset(minClip, folderPath + "/Anim_" + machineName + "_Slider_Min.anim");
            blendTree.AddChild(maxClip, 1f);
            AssetDatabase.CreateAsset(maxClip, folderPath + "/Anim_" + machineName + "_Slider_Max.anim");

            animatorState.motion = blendTree;
            
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));
            blendTree.hideFlags = HideFlags.HideInHierarchy;
        }
        
        private void generateReorderableList()
        {
            gameObjectList = new ReorderableList(materialPropertyTargets, typeof(CVRAdvancedSettingsTargetEntryMaterialProperty), 
                true, true, true, true);
            gameObjectList.drawHeaderCallback = OnDrawHeader;
            gameObjectList.drawElementCallback = OnDrawElement;
            gameObjectList.elementHeightCallback = OnHeightElement;
            gameObjectList.onAddCallback = OnAdd;
            gameObjectList.onChangedCallback = OnChanged;
        }
        
        public ReorderableList GetReorderableList(CVRAvatar avatar)
        {
            target = avatar;
            
            if (gameObjectList == null) generateReorderableList();

            return gameObjectList;
        }

        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }

        private void OnAdd(ReorderableList list)
        {
            materialPropertyTargets.Add(new CVRAdvancedSettingsTargetEntryMaterialProperty());
        }

        private float OnHeightElement(int index)
        {
            return EditorGUIUtility.singleLineHeight * 6.25f;
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > materialPropertyTargets.Count) return;
            entity = materialPropertyTargets[index];
            
            rect.y += 2;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Game Object");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            
            var propertyList = new Dictionary<string, string>();
            var targetGameObject = (GameObject) EditorGUI.ObjectField(_rect, entity.gameObject, typeof(GameObject), true);
            
            if (targetGameObject != null && targetGameObject.transform.GetComponentInParent(typeof(CVRAvatar)) == target)
            {
                var meshRenderer = (MeshRenderer) targetGameObject.GetComponent(typeof(MeshRenderer));
                var skinnedMeshRenderer = (SkinnedMeshRenderer) targetGameObject.GetComponent(typeof(SkinnedMeshRenderer));
                var particleRenderer = (ParticleSystemRenderer) targetGameObject.GetComponent(typeof(ParticleSystemRenderer));
                var lineRenderer = (LineRenderer) targetGameObject.GetComponent(typeof(LineRenderer));
                var trailRenderer = (TrailRenderer) targetGameObject.GetComponent(typeof(TrailRenderer));
                var rendererFound = false;

                if (meshRenderer != null)
                {
                    for (var i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                    {
                        var material = meshRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Float || shader.GetPropertyType(j) == ShaderPropertyType.Range)
                            {
                                var propertyKey = "MeshRenderer: " + shader.GetPropertyDescription(j) + "(" + shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "MSR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }

                if (skinnedMeshRenderer != null)
                {
                    for (var i = 0; i < skinnedMeshRenderer.sharedMaterials.Length; i++)
                    {
                        var material = skinnedMeshRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Float || shader.GetPropertyType(j) == ShaderPropertyType.Range)
                            {
                                var propertyKey = "SkinnedMeshRenderer: " + shader.GetPropertyDescription(j) + "(" + shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "SMR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }
                
                if (particleRenderer != null)
                {
                    for (var i = 0; i < particleRenderer.sharedMaterials.Length; i++)
                    {
                        var material = particleRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Float || shader.GetPropertyType(j) == ShaderPropertyType.Range)
                            {
                                var propertyKey = "ParticleRenderer: " + shader.GetPropertyDescription(j) + "(" +
                                                  shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "PTR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }
                
                if (lineRenderer != null)
                {
                    for (var i = 0; i < lineRenderer.sharedMaterials.Length; i++)
                    {
                        var material = lineRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Float || shader.GetPropertyType(j) == ShaderPropertyType.Range)
                            {
                                var propertyKey = "LineRenderer: " + shader.GetPropertyDescription(j) + "(" +
                                                  shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "LNR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }
                
                if (trailRenderer != null)
                {
                    for (var i = 0; i < trailRenderer.sharedMaterials.Length; i++)
                    {
                        var material = trailRenderer.sharedMaterials[i];
                        if (material == null) continue;
                        var shader = material.shader;
                        for (var j = 0; j < shader.GetPropertyCount(); j++)
                        {
                            if (shader.GetPropertyType(j) == ShaderPropertyType.Float || shader.GetPropertyType(j) == ShaderPropertyType.Range)
                            {
                                var propertyKey = "TrailRenderer: " + shader.GetPropertyDescription(j) + "(" +
                                                  shader.GetPropertyName(j) + ")";
                                if (!propertyList.ContainsKey(propertyKey))
                                {
                                    propertyList.Add(
                                        propertyKey,
                                        "TLR:" + shader.GetPropertyName(j)
                                    );
                                }
                            }
                        }
                    }

                    rendererFound = true;
                }

                if (rendererFound)
                {
                    entity.gameObject = targetGameObject;
                    entity.treePath = AnimationUtility.CalculateTransformPath(targetGameObject.transform, target.transform);
                }
                else if (entity.gameObject != targetGameObject)
                {
                    entity.gameObject = null;
                    entity.treePath = "";
                }
            }
            else if (entity.gameObject != targetGameObject)
            {
                entity.gameObject = null;
                entity.treePath = "";
            }

            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(_rect, "Path");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            EditorGUI.LabelField(_rect, entity.treePath);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            var propertyNames = new string[propertyList.Count];
            propertyList.Values.CopyTo(propertyNames, 0);
            var propertyDescriptions = new string[propertyList.Count];
            propertyList.Keys.CopyTo(propertyDescriptions, 0);
            
            EditorGUI.LabelField(_rect, "Property");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            var propertyIndex = EditorGUI.Popup(_rect, Array.IndexOf(propertyNames, entity.propertyTypeIdentifier + ":" + entity.propertyName), propertyDescriptions);
            if (propertyIndex >= 0)
            {
                var property = propertyNames[propertyIndex];
                entity.propertyName = property.Substring(4);
                entity.propertyTypeIdentifier = property.Substring(0, 3);
                switch (entity.propertyTypeIdentifier)
                {
                    case "SMR":
                        entity.propertyType = typeof(SkinnedMeshRenderer);
                        break;
                    case "MSR":
                        entity.propertyType = typeof(MeshRenderer);
                        break;
                    case "PTR":
                        entity.propertyType = typeof(ParticleSystemRenderer);
                        break;
                    case "LNR":
                        entity.propertyType = typeof(LineRenderer);
                        break;
                    case "TLR":
                        entity.propertyType = typeof(TrailRenderer);
                        break;
                    default:
                        entity.propertyType = typeof(SkinnedMeshRenderer);
                        break;
                }
            }
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Min Value");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.minValue = EditorGUI.FloatField(_rect, entity.minValue);
            
            rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
            _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(_rect, "Max Value");
            _rect.x += 80;
            _rect.width = rect.width - 80;
            entity.maxValue = EditorGUI.FloatField(_rect, entity.maxValue);
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "Material Properties");
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingJoystick2D : CVRAdvancesAvatarSettingBase 
    {
        public Vector2 defaultValue = Vector2.zero;

        #if UNITY_EDITOR
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameterX = new AnimatorControllerParameter
            {
                name = machineName + "-x",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.x
            };

            controller.AddParameter(animatorParameterX);
            
            var animatorParameterY = new AnimatorControllerParameter
            {
                name = machineName + "-y",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.y
            };

            controller.AddParameter(animatorParameterY);

            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTree = new BlendTree();
            blendTree.name = machineName + " Blend Tree";
            blendTree.blendParameter = machineName + "-x";
            blendTree.useAutomaticThresholds = false;
            
            var blendTreeXMin = new BlendTree();
            blendTreeXMin.name = machineName + " Blend Tree X Min";
            blendTreeXMin.blendParameter = machineName + "-y";
            blendTreeXMin.useAutomaticThresholds = false;
            
            var blendTreeXMax = new BlendTree();
            blendTreeXMax.name = machineName + " Blend Tree X Max";
            blendTreeXMax.blendParameter = machineName + "-y";
            blendTreeXMax.useAutomaticThresholds = false;
            
            blendTree.AddChild(blendTreeXMin, 0f);
            blendTree.AddChild(blendTreeXMax, 1f);

            var X0Y0Clip = new AnimationClip();
            var X0Y1Clip = new AnimationClip();
            var X1Y0Clip = new AnimationClip();
            var X1Y1Clip = new AnimationClip();

            blendTreeXMin.AddChild(X0Y0Clip, 0f);
            AssetDatabase.CreateAsset(X0Y0Clip, folderPath + "/Anim_" + machineName + "_Joystick2D_X0Y0.anim");
            blendTreeXMin.AddChild(X0Y1Clip, 1f);
            AssetDatabase.CreateAsset(X0Y1Clip, folderPath + "/Anim_" + machineName + "_Joystick2D_X0Y1.anim");
            
            blendTreeXMax.AddChild(X1Y0Clip, 0f);
            AssetDatabase.CreateAsset(X1Y0Clip, folderPath + "/Anim_" + machineName + "_Joystick2D_X1Y0.anim");
            blendTreeXMax.AddChild(X1Y1Clip, 1f);
            AssetDatabase.CreateAsset(X1Y1Clip, folderPath + "/Anim_" + machineName + "_Joystick2D_X1Y1.anim");

            animatorState.motion = blendTree;
            
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));
            blendTree.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMax.hideFlags = HideFlags.HideInHierarchy;
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingJoystick3D : CVRAdvancesAvatarSettingBase
    {
        public Vector3 defaultValue = Vector3.zero;

        #if UNITY_EDITOR
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameterX = new AnimatorControllerParameter
            {
                name = machineName + "-x",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.x
            };

            controller.AddParameter(animatorParameterX);
            
            var animatorParameterY = new AnimatorControllerParameter
            {
                name = machineName + "-y",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.y
            };

            controller.AddParameter(animatorParameterY);
            
            var animatorParameterZ = new AnimatorControllerParameter
            {
                name = machineName + "-z",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.z
            };

            controller.AddParameter(animatorParameterZ);
            
            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTreeX = new BlendTree();
            blendTreeX.name = machineName + " Blend Tree x";
            blendTreeX.blendParameter = machineName + "-x";
            blendTreeX.useAutomaticThresholds = false;
            
            var blendTreeXMin = new BlendTree();
            blendTreeXMin.name = machineName + " Blend Tree X Min";
            blendTreeXMin.blendParameter = machineName + "-y";
            blendTreeXMin.useAutomaticThresholds = false;
            
            var blendTreeXMax = new BlendTree();
            blendTreeXMax.name = machineName + " Blend Tree X Max";
            blendTreeXMax.blendParameter = machineName + "-y";
            blendTreeXMax.useAutomaticThresholds = false;
            
            blendTreeX.AddChild(blendTreeXMin, 0f);
            blendTreeX.AddChild(blendTreeXMax, 1f);

            var blendTreeXMinYMin = new BlendTree();
            blendTreeXMinYMin.name = machineName + " Blend Tree X Min Y Min";
            blendTreeXMinYMin.blendParameter = machineName + "-z";
            blendTreeXMinYMin.useAutomaticThresholds = false;
            
            var blendTreeXMinYMax = new BlendTree();
            blendTreeXMinYMax.name = machineName + " Blend Tree X Min Y Max";
            blendTreeXMinYMax.blendParameter = machineName + "-z";
            blendTreeXMinYMax.useAutomaticThresholds = false;
            
            blendTreeXMin.AddChild(blendTreeXMinYMin, 0f);
            blendTreeXMin.AddChild(blendTreeXMinYMax, 1f);
            
            var blendTreeXMaxYMin = new BlendTree();
            blendTreeXMaxYMin.name = machineName + " Blend Tree X Max Y Min";
            blendTreeXMaxYMin.blendParameter = machineName + "-z";
            blendTreeXMaxYMin.useAutomaticThresholds = false;
            
            var blendTreeXMaxYMax = new BlendTree();
            blendTreeXMaxYMax.name = machineName + " Blend Tree X Max Y Max";
            blendTreeXMaxYMax.blendParameter = machineName + "-z";
            blendTreeXMaxYMax.useAutomaticThresholds = false;
            
            blendTreeXMax.AddChild(blendTreeXMaxYMin, 0f);
            blendTreeXMax.AddChild(blendTreeXMaxYMax, 1f);

            var clipX0Y0Z0 = new AnimationClip();
            var clipX0Y0Z1 = new AnimationClip();
            var clipX0Y1Z0 = new AnimationClip();
            var clipX0Y1Z1 = new AnimationClip();
            var clipX1Y0Z0 = new AnimationClip();
            var clipX1Y0Z1 = new AnimationClip();
            var clipX1Y1Z0 = new AnimationClip();
            var clipX1Y1Z1 = new AnimationClip();

            blendTreeXMinYMin.AddChild(clipX0Y0Z0, 0f);
            AssetDatabase.CreateAsset(clipX0Y0Z0, folderPath + "/Anim_" + machineName + "_Joystick3D_X0Y0Z0.anim");
            blendTreeXMinYMin.AddChild(clipX0Y0Z1, 1f);
            AssetDatabase.CreateAsset(clipX0Y0Z1, folderPath + "/Anim_" + machineName + "_Joystick3D_X0Y0Z1.anim");
            
            blendTreeXMinYMax.AddChild(clipX0Y1Z0, 0f);
            AssetDatabase.CreateAsset(clipX0Y1Z0, folderPath + "/Anim_" + machineName + "_Joystick3D_X0Y1Z0.anim");
            blendTreeXMinYMax.AddChild(clipX0Y1Z1, 1f);
            AssetDatabase.CreateAsset(clipX0Y1Z1, folderPath + "/Anim_" + machineName + "_Joystick3D_X0Y1Z1.anim");
            
            blendTreeXMaxYMin.AddChild(clipX1Y0Z0, 0f);
            AssetDatabase.CreateAsset(clipX1Y0Z0, folderPath + "/Anim_" + machineName + "_Joystick3D_X1Y0Z0.anim");
            blendTreeXMaxYMin.AddChild(clipX1Y0Z1, 1f);
            AssetDatabase.CreateAsset(clipX1Y0Z1, folderPath + "/Anim_" + machineName + "_Joystick3D_X1Y0Z1.anim");
            
            blendTreeXMaxYMax.AddChild(clipX1Y1Z0, 0f);
            AssetDatabase.CreateAsset(clipX1Y1Z0, folderPath + "/Anim_" + machineName + "_Joystick3D_X1Y1Z0.anim");
            blendTreeXMaxYMax.AddChild(clipX1Y1Z1, 1f);
            AssetDatabase.CreateAsset(clipX1Y1Z1, folderPath + "/Anim_" + machineName + "_Joystick3D_X1Y1Z1.anim");

            animatorState.motion = blendTreeX;
            
            AssetDatabase.AddObjectToAsset(blendTreeX, AssetDatabase.GetAssetPath(controller));
            blendTreeX.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMax.hideFlags = HideFlags.HideInHierarchy;
            
            AssetDatabase.AddObjectToAsset(blendTreeXMinYMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMinYMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMinYMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMinYMax.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMaxYMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMaxYMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMaxYMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMaxYMax.hideFlags = HideFlags.HideInHierarchy;
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingInputSingle : CVRAdvancesAvatarSettingBase 
    {
        public float defaultValue;

        #if UNITY_EDITOR
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameter = new AnimatorControllerParameter
            {
                name = machineName,
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue
            };

            controller.AddParameter(animatorParameter);

            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTree = new BlendTree();
            blendTree.name = machineName + " Blend Tree";
            blendTree.blendParameter = machineName;
            blendTree.useAutomaticThresholds = false;

            var minClip = new AnimationClip();
            var maxClip = new AnimationClip();

            blendTree.AddChild(minClip, -9999f);
            AssetDatabase.CreateAsset(minClip, folderPath + "/Anim_" + machineName + "_InputSingle_Min.anim");
            blendTree.AddChild(maxClip, 9999f);
            AssetDatabase.CreateAsset(maxClip, folderPath + "/Anim_" + machineName + "_InputSingle_Max.anim");

            animatorState.motion = blendTree;
            
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));
            blendTree.hideFlags = HideFlags.HideInHierarchy;
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingInputVector2 : CVRAdvancesAvatarSettingBase 
    {
        public Vector2 defaultValue = Vector2.zero;

        #if UNITY_EDITOR
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameterX = new AnimatorControllerParameter
            {
                name = machineName + "-x",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.x
            };

            controller.AddParameter(animatorParameterX);
            
            var animatorParameterY = new AnimatorControllerParameter
            {
                name = machineName + "-y",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.y
            };

            controller.AddParameter(animatorParameterY);

            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTree = new BlendTree();
            blendTree.name = machineName + " Blend Tree";
            blendTree.blendParameter = machineName + "-x";
            blendTree.useAutomaticThresholds = false;
            
            var blendTreeXMin = new BlendTree();
            blendTreeXMin.name = machineName + " Blend Tree X Min";
            blendTreeXMin.blendParameter = machineName + "-y";
            blendTreeXMin.useAutomaticThresholds = false;
            
            var blendTreeXMax = new BlendTree();
            blendTreeXMax.name = machineName + " Blend Tree X Max";
            blendTreeXMax.blendParameter = machineName + "-y";
            blendTreeXMax.useAutomaticThresholds = false;
            
            blendTree.AddChild(blendTreeXMin, -9999f);
            blendTree.AddChild(blendTreeXMax, 9999f);

            var X0Y0Clip = new AnimationClip();
            var X0Y1Clip = new AnimationClip();
            var X1Y0Clip = new AnimationClip();
            var X1Y1Clip = new AnimationClip();

            blendTreeXMin.AddChild(X0Y0Clip, -9999f);
            AssetDatabase.CreateAsset(X0Y0Clip, folderPath + "/Anim_" + machineName + "_InputVector2_X0Y0.anim");
            blendTreeXMin.AddChild(X0Y1Clip, 9999f);
            AssetDatabase.CreateAsset(X0Y1Clip, folderPath + "/Anim_" + machineName + "_InputVector2_X0Y1.anim");
            
            blendTreeXMax.AddChild(X1Y0Clip, -9999f);
            AssetDatabase.CreateAsset(X1Y0Clip, folderPath + "/Anim_" + machineName + "_InputVector2_X1Y0.anim");
            blendTreeXMax.AddChild(X1Y1Clip, 9999f);
            AssetDatabase.CreateAsset(X1Y1Clip, folderPath + "/Anim_" + machineName + "_InputVector2_X1Y1.anim");

            animatorState.motion = blendTree;
            
            AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));
            blendTree.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMax.hideFlags = HideFlags.HideInHierarchy;
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancesAvatarSettingInputVector3 : CVRAdvancesAvatarSettingBase
    {
        public Vector3 defaultValue = Vector3.zero;

        #if UNITY_EDITOR
        
        public override void SetupAnimator(ref AnimatorController controller, string machineName, string folderPath)
        {
            var animatorLayer = new UnityEditor.Animations.AnimatorControllerLayer
            {
                name = machineName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine()
            };
            
            animatorLayer.stateMachine.name = machineName;
            AssetDatabase.AddObjectToAsset(animatorLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
            animatorLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            
            controller.AddLayer(animatorLayer);

            var animatorParameterX = new AnimatorControllerParameter
            {
                name = machineName + "-x",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.x
            };

            controller.AddParameter(animatorParameterX);
            
            var animatorParameterY = new AnimatorControllerParameter
            {
                name = machineName + "-y",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.y
            };

            controller.AddParameter(animatorParameterY);
            
            var animatorParameterZ = new AnimatorControllerParameter
            {
                name = machineName + "-z",
                type = AnimatorControllerParameterType.Float,
                defaultFloat = defaultValue.z
            };

            controller.AddParameter(animatorParameterZ);
            
            var animatorState = animatorLayer.stateMachine.AddState(machineName + " Blend Tree");
            
            var blendTreeX = new BlendTree();
            blendTreeX.name = machineName + " Blend Tree x";
            blendTreeX.blendParameter = machineName + "-x";
            blendTreeX.useAutomaticThresholds = false;
            
            var blendTreeXMin = new BlendTree();
            blendTreeXMin.name = machineName + " Blend Tree X Min";
            blendTreeXMin.blendParameter = machineName + "-y";
            blendTreeXMin.useAutomaticThresholds = false;
            
            var blendTreeXMax = new BlendTree();
            blendTreeXMax.name = machineName + " Blend Tree X Max";
            blendTreeXMax.blendParameter = machineName + "-y";
            blendTreeXMax.useAutomaticThresholds = false;
            
            blendTreeX.AddChild(blendTreeXMin, -9999f);
            blendTreeX.AddChild(blendTreeXMax, 9999f);

            var blendTreeXMinYMin = new BlendTree();
            blendTreeXMinYMin.name = machineName + " Blend Tree X Min Y Min";
            blendTreeXMinYMin.blendParameter = machineName + "-z";
            blendTreeXMinYMin.useAutomaticThresholds = false;
            
            var blendTreeXMinYMax = new BlendTree();
            blendTreeXMinYMax.name = machineName + " Blend Tree X Min Y Max";
            blendTreeXMinYMax.blendParameter = machineName + "-z";
            blendTreeXMinYMax.useAutomaticThresholds = false;
            
            blendTreeXMin.AddChild(blendTreeXMinYMin, -9999f);
            blendTreeXMin.AddChild(blendTreeXMinYMax, 9999f);
            
            var blendTreeXMaxYMin = new BlendTree();
            blendTreeXMaxYMin.name = machineName + " Blend Tree X Max Y Min";
            blendTreeXMaxYMin.blendParameter = machineName + "-z";
            blendTreeXMaxYMin.useAutomaticThresholds = false;
            
            var blendTreeXMaxYMax = new BlendTree();
            blendTreeXMaxYMax.name = machineName + " Blend Tree X Max Y Max";
            blendTreeXMaxYMax.blendParameter = machineName + "-z";
            blendTreeXMaxYMax.useAutomaticThresholds = false;
            
            blendTreeXMax.AddChild(blendTreeXMaxYMin, -9999f);
            blendTreeXMax.AddChild(blendTreeXMaxYMax, 9999f);

            var clipX0Y0Z0 = new AnimationClip();
            var clipX0Y0Z1 = new AnimationClip();
            var clipX0Y1Z0 = new AnimationClip();
            var clipX0Y1Z1 = new AnimationClip();
            var clipX1Y0Z0 = new AnimationClip();
            var clipX1Y0Z1 = new AnimationClip();
            var clipX1Y1Z0 = new AnimationClip();
            var clipX1Y1Z1 = new AnimationClip();

            blendTreeXMinYMin.AddChild(clipX0Y0Z0, -9999f);
            AssetDatabase.CreateAsset(clipX0Y0Z0, folderPath + "/Anim_" + machineName + "_InputVector3_X0Y0Z0.anim");
            blendTreeXMinYMin.AddChild(clipX0Y0Z1, 9999f);
            AssetDatabase.CreateAsset(clipX0Y0Z1, folderPath + "/Anim_" + machineName + "_InputVector3_X0Y0Z1.anim");
            
            blendTreeXMinYMax.AddChild(clipX0Y1Z0, -9999f);
            AssetDatabase.CreateAsset(clipX0Y1Z0, folderPath + "/Anim_" + machineName + "_InputVector3_X0Y1Z0.anim");
            blendTreeXMinYMax.AddChild(clipX0Y1Z1, 9999f);
            AssetDatabase.CreateAsset(clipX0Y1Z1, folderPath + "/Anim_" + machineName + "_InputVector3_X0Y1Z1.anim");
            
            blendTreeXMaxYMin.AddChild(clipX1Y0Z0, -9999f);
            AssetDatabase.CreateAsset(clipX1Y0Z0, folderPath + "/Anim_" + machineName + "_InputVector3_X1Y0Z0.anim");
            blendTreeXMaxYMin.AddChild(clipX1Y0Z1, 9999f);
            AssetDatabase.CreateAsset(clipX1Y0Z1, folderPath + "/Anim_" + machineName + "_InputVector3_X1Y0Z1.anim");
            
            blendTreeXMaxYMax.AddChild(clipX1Y1Z0, -9999f);
            AssetDatabase.CreateAsset(clipX1Y1Z0, folderPath + "/Anim_" + machineName + "_InputVector3_X1Y1Z0.anim");
            blendTreeXMaxYMax.AddChild(clipX1Y1Z1, 9999f);
            AssetDatabase.CreateAsset(clipX1Y1Z1, folderPath + "/Anim_" + machineName + "_InputVector3_X1Y1Z1.anim");

            animatorState.motion = blendTreeX;
            
            AssetDatabase.AddObjectToAsset(blendTreeX, AssetDatabase.GetAssetPath(controller));
            blendTreeX.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMax.hideFlags = HideFlags.HideInHierarchy;
            
            AssetDatabase.AddObjectToAsset(blendTreeXMinYMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMinYMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMinYMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMinYMax.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMaxYMin, AssetDatabase.GetAssetPath(controller));
            blendTreeXMaxYMin.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(blendTreeXMaxYMax, AssetDatabase.GetAssetPath(controller));
            blendTreeXMaxYMax.hideFlags = HideFlags.HideInHierarchy;
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancedSettingsTargetEntryGameObject {
#if UNITY_EDITOR
        public bool isCollapsed;
#endif
        public GameObject gameObject;
        public string treePath;
        public bool onState = true;
    }

    [System.Serializable]
    public class CVRAdvancedSettingsDropDownEntry
    {
        public string name;

        #if UNITY_EDITOR
        public bool isCollapsed;

        public GameObjectReorderableList gameObjectList = new GameObjectReorderableList();
        
        public ReorderableList GetReorderableList(CVRAvatar avatar)
        {
            return gameObjectList.GetReorderableList(avatar);
        }
        
        #endif
    }
    
    [System.Serializable]
    public class CVRAdvancedSettingsTargetEntryMaterialColor 
    {
#if UNITY_EDITOR
        public bool isCollapsed;
#endif
        public GameObject gameObject;
        public string treePath;
        public Type propertyType;
        public string propertyTypeIdentifier;
        public string propertyName;
    }
    
    [System.Serializable]
    public class CVRAdvancedSettingsTargetEntryMaterialProperty 
    {
#if UNITY_EDITOR
        public bool isCollapsed;
#endif
        public GameObject gameObject;
        public string treePath;
        public Type propertyType;
        public string propertyTypeIdentifier;
        public string propertyName;
        public float minValue;
        public float maxValue;
    }

    public class GameObjectReorderableList {
        
        public string name;

        #if UNITY_EDITOR
        public bool isCollapsed;
        
        public List<CVRAdvancedSettingsTargetEntryGameObject> gameObjectTargets =
            new List<CVRAdvancedSettingsTargetEntryGameObject>();

        private CVRAdvancedSettingsTargetEntryGameObject entity;
        private ReorderableList gameObjectList;
        private CVRAvatar target;
        
        private void generateReorderableList()
        {
            gameObjectList = new ReorderableList(gameObjectTargets, typeof(CVRAdvancedSettingsTargetEntryGameObject), 
                true, true, true, true);
            gameObjectList.drawHeaderCallback = OnDrawHeader;
            gameObjectList.drawElementCallback = OnDrawElement;
            gameObjectList.elementHeightCallback = OnHeightElement;
            gameObjectList.onAddCallback = OnAdd;
            gameObjectList.onChangedCallback = OnChanged;
        }
        
        public ReorderableList GetReorderableList(CVRAvatar avatar)
        {
            target = avatar;
            
            if (gameObjectList == null) generateReorderableList();

            return gameObjectList;
        }

        private void OnChanged(ReorderableList list)
        {
            EditorUtility.SetDirty(target);
        }

        private void OnAdd(ReorderableList list)
        {
            gameObjectTargets.Add(new CVRAdvancedSettingsTargetEntryGameObject());
        }

        private float OnHeightElement(int index)
        {
            entity = gameObjectTargets[index];
            if (entity.isCollapsed) {
                return EditorGUIUtility.singleLineHeight * 1.25f;
            } else {
                return EditorGUIUtility.singleLineHeight * 3.75f;
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (index > gameObjectTargets.Count) return;
            entity = gameObjectTargets[index];
            
            rect.y += 2;
            rect.x += 12;
            rect.width -= 12;
            Rect _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

            entity.isCollapsed = !EditorGUI.Foldout(_rect, !entity.isCollapsed, "Name", true);
            _rect.x += 80;
            _rect.width = rect.width - 80;
            var targetGameObject = (GameObject) EditorGUI.ObjectField(_rect, entity.gameObject, typeof(GameObject), true);

            if (!entity.isCollapsed) {
                if (targetGameObject != null &&
                    targetGameObject.transform.GetComponentInParent(typeof(CVRAvatar)) == target) {
                    entity.gameObject = targetGameObject;
                    entity.treePath =
                        AnimationUtility.CalculateTransformPath(targetGameObject.transform, target.transform);
                }
                else if (entity.gameObject != targetGameObject) {
                    entity.gameObject = null;
                    entity.treePath = "";
                }

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(_rect, "Path");
                _rect.x += 80;
                _rect.width = rect.width - 80;
                EditorGUI.LabelField(_rect, entity.treePath);

                rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                _rect = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(_rect, new GUIContent("Set to",
                                                           "If checked, the object will be active once the toggle was pressed. If unchecked the object " +
                                                           "will be deactivated when the toggle is pressed."));
                _rect.x += 80;
                _rect.width = rect.width - 80;
                entity.onState = EditorGUI.Toggle(_rect, entity.onState);
            }
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect _rect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            GUI.Label(_rect, "GameObjects");
        }
        
        #endif
    }
}