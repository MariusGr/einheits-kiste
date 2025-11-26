using UnityEngine;

namespace EinheitsKiste
{
    public static class Transforms
    {
        public static void RotateAround(Transform transform, Vector3 pivotPoint, Quaternion rot)
        {
            transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
            transform.rotation = rot * transform.rotation;
        }
    }
}
