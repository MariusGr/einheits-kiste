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

        // Position and Rotation only
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
            var scale = target.localScale;
            return Lerp(target, targetPosition, targetRotation, () => scale, time, easingFunction);
        }

        // Position and Scale only
        public static IEnumerator Lerp(Transform target,
                                       Vector3 targetPosition,
                                       Vector3 targetScale,
                                       float time,
                                       Func<float, float> easingFunction = null)
        {
            var rotation = target.localRotation;
            return Lerp(target, () => targetPosition, () => rotation, () => targetScale, time, easingFunction);
        }

        public static IEnumerator Lerp(Transform target,
                                       Func<Vector3> targetPosition,
                                       Func<Vector3> targetScale,
                                       float time,
                                       Func<float, float> easingFunction = null)
        {
            var rotation = target.localRotation;
            return Lerp(target, targetPosition, () => rotation, targetScale, time, easingFunction);
        }

        // Position only
        public static IEnumerator LerpPosition(Transform target,
                                       Vector3 targetPosition,
                                       float time,
                                       Func<float, float> easingFunction = null)
        {
            target.GetLocalPositionAndRotation(out _, out var rotation);
            return Lerp(target, () => targetPosition, () => rotation, () => target.localScale, time, easingFunction);
        }

        // Rotation only
        public static IEnumerator LerpRotation(Transform target,
                                        Quaternion targetRotation,
                                        float time,
                                        Func<float, float> easingFunction = null)
          {
                target.GetLocalPositionAndRotation(out var position, out _);
                return Lerp(target, () => position, () => targetRotation, () => target.localScale, time, easingFunction);
          }

        // Scale only
        public static IEnumerator LerpScale(Transform target,
                                       Vector3 targetScale,
                                       float time,
                                       Func<float, float> easingFunction = null)
        {
            target.GetLocalPositionAndRotation(out var position, out var rotation);
            return Lerp(target, () => position, () => rotation, () => targetScale, time, easingFunction);
        }

        // Full version
        public static IEnumerator Lerp(Transform target,
                                       Vector3 targetPosition,
                                       Quaternion targetRotation,
                                       Vector3 targetScale,
                                       float time,
                                       Func<float, float> easingFunction = null)
            => Lerp(target, () => targetPosition, () => targetRotation, () => targetScale, time, easingFunction);

        // TODO Refactor so that not all lerps are being calculated if not needed
        public static IEnumerator Lerp(Transform target,
                                       Func<Vector3> targetPosition,
                                       Func<Quaternion> targetRotation,
                                       Func<Vector3> targetScale,
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
            Vector3 startingScale = target.localScale;

            while (elapsedTime < time)
            {
                float t = elapsedTime / time;
                float easedT = easingFunction(t);
                target.SetLocalPositionAndRotation(
                    Vector3.Lerp(startingPosition, targetPosition(), easedT), Quaternion.Lerp(startingRotation, targetRotation(), easedT));
                target.localScale = Vector3.Lerp(startingScale, targetScale(), easedT);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            target.SetLocalPositionAndRotation(targetPosition(), targetRotation());
        }
    }
}
