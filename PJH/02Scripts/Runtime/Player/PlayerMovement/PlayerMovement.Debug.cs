using System;
using UnityEngine;

namespace PJH.Runtime.Players
{
    public partial class PlayerMovement
    {
        private void OnDrawGizmos()
        {
            Vector3 origin = transform.position + Vector3.up * 1f;
            Vector3 direction = transform.forward;
            Quaternion rotation = transform.rotation;

            Vector3 endPoint = origin + direction * _castDistanceWhenAttacking;

            Gizmos.color = Color.red;

            Gizmos.matrix = Matrix4x4.TRS(origin, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _boxCastHalfExtentsWhenAttacking * 2);

            Gizmos.matrix = Matrix4x4.TRS(endPoint, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _boxCastHalfExtentsWhenAttacking * 2);

            Gizmos.matrix = Matrix4x4.identity;
            Vector3 boxCenter = origin + (endPoint - origin) * 0.5f;
            Vector3 boxSize = new Vector3(
                _boxCastHalfExtentsWhenAttacking.x * 2,
                _boxCastHalfExtentsWhenAttacking.y * 2,
                _castDistanceWhenAttacking + _boxCastHalfExtentsWhenAttacking.z * 2
            );

            Gizmos.matrix = Matrix4x4.TRS(boxCenter, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}