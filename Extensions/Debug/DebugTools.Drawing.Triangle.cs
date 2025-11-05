using UnityEngine;

namespace EinheitsKiste
{
    public static partial class DebugTools
    {
        public static void DrawBoundingBox(Bounds bounds, Transform transform)
            => DrawBoundingBox(bounds, transform, Color.red);

        public static void DrawBoundingBox(Bounds bounds, Transform transform, Color color)
        {
            Vector3 halfSize = bounds.size * .5f;
            Vector3 center = bounds.center;
            Vector3[] points = new Vector3[]
            {
                // top
                center + Vector3.Scale(halfSize, new Vector3(-1f, 1f, 1f)),
                center + Vector3.Scale(halfSize, new Vector3(1f, 1f, 1f)),
                center + Vector3.Scale(halfSize, new Vector3(1f, 1f, -1f)),
                center + Vector3.Scale(halfSize, new Vector3(-1f, 1f, -1f)),

                // bottom
                center + Vector3.Scale(halfSize, new Vector3(-1f, -1f, 1f)),
                center + Vector3.Scale(halfSize, new Vector3(1f, -1f, 1f)),
                center + Vector3.Scale(halfSize, new Vector3(1f, -1f, -1f)),
                center + Vector3.Scale(halfSize, new Vector3(-1f, -1f, -1f)),
            };

            for (int i = 0; i < points.Length; i++)
                points[i] = transform.localToWorldMatrix.MultiplyPoint(points[i]);

            // top
            DrawLine(points[0], points[1], color);
            DrawLine(points[1], points[2], color);
            DrawLine(points[2], points[3], color);
            DrawLine(points[3], points[0], color);

            // bottom
            DrawLine(points[4], points[5], color);
            DrawLine(points[5], points[6], color);
            DrawLine(points[6], points[7], color);
            DrawLine(points[7], points[4], color);

            // connect top and bottom
            DrawLine(points[0], points[4], color);
            DrawLine(points[1], points[5], color);
            DrawLine(points[2], points[6], color);
            DrawLine(points[3], points[7], color);
        }
    }
}