using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools
{
    public class JP_Debug : MonoBehaviour
    {
        /// <summary>
        /// Draws a cube at the specified position, offset, and of the specified size
        /// </summary>
        public static void DrawGizmoCube(Transform transform, Vector3 offset, Vector3 cubeSize, bool wireOnly)
        {
            Matrix4x4 rotationMatrix = transform.localToWorldMatrix;
            Gizmos.matrix = rotationMatrix;
            if (wireOnly)
            {
                Gizmos.DrawWireCube(offset, cubeSize);
            }
            else
            {
                Gizmos.DrawCube(offset, cubeSize);
            }
        }

        /// <summary>
        /// Draws a debug ray in 3D and does the actual raycast
        /// </summary>
        /// <returns>The raycast hit.</returns>
        /// <param name="rayOriginPoint">Ray origin point.</param>
        /// <param name="rayDirection">Ray direction.</param>
        /// <param name="rayDistance">Ray distance.</param>
        /// <param name="mask">Mask.</param>
        /// <param name="debug">If set to <c>true</c> debug.</param>
        /// <param name="color">Color.</param>
        /// <param name="drawGizmo">If set to <c>true</c> draw gizmo.</param>
        public static RaycastHit Raycast3D(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
        {
            if (drawGizmo)
            {
                Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
            }
            RaycastHit hit;
            Physics.Raycast(rayOriginPoint, rayDirection, out hit, rayDistance, mask);
            return hit;
        }

        /// <summary>
        /// Performs a box cast and visualizes the results using Unity's Debug.DrawRay function.
        /// </summary>
        /// <param name="origin">The starting point of the box cast.</param>
        /// <param name="halfExtents">Half the size of the box used in the cast.</param>
        /// <param name="direction">The direction in which to cast the box.</param>
        /// <param name="maxDistance">The maximum distance of the box cast.</param>
        /// <param name="duration">The duration (in seconds) that the debug rays should be visible.</param>
        public static void DebugBoxCast(Vector3 origin, Vector3 halfExtents, Vector3 direction, float maxDistance, LayerMask layerMask)
        {
            RaycastHit hit;
            bool isHit = Physics.BoxCast(origin, halfExtents, direction, out hit, Quaternion.identity, maxDistance, layerMask);

            if (isHit)
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.green);
                Debug.DrawRay(hit.point, hit.normal, Color.green);
                Debug.DrawRay(hit.point, Vector3.up * 0.1f, Color.red);
            }
            else
            {
                Debug.DrawRay(origin, direction * maxDistance, Color.green);
            }
        }
    }
}
