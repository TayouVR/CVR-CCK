﻿﻿using System;
 using UnityEngine;

namespace ABI.CCK.Components
{
    public class CVRAdvancedAvatarSettingsPointer : MonoBehaviour
    {
        private void Start()
        {
            var sphereCollider = gameObject.AddComponent<SphereCollider>();

            sphereCollider.isTrigger = true;
            sphereCollider.radius = 0.00125f;
        }

        private void OnDrawGizmos()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = Color.cyan;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawSphere(Vector3.zero, 0.015f);
            }
        }
    }
}