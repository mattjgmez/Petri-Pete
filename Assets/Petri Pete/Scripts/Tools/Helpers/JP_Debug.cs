using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        /// <param name="size">The size of the box used in the cast.</param>
        /// <param name="angle">The angle in degrees of the box used in the cast.</param>
        /// <param name="direction">The direction in which to cast the box.</param>
        /// <param name="distance">The distance of the box cast.</param>
        /// <param name="layerMask">The layer mask filter of the box cast.</param>
        public static void DebugBoxCast2D(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask layerMask)
        {
            bool isHit = Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask);

            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector2.zero, size);

            Gizmos.matrix = Matrix4x4.TRS(origin + (direction.normalized * distance), Quaternion.identity, Vector3.one);
            Gizmos.DrawWireCube(Vector2.zero, size);

            Gizmos.color = isHit ? Color.red : Color.cyan;
            Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.identity, Vector3.one);
            Gizmos.DrawLine(Vector2.zero, direction.normalized * distance);
        }
    }
}
