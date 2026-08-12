using UnityEngine;

namespace EinheitsKiste
{
    public static class MathUtils
    {
        public static Vector3 ClosestPointOnLineSegment(Vector3 segmentStart, Vector3 segmentEnd, Vector3 point)
        {
            Vector3 segment = segmentEnd - segmentStart;
            float t = Mathf.Max(0f, Mathf.Min(1f, Vector3.Dot(point - segmentStart, segment) / Vector3.Dot(segment, segment)));
            return segmentStart + t * segment;
        }
    }
}
