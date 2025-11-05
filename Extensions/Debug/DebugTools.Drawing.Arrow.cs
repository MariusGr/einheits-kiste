using UnityEngine;

namespace EinheitsKiste
{
    public static partial class DebugTools
    {
        /// <summary>
        /// DrawArrow is a helper class to draw arrows in the scene view.
        /// Source: https://forum.unity.com/threads/debug-drawarrow.85980/
        /// </summary>
        public static class DrawArrow
        {
            public static void ForGizmo(Vector3 start, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.DrawRay(start, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(start + direction, right * arrowHeadLength);
                Gizmos.DrawRay(start + direction, left * arrowHeadLength);
            }

            public static void ForGizmo(Vector3 start, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(start, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(start + direction, right * arrowHeadLength);
                Gizmos.DrawRay(start + direction, left * arrowHeadLength);
            }

            public static void ForDebug(Vector3 start, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(start, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(start + direction, right * arrowHeadLength);
                Debug.DrawRay(start + direction, left * arrowHeadLength);
            }

            public static void ForDebug(Vector3 start, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(start, direction, color);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(start + direction, right * arrowHeadLength, color);
                Debug.DrawRay(start + direction, left * arrowHeadLength, color);
            }

            public static void ForDebug(Vector3 start,
                                        Vector3 end,
                                        Vector3 facingDirection,
                                        Color color,
                                        float duration = Mathf.Infinity,
                                        float finSize = .4f)
            {
                DrawLine(start, end, color, duration);
                var fin = (end - start) * finSize;

                Vector3 arrowFin1 = Quaternion.AngleAxis(160f, facingDirection) * fin;
                Vector3 arrowFin2 = Quaternion.AngleAxis(-160f, facingDirection) * fin;

                DrawLine(end, end + arrowFin1, color, duration);
                DrawLine(end, end + arrowFin2, color, duration);
            }
        }
    }
}