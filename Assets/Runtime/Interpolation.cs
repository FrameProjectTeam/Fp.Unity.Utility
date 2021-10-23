using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Fp.Utility
{
    public static class Interpolation
    {
        public enum EaseType
        {
            Linear,
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc
        }

        public delegate float EaseFunction(float start, float end, float t);

        public delegate Vector3 ToVector3<in T>(T v);

        /// <summary>
        ///     Returns sequence generator from start to end over duration using the
        ///     given easing function. The sequence is generated as it is accessed
        ///     using the Time.deltaTime to calculate the portion of duration that has
        ///     elapsed.
        /// </summary>
        public static IEnumerator GenerateEase(EaseFunction ease, Vector3 start, Vector3 end, float duration)
        {
            IEnumerable<float> timer = GenerateTimer(duration);
            return GenerateEase(ease, start, end, duration, timer);
        }

        /// <summary>
        ///     Instead of easing based on time, generate n interpolated points (slices)
        ///     between the start and end positions.
        /// </summary>
        public static IEnumerator GenerateEase(EaseFunction ease, Vector3 start, Vector3 end, int slices)
        {
            IEnumerable<float> counter = GenerateCounter(0, slices + 1, 1);
            return GenerateEase(ease, start, end, slices + 1, counter);
        }

        /// <summary>
        ///     Returns the static method that implements the given easing type for scalars.
        ///     Use this method to easily switch between easing interpolation types.
        /// </summary>
        public static EaseFunction Ease(EaseType type)
        {
            // Source Flash easing functions:
            // http://gizma.com/easing/
            // http://www.robertpenner.com/easing/easing_demo.html
            //
            // Changed to use more friendly variable names, that follow my Lerp
            // conventions:
            // start = b (start value)
            // distance = c (change in value)
            // elapsedTime = t (current time)
            // duration = d (time duration)

            EaseFunction f = null;
            switch (type)
            {
                case EaseType.Linear:
                    f = Linear;
                    break;
                case EaseType.EaseInQuad:
                    f = EaseInQuad;
                    break;
                case EaseType.EaseOutQuad:
                    f = EaseOutQuad;
                    break;
                case EaseType.EaseInOutQuad:
                    f = EaseInOutQuad;
                    break;
                case EaseType.EaseInCubic:
                    f = EaseInCubic;
                    break;
                case EaseType.EaseOutCubic:
                    f = EaseOutCubic;
                    break;
                case EaseType.EaseInOutCubic:
                    f = EaseInOutCubic;
                    break;
                case EaseType.EaseInQuart:
                    f = EaseInQuart;
                    break;
                case EaseType.EaseOutQuart:
                    f = EaseOutQuart;
                    break;
                case EaseType.EaseInOutQuart:
                    f = EaseInOutQuart;
                    break;
                case EaseType.EaseInQuint:
                    f = EaseInQuint;
                    break;
                case EaseType.EaseOutQuint:
                    f = EaseOutQuint;
                    break;
                case EaseType.EaseInOutQuint:
                    f = EaseInOutQuint;
                    break;
                case EaseType.EaseInSine:
                    f = EaseInSine;
                    break;
                case EaseType.EaseOutSine:
                    f = EaseOutSine;
                    break;
                case EaseType.EaseInOutSine:
                    f = EaseInOutSine;
                    break;
                case EaseType.EaseInExpo:
                    f = EaseInExpo;
                    break;
                case EaseType.EaseOutExpo:
                    f = EaseOutExpo;
                    break;
                case EaseType.EaseInOutExpo:
                    f = EaseInOutExpo;
                    break;
                case EaseType.EaseInCirc:
                    f = EaseInCirc;
                    break;
                case EaseType.EaseOutCirc:
                    f = EaseOutCirc;
                    break;
                case EaseType.EaseInOutCirc:
                    f = EaseInOutCirc;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return f;
        }

        /// <summary>
        ///     Returns sequence generator from the first node to the last node over
        ///     duration time using the points in-between the first and last node
        ///     as control points of a bezier curve used to generate the interpolated points
        ///     in the sequence. If there are no control points (ie. only two nodes, first
        ///     and last) then this behaves exactly the same as NewEase(). In other words
        ///     a zero-degree bezier spline curve is just the easing method. The sequence
        ///     is generated as it is accessed using the Time.deltaTime to calculate the
        ///     portion of duration that has elapsed.
        /// </summary>
        public static IEnumerable<Vector3> GenerateBezier(EaseFunction ease, Transform[] nodes, float duration)
        {
            IEnumerable<float> timer = GenerateTimer(duration);
            return GenerateBezier<Transform>(ease, nodes, TransformDotPosition, duration, timer);
        }

        /// <summary>
        ///     Instead of interpolating based on time, generate n interpolated points
        ///     (slices) between the first and last node.
        /// </summary>
        public static IEnumerable<Vector3> GenerateBezier(EaseFunction ease, Transform[] nodes, int slices)
        {
            IEnumerable<float> counter = GenerateCounter(0, slices + 1, 1);
            return GenerateBezier<Transform>(ease, nodes, TransformDotPosition, slices + 1, counter);
        }

        /// <summary>
        ///     A Vector3[] variation of the Transform[] NewBezier() function.
        ///     Same functionality but using Vector3s to define bezier curve.
        /// </summary>
        public static IEnumerable<Vector3> GenerateBezier(EaseFunction ease, Vector3[] points, float duration)
        {
            IEnumerable<float> timer = GenerateTimer(duration);
            return GenerateBezier<Vector3>(ease, points, Identity, duration, timer);
        }

        /// <summary>
        ///     A Vector3[] variation of the Transform[] NewBezier() function.
        ///     Same functionality but using Vector3s to define bezier curve.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="points"></param>
        /// <param name="slices"></param>
        /// <returns></returns>
        public static IEnumerable<Vector3> GenerateBezier(EaseFunction ease, Vector3[] points, int slices)
        {
            IEnumerable<float> counter = GenerateCounter(0, slices + 1, 1);
            return GenerateBezier<Vector3>(ease, points, Identity, slices + 1, counter);
        }

        /// <summary>
        ///     Returns sequence generator from the first node, through each control point,
        ///     and to the last node. N points are generated between each node (slices)
        ///     using Catmull-Rom.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="slices"></param>
        /// <param name="loop"></param>
        /// <param name="tension"></param>
        /// <returns></returns>
        public static IEnumerable<Vector3> GenerateCatmullRom(Transform[] nodes, int slices, float tension = 0.5f, bool loop = false)
        {
            return GenerateCatmullRom(nodes, TransformDotPosition, slices, tension, loop);
        }

        /// <summary>
        ///     A Vector3[] variation of the Transform[] NewCatmullRom() function.
        ///     Same functionality but using Vector3s to define curve.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="slices"></param>
        /// <param name="tension"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static IEnumerable<Vector3> GenerateCatmullRom(Vector3[] points, int slices, float tension = 0.5f, bool loop = false)
        {
            return GenerateCatmullRom(points, Identity, slices, tension, loop);
        }

        /// <summary>
        ///     Calculates hermit curve position at t[0, 1]
        /// </summary>
        /// <returns></returns>
        public static Vector3 Hermit(
            Vector3 start,
            Vector3 end,
            Vector3 tanStart,
            Vector3 tanEnd,
            float t)
        {
            // Hermite curve formula:
            // (2t^3 - 3t^2 + 1) * p0 + (t^3 - 2t^2 + t) * m0 + (-2t^3 + 3t^2) * p1 + (t^3 - t^2) * m1

            float tSqr = t * t;
            float tCube = tSqr * t;

            Vector3 position =
                (2.0f * tCube - 3.0f * tSqr + 1.0f) * start +
                (tCube - 2.0f * tSqr + t) * tanStart +
                (-2.0f * tCube + 3.0f * tSqr) * end +
                (tCube - tSqr) * tanEnd;

            return position;
        }

        /// <summary>
        ///     Calculate hermit tangent at curve
        /// </summary>
        public static Vector3 HermitTangent(
            Vector3 start,
            Vector3 end,
            Vector3 tanStart,
            Vector3 tanEnd,
            float t)
        {
            // Calculate tangents
            // p'(t) = (6t² - 6t)p0 + (3t² - 4t + 1)m0 + (-6t² + 6t)p1 + (3t² - 2t)m1
            float tSqr = t * t;
            Vector3 tangent =
                (6 * tSqr - 6 * t) * start +
                (3 * tSqr - 4 * t + 1) * tanStart +
                (-6 * tSqr + 6 * t) * end +
                (3 * tSqr - 2 * t) * tanEnd;

            return tangent.normalized;
        }

        /// <summary>
        ///     Generic catmull-rom spline sequence generator used to implement the Vector3[] and Transform[] variants.
        ///     Normally you would not use this function directly.
        /// </summary>
        public static IEnumerable<Vector3> GenerateCatmullRom<T>(
            IList<T> nodes,
            ToVector3<T> toVector3,
            int slices,
            float tension = 0.5f,
            bool loop = false)
        {
            // need at least two nodes to spline between
            if (nodes.Count < 2)
            {
                yield break;
            }

            // yield the first point explicitly, if looping the first point
            // will be generated again in the step for loop when interpolating
            // from last point back to the first point
            yield return toVector3(nodes[0]);

            int last = nodes.Count - 1;
            for (var current = 0; loop || current < last; current++)
            {
                // wrap around when looping
                if (loop && current > last)
                {
                    current = 0;
                }

                // handle edge cases for looping and non-looping scenarios
                // when looping we wrap around, when not looping use start for previous
                // and end for next when you at the ends of the nodes array
                int previous = current == 0 ? loop ? last : current : current - 1;
                int start = current;
                int end = current == last ? loop ? 0 : current : current + 1;
                int next = end == last ? loop ? 0 : end : end + 1;

                // adding one guarantees yielding at least the end point
                int stepCount = slices + 1;
                for (var step = 1; step <= stepCount; step++)
                {
                    yield return CatmullRom(toVector3(nodes[previous]),
                                            toVector3(nodes[start]),
                                            toVector3(nodes[end]),
                                            toVector3(nodes[next]),
                                            (float)step / stepCount, tension);
                }
            }
        }

        /// <summary>
        ///     A Vector3 Catmull-Rom spline. Catmull-Rom splines are similar to bezier
        ///     splines but have the useful property that the generated curve will go
        ///     through each of the control points.
        ///     NOTE: The <see cref="GenerateCatmullRom" /> functions are an easier to use alternative to this
        ///     raw Catmull-Rom implementation.
        /// </summary>
        /// <param name="previous">
        ///     the point just before the start point or the start point itself if no previous point is
        ///     available
        /// </param>
        /// <param name="start">generated when elapsedTime == 0</param>
        /// <param name="end">generated when elapsedTime >= duration</param>
        /// <param name="next">the point just after the end point or the end point itself if no next point is available</param>
        /// <param name="t">Time value for ease function</param>
        /// <param name="tension">Catmull-Rom tension</param>
        /// <returns></returns>
        public static Vector3 CatmullRom(
            Vector3 previous,
            Vector3 start,
            Vector3 end,
            Vector3 next,
            float t,
            float tension = 0.5f)
        {
            float tSqr = t * t;
            float tCube = tSqr * t;

            return
                previous * (-tension * t + 2 * tension * tSqr - tension * tCube) +
                start * (1 + (tension - 3) * tSqr + (2 - tension) * tCube) +
                end * (tension * t + (3 - 2 * tension) * tSqr + (tension - 2) * tCube) +
                next * (-tension * tSqr + tension * tCube);
        }

        private static Vector3 Identity(Vector3 v)
        {
            return v;
        }

        private static Vector3 TransformDotPosition(Transform t)
        {
            return t.position;
        }

        private static IEnumerable<float> GenerateTimer(float duration)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < duration)
            {
                yield return elapsedTime;
                elapsedTime += Time.deltaTime;
                // make sure last value is never skipped
                if (elapsedTime >= duration)
                {
                    yield return elapsedTime;
                }
            }
        }

        /// <summary>
        ///     Generates sequence of integers from start to end (inclusive) one step at a time.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private static IEnumerable<float> GenerateCounter(int start, int end, int step)
        {
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
        }

        /// <summary>
        ///     Generic easing sequence generator used to implement the time and slice variants. Normally you would not use this
        ///     function directly.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="duration"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        private static IEnumerator GenerateEase(
            EaseFunction ease,
            Vector3 start,
            Vector3 end,
            float duration,
            IEnumerable<float> times)
        {
            Vector3 distance = end - start;
            foreach (float i in times)
            {
                yield return Ease(ease, start, distance, i / duration);
            }
        }

        /// <summary>
        ///     Vector3 interpolation using given easing method. Easing is done independently on all three vector axis.
        /// </summary>
        /// <param name="ease">Ease function</param>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="t">Time use for ease</param>
        /// <returns></returns>
        private static Vector3 Ease(
            EaseFunction ease,
            Vector3 start,
            Vector3 end,
            float t)
        {
            start.x = ease(start.x, end.x, t);
            start.y = ease(start.y, end.y, t);
            start.z = ease(start.z, end.z, t);
            return start;
        }

        /// <summary>
        ///     Generic bezier spline sequence generator used to implement the time and slice variants. Normally you would not use
        ///     this function directly.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="nodes"></param>
        /// <param name="toVector3"></param>
        /// <param name="maxStep"></param>
        /// <param name="steps"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerable<Vector3> GenerateBezier<T>(
            EaseFunction ease,
            IList nodes,
            ToVector3<T> toVector3,
            float maxStep,
            IEnumerable<float> steps)
        {
            // need at least two nodes to spline between
            if (nodes.Count < 2)
            {
                yield break;
            }

            // copy nodes array since Bezier is destructive
            var points = new Vector3[nodes.Count];

            foreach (float step in steps)
            {
                // re-initialize copy before each destructive call to Bezier
                for (var i = 0; i < nodes.Count; i++)
                {
                    points[i] = toVector3((T)nodes[i]);
                }

                yield return Bezier(ease, points, step / maxStep);
                // make sure last value is always generated
            }
        }

        /// <summary>
        ///     A Vector3 n-degree bezier spline.
        ///     WARNING: The points array is modified by Bezier. See NewBezier() for a safe and user friendly alternative.
        ///     You can pass zero control points, just the start and end points, for just plain easing.
        ///     In other words a zero-degree bezier spline curve is just the easing method.
        /// </summary>
        /// <param name="ease">Ease function</param>
        /// <param name="points">start point, n control points, end point</param>
        /// <param name="t">Time value for ease function</param>
        private static Vector3 Bezier(EaseFunction ease, Vector3[] points, float t)
        {
            // Reference: http://ibiblio.org/e-notes/Splines/Bezier.htm
            // Interpolate the n starting points to generate the next j = (n - 1) points,
            // then interpolate those n - 1 points to generate the next n - 2 points,
            // continue this until we have generated the last point (n - (n - 1)), j = 1.
            // We store the next set of output points in the same array as the
            // input points used to generate them. This works because we store the
            // result in the slot of the input point that is no longer used for this
            // iteration.
            for (int j = points.Length - 1; j > 0; j--)
            {
                for (var i = 0; i < j; i++)
                {
                    points[i].x = ease(points[i].x, points[i + 1].x - points[i].x, t);
                    points[i].y = ease(points[i].y, points[i + 1].y - points[i].y, t);
                    points[i].z = ease(points[i].z, points[i + 1].z - points[i].z, t);
                }
            }

            return points[0];
        }

        /// <summary>
        ///     Linear interpolation (same as Mathf.Lerp)
        /// </summary>
        private static float Linear(float start, float distance, float t)
        {
            // clamp elapsedTime to be <= duration
            return distance * t + start;
        }

        /// <summary>
        ///     quadratic easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInQuad(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            return distance * t * t + start;
        }

        /// <summary>
        ///     quadratic easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutQuad(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            return -distance * t * (t - 2) + start;
        }

        /// <summary>
        ///     quadratic easing in/out - acceleration until halfway, then deceleration
        /// </summary>
        private static float EaseInOutQuad(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t /= 0.5f;
            if (t < 1)
            {
                return distance / 2 * t * t + start;
            }

            t--;
            return -distance / 2 * (t * (t - 2) - 1) + start;
        }

        /// <summary>
        ///     cubic easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInCubic(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            return distance * t * t * t + start;
        }

        /// <summary>
        ///     cubic easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutCubic(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t--;
            return distance * (t * t * t + 1) + start;
        }

        /// <summary>
        ///     cubic easing in/out - acceleration until halfway, then deceleration
        /// </summary>
        private static float EaseInOutCubic(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t /= 2;
            if (t < 1)
            {
                return distance / 2 * t * t * t + start;
            }

            t -= 2;
            return distance / 2 * (t * t * t + 2) + start;
        }

        /// <summary>
        ///     quartic easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInQuart(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            return distance * t * t * t * t + start;
        }

        /// <summary>
        ///     quartic easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutQuart(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t--;
            return -distance * (t * t * t * t - 1) + start;
        }

        /// <summary>
        ///     quartic easing in/out - acceleration until halfway, then deceleration
        /// </summary>
        private static float EaseInOutQuart(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t /= 0.5f;
            if (t < 1)
            {
                return distance / 2 * t * t * t * t + start;
            }

            t -= 2;
            return -distance / 2 * (t * t * t * t - 2) + start;
        }

        /// <summary>
        ///     quintic easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInQuint(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            return distance * t * t * t * t * t + start;
        }

        /// <summary>
        ///     quintic easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutQuint(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t--;
            return distance * (t * t * t * t * t + 1) + start;
        }

        /// <summary>
        ///     quintic easing in/out - acceleration until halfway, then deceleration
        /// </summary>
        private static float EaseInOutQuint(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t /= 0.5f;
            if (t < 1)
            {
                return distance / 2 * t * t * t * t * t + start;
            }

            t -= 2;
            return distance / 2 * (t * t * t * t * t + 2) + start;
        }

        /// <summary>
        ///     sinusoidal easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInSine(float start, float distance, float t)
        {
            // clamp elapsedTime to be <= duration
            return -distance * Mathf.Cos(t * (Mathf.PI / 2)) + distance + start;
        }

        /// <summary>
        ///     sinusoidal easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutSine(float start, float distance, float t)
        {
            return distance * Mathf.Sin(t * (Mathf.PI / 2)) + start;
        }

        /// <summary>
        ///     sinusoidal easing in/out - accelerating until halfway, then decelerating
        /// </summary>
        private static float EaseInOutSine(float start, float distance, float t)
        {
            // clamp elapsedTime to be <= duration
            return -distance / 2 * (Mathf.Cos(Mathf.PI * t) - 1) + start;
        }

        /// <summary>
        ///     exponential easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInExpo(float start, float distance, float t)
        {
            // clamp elapsedTime to be <= duration
            return distance * Mathf.Pow(2, 10 * (t - 1)) + start;
        }

        /// <summary>
        ///     exponential easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutExpo(float start, float distance, float t)
        {
            // clamp elapsedTime to be <= duration
            return distance * (-Mathf.Pow(2, -10 * t) + 1) + start;
        }

        /// <summary>
        ///     exponential easing in/out - accelerating until halfway, then decelerating
        /// </summary>
        private static float EaseInOutExpo(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t /= 0.5f;
            if (t < 1)
            {
                return distance / 2 * Mathf.Pow(2, 10 * (t - 1)) + start;
            }

            t--;
            return distance / 2 * (-Mathf.Pow(2, -10 * t) + 2) + start;
        }

        /// <summary>
        ///     circular easing in - accelerating from zero velocity
        /// </summary>
        private static float EaseInCirc(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            return -distance * (Mathf.Sqrt(1 - t * t) - 1) + start;
        }

        /// <summary>
        ///     circular easing out - decelerating to zero velocity
        /// </summary>
        private static float EaseOutCirc(float start, float distance, float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t--;
            return distance * Mathf.Sqrt(1 - t * t) + start;
        }

        /// <summary>
        ///     circular easing in/out - acceleration until halfway, then deceleration
        /// </summary>
        private static float EaseInOutCirc(
            float start,
            float distance,
            float t)
        {
            // clamp elapsedTime so that it cannot be greater than duration
            t /= 0.5f;
            if (t < 1)
            {
                return -distance / 2 * (Mathf.Sqrt(1 - t * t) - 1) + start;
            }

            t -= 2;
            return distance / 2 * (Mathf.Sqrt(1 - t * t) + 1) + start;
        }
    }
}