using UnityEngine;
using System.Collections;
using System;

namespace EinheitsKiste
{

    public static class EasingFunctions
    {
        public static float Linear(float t) => t;
        public static float EasyIn2(float t) => t * t;
        public static float EasyIn3(float t) => t * t * t;
        public static float EasyOut(float t) => 1f - Mathf.Pow(1f - t, 3);
        public static float EasyInOut(float t)
        {
            if (t < 0.5f)
                return 4f * t * t * t;
            else
            {
                float f = (2f * t) - 2f;
                return 0.5f * f * f * f + 1f;
            }
        }
    }

    public static class LerpOverTime
    {
        public static IEnumerator Lerp(float start, float target, float time, Action<float> onUpdate, Func<float, float> easingFunction = null)
        {
            easingFunction ??= EasingFunctions.Linear;

            if (time <= 0f)
            {
                onUpdate(target);
                yield break;
            }

            float elapsedTime = 0f;
            while (elapsedTime < time)
            {
                float t = elapsedTime / time;
                float easedT = easingFunction(t);
                onUpdate(Mathf.Lerp(start, target, easedT));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            onUpdate(target);
        }

        public static IEnumerator Lerp(Transform target,
                                       Vector3 targetPosition,
                                       Quaternion targetRotation,
                                       float time,
                                       Func<float, float> easingFunction = null)
         => Lerp(target, () => targetPosition, () => targetRotation, time, easingFunction);

        public static IEnumerator Lerp(Transform target,
                                       Func<Vector3> targetPosition,
                                       Func<Quaternion> targetRotation,
                                       float time,
                                       Func<float, float> easingFunction = null)
        {
            easingFunction ??= EasingFunctions.Linear;

            if (time <= 0f)
            {
                target.SetLocalPositionAndRotation(targetPosition(), targetRotation());

                yield break;
            }

            float elapsedTime = 0f;
            target.GetLocalPositionAndRotation(out Vector3 startingPosition, out Quaternion startingRotation);

            while (elapsedTime < time)
            {
                float t = elapsedTime / time;
                float easedT = easingFunction(t);
                target.SetLocalPositionAndRotation(
                    Vector3.Lerp(startingPosition, targetPosition(), easedT), Quaternion.Lerp(startingRotation, targetRotation(), easedT));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            target.SetLocalPositionAndRotation(targetPosition(), targetRotation());
        }
    }
}
