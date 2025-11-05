using UnityEngine;

namespace EinheitsKiste
{
    public static partial class DebugTools
    {
        public static void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
            => DrawTriangle(point1, point2, point3, Color.red);

        public static void DrawTriangle(Vector3 point1, Vector3 point2, Vector3 point3, Color color)
        {
            DrawLine(point1, point2, color);
            DrawLine(point2, point3, color);
            DrawLine(point3, point1, color);
        }
    }
}