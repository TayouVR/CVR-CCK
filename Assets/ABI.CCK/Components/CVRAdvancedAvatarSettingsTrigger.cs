﻿using System;
 using UnityEngine;

namespace ABI.CCK.Components
{
    public class CVRAdvancedAvatarSettingsTrigger : MonoBehaviour
    {
        public Vector3 areaSize = new Vector3(0.05f, 0.05f, 0.05f);
        public Vector3 areaOffset = Vector3.zero;
        public string settingName;
        public float settingValue = 0;

        public void Trigger()
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
}