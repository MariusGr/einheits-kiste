using UnityEngine;

namespace EinheitsKiste
{
    public static class Transforms
    {
        public static void RotateAround(Transform transform, Vector3 pivotPoint, Quaternion rot)
            => transform.SetPositionAndRotation(rot * (transform.position - pivotPoint) + pivotPoint, rot * transform.rotation);
    }
}
