using UnityEngine;

namespace PJH.Runtime.Core
{
    public class MUtil
    {
        public static string GetClosestDirection(Vector3 direction)
        {
            Vector3[] directions =
            {
                Vector3.right,
                Vector3.left,
                Vector3.forward,
                Vector3.back,
                new Vector3(0.7f, 0, 0.7f),
                new Vector3(-0.7f, 0, 0.7f),
                new Vector3(0.7f, 0, -0.7f),
                new Vector3(-0.7f, 0, -0.7f)
            };
            string[] directionNames = { "Right", "Left", "Forward", "Back", "ForwardR", "ForwardL", "BackR", "BackL" };

            float maxDot = -1f;
            int closestDirectionIndex = -1;

            for (int i = 0; i < directions.Length; i++)
            {
                float dot = Vector3.Dot(direction, directions[i]);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    closestDirectionIndex = i;
                }
            }

            return directionNames[closestDirectionIndex];
        }
    }
}