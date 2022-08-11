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
			EaseInOutCirc,
			InElastic,
			OutElastic,
			InOutElastic,
			InBack,
			OutBack,
			InOutBack,
			InBounce,
			OutBounce,
			InOutBounce
		}

		public delegate float EaseFunction(float t);

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
			switch(type)
			{
				case EaseType.Linear:
					f = Linear;
					break;
				case EaseType.EaseInQuad:
					f = InQuad;
					break;
				case EaseType.EaseOutQuad:
					f = OutQuad;
					break;
				case EaseType.EaseInOutQuad:
					f = InOutQuad;
					break;
				case EaseType.EaseInCubic:
					f = InCubic;
					break;
				case EaseType.EaseOutCubic:
					f = OutCubic;
					break;
				case EaseType.EaseInOutCubic:
					f = InOutCubic;
					break;
				case EaseType.EaseInQuart:
					f = InQuart;
					break;
				case EaseType.EaseOutQuart:
					f = OutQuart;
					break;
				case EaseType.EaseInOutQuart:
					f = InOutQuart;
					break;
				case EaseType.EaseInQuint:
					f = InQuint;
					break;
				case EaseType.EaseOutQuint:
					f = OutQuint;
					break;
				case EaseType.EaseInOutQuint:
					f = InOutQuint;
					break;
				case EaseType.EaseInSine:
					f = InSine;
					break;
				case EaseType.EaseOutSine:
					f = OutSine;
					break;
				case EaseType.EaseInOutSine:
					f = InOutSine;
					break;
				case EaseType.EaseInExpo:
					f = InExpo;
					break;
				case EaseType.EaseOutExpo:
					f = OutExpo;
					break;
				case EaseType.EaseInOutExpo:
					f = InOutExpo;
					break;
				case EaseType.EaseInCirc:
					f = InCirc;
					break;
				case EaseType.EaseOutCirc:
					f = OutCirc;
					break;
				case EaseType.EaseInOutCirc:
					f = InOutCirc;
					break;
				case EaseType.InElastic:
					f = InElastic;
					break;
				case EaseType.OutElastic:
					f = OutElastic;
					break;
				case EaseType.InOutElastic:
					f = InOutElastic;
					break;
				case EaseType.InBack:
					f = InBack;
					break;
				case EaseType.OutBack:
					f = OutBack;
					break;
				case EaseType.InOutBack:
					f = InOutBack;
					break;
				case EaseType.InBounce:
					f = InBounce;
					break;
				case EaseType.OutBounce:
					f = OutBounce;
					break;
				case EaseType.InOutBounce:
					f = InOutBounce;
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
			if(nodes.Count < 2)
			{
				yield break;
			}

			// yield the first point explicitly, if looping the first point
			// will be generated again in the step for loop when interpolating
			// from last point back to the first point
			yield return toVector3(nodes[0]);

			int last = nodes.Count - 1;
			for(var current = 0; loop || current < last; current++)
			{
				// wrap around when looping
				if(loop && current > last)
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
				for(var step = 1; step <= stepCount; step++)
				{
					yield return CatmullRom(
						toVector3(nodes[previous]),
						toVector3(nodes[start]),
						toVector3(nodes[end]),
						toVector3(nodes[next]),
						(float)step / stepCount, tension
					);
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

		public static float Linear(float t)
		{
			return t;
		}

		public static float InQuad(float t)
		{
			return t * t;
		}

		public static float OutQuad(float t)
		{
			return 1 - InQuad(1 - t);
		}

		public static float InOutQuad(float t)
		{
			if(t < 0.5)
			{
				return InQuad(t * 2) / 2;
			}

			return 1 - InQuad((1 - t) * 2) / 2;
		}

		public static float InCubic(float t)
		{
			return t * t * t;
		}

		public static float OutCubic(float t)
		{
			return 1 - InCubic(1 - t);
		}

		public static float InOutCubic(float t)
		{
			if(t < 0.5)
			{
				return InCubic(t * 2) / 2;
			}

			return 1 - InCubic((1 - t) * 2) / 2;
		}

		public static float InQuart(float t)
		{
			return t * t * t * t;
		}

		public static float OutQuart(float t)
		{
			return 1 - InQuart(1 - t);
		}

		public static float InOutQuart(float t)
		{
			if(t < 0.5)
			{
				return InQuart(t * 2) / 2;
			}

			return 1 - InQuart((1 - t) * 2) / 2;
		}

		public static float InQuint(float t)
		{
			return t * t * t * t * t;
		}

		public static float OutQuint(float t)
		{
			return 1 - InQuint(1 - t);
		}

		public static float InOutQuint(float t)
		{
			if(t < 0.5)
			{
				return InQuint(t * 2) / 2;
			}

			return 1 - InQuint((1 - t) * 2) / 2;
		}

		public static float InSine(float t)
		{
			return (float)-Math.Cos(t * Math.PI / 2);
		}

		public static float OutSine(float t)
		{
			return (float)Math.Sin(t * Math.PI / 2);
		}

		public static float InOutSine(float t)
		{
			return (float)(Math.Cos(t * Math.PI) - 1) / -2;
		}

		public static float InExpo(float t)
		{
			return (float)Math.Pow(2, 10 * (t - 1));
		}

		public static float OutExpo(float t)
		{
			return 1 - InExpo(1 - t);
		}

		public static float InOutExpo(float t)
		{
			if(t < 0.5)
			{
				return InExpo(t * 2) / 2;
			}

			return 1 - InExpo((1 - t) * 2) / 2;
		}

		public static float InCirc(float t)
		{
			return -((float)Math.Sqrt(1 - t * t) - 1);
		}

		public static float OutCirc(float t)
		{
			return 1 - InCirc(1 - t);
		}

		public static float InOutCirc(float t)
		{
			if(t < 0.5)
			{
				return InCirc(t * 2) / 2;
			}

			return 1 - InCirc((1 - t) * 2) / 2;
		}

		public static float InElastic(float t)
		{
			return 1 - OutElastic(1 - t);
		}

		public static float OutElastic(float t)
		{
			var p = 0.3f;
			return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
		}

		public static float InOutElastic(float t)
		{
			if(t < 0.5)
			{
				return InElastic(t * 2) / 2;
			}

			return 1 - InElastic((1 - t) * 2) / 2;
		}

		public static float InBack(float t)
		{
			var s = 1.70158f;
			return t * t * ((s + 1) * t - s);
		}

		public static float OutBack(float t)
		{
			return 1 - InBack(1 - t);
		}

		public static float InOutBack(float t)
		{
			if(t < 0.5)
			{
				return InBack(t * 2) / 2;
			}

			return 1 - InBack((1 - t) * 2) / 2;
		}

		public static float InBounce(float t)
		{
			return 1 - OutBounce(1 - t);
		}

		public static float OutBounce(float t)
		{
			var div = 2.75f;
			var mult = 7.5625f;

			if(t < 1 / div)
			{
				return mult * t * t;
			}

			if(t < 2 / div)
			{
				t -= 1.5f / div;
				return mult * t * t + 0.75f;
			}

			if(t < 2.5 / div)
			{
				t -= 2.25f / div;
				return mult * t * t + 0.9375f;
			}

			t -= 2.625f / div;
			return mult * t * t + 0.984375f;
		}

		public static float InOutBounce(float t)
		{
			if(t < 0.5)
			{
				return InBounce(t * 2) / 2;
			}

			return 1 - InBounce((1 - t) * 2) / 2;
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
			while(elapsedTime < duration)
			{
				yield return elapsedTime;
				elapsedTime += Time.deltaTime;
				// make sure last value is never skipped
				if(elapsedTime >= duration)
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
			for(int i = start; i <= end; i += step)
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
			foreach(float i in times)
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
			start.x = Ease(ease, start.x, end.x, t);
			start.y = Ease(ease, start.y, end.y, t);
			start.z = Ease(ease, start.z, end.z, t);
			return start;
		}

		/// <summary>
		///     Single(float) interpolation using given easing method.
		/// </summary>
		/// <param name="ease">Ease function</param>
		/// <param name="start">Start value</param>
		/// <param name="distance">End value</param>
		/// <param name="t">Time use for ease</param>
		/// <returns></returns>
		private static float Ease(EaseFunction ease, float start, float distance, float t)
		{
			return distance * ease(t) + start;
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
			if(nodes.Count < 2)
			{
				yield break;
			}

			// copy nodes array since Bezier is destructive
			var points = new Vector3[nodes.Count];

			foreach(float step in steps)
			{
				// re-initialize copy before each destructive call to Bezier
				for(var i = 0; i < nodes.Count; i++)
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
			for(int j = points.Length - 1; j > 0; j--)
			{
				for(var i = 0; i < j; i++)
				{
					points[i].x = Ease(ease, points[i].x, points[i + 1].x - points[i].x, t);
					points[i].y = Ease(ease, points[i].y, points[i + 1].y - points[i].y, t);
					points[i].z = Ease(ease, points[i].z, points[i + 1].z - points[i].z, t);
				}
			}

			return points[0];
		}
	}
}