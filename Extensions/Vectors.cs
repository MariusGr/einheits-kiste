using UnityEngine;

namespace EinheitsKiste
{
    public static class Vectors
    {
        /// <summary>
        /// Checks if this vector is on the right or left of a given forward vector.
        /// Source: https://discussions.unity.com/t/left-right-test-function/399026/5
        /// </summary>
        /// <param name="forward">
        /// The vector which separates left from right side.
        /// </param>
        /// <param name="up">
        /// The vector perpendicular to 'forward'.
        /// </param>
        /// <param name="targetDirection">
        /// The vector to test.
        /// </param>
        /// <returns>
        /// -1 when targetDirection is to the left of 'forward', 1 to the right, and 0 if it is parallel to 'forward'.
        /// </returns>
        public static float LeftRightSign(this Vector3 targetDirection, Vector3 forward, Vector3 up)
        {
            Vector3 cross = Vector3.Cross(forward, targetDirection);
            float direction = Vector3.Dot(cross, up);
            return Mathf.Sign(direction);
        }
    }
}
