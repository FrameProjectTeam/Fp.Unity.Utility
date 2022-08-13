using System.Runtime.CompilerServices;

using UnityEngine;

namespace Fp.Utility
{
	/// <summary>
	///     Ray intersection functions
	///     Originally ported function from <see cref="https://iquilezles.org/articles/intersectors/" />
	/// </summary>
	public static class RayIntersection
	{
		private const float s_hexEdgeOffset = 0.866025f;

		private static readonly Vector3 s_hexNormal1 = new(1.0f, 0.0f, 0.0f);
		private static readonly Vector3 s_hexNormal2 = new(0.5f, 0.0f, s_hexEdgeOffset);
		private static readonly Vector3 s_hexNormal3 = new(-0.5f, 0.0f, s_hexEdgeOffset);
		private static readonly Vector3 s_hexNormal4 = new(0.0f, 1.0f, 0.0f);

		/// <summary>
		///     Sphere of size <see cref="radius" /> centered at point <see cref="center" />
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="center">Sphere's center</param>
		/// <param name="radius">Sphere's radius</param>
		/// <param name="t1">First intersection point along <see cref="rayDir" /></param>
		/// <param name="t2">Second intersection point along <see cref="rayDir" /></param>
		/// <returns>Return true if sphere is intersected and false if not</returns>
		public static bool SphereIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 center,
			float radius,
			out float t1,
			out float t2)
		{
			Vector3 oc = rayOrigin - center;
			float b = Vector3.Dot(oc, rayDir);
			float c = Vector3.Dot(oc, oc) - radius * radius;
			float h = b * b - c;
			if(h < 0.0)
			{
				t1 = -1;
				t2 = -1;
				return false;
			}

			h = Mathf.Sqrt(h);
			t1 = -b - h;
			t2 = -b + h;
			return true;
		}

		/// <summary>
		///     Axis aligned box centered at the origin, with size <see cref="boxSize" />
		///     The ray must be in object(box) space.
		/// </summary>
		/// <param name="rayOrigin">Ray origin point in object(box) space</param>
		/// <param name="rayDir">Ray direction in object(box) space</param>
		/// <param name="size">Box size</param>
		/// <param name="t1">First intersection point along <see cref="rayDir" /></param>
		/// <param name="t2">Second intersection point along <see cref="rayDir" /></param>
		/// <param name="normal">Intersection hit normal</param>
		/// <returns>Return true if box is intersected and false if not</returns>
		public static bool BoxIntersection(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 size,
			out float t1,
			out float t2,
			out Vector3 normal)
		{
			// can precompute if traversing a set of aligned boxes
			var m = new Vector3(1f / rayDir.x, 1f / rayDir.y, 1f / rayDir.z);
			// can precompute if traversing a set of aligned boxes
			Vector3 n = Vector3.Scale(m, rayOrigin);
			Abs(ref m);
			Vector3 k = Vector3.Scale(m, size);
			Vector3 tN = -n - k;
			Vector3 tF = -n + k;
			t1 = Mathf.Max(Mathf.Max(tN.x, tN.y), tN.z);
			t2 = Mathf.Min(Mathf.Min(tF.x, tF.y), tF.z);
			if(t1 > t2 || t2 < 0.0)
			{
				normal = Vector3.zero;
				return false;
			}

			Sign(ref rayDir);
			normal = Vector3.Scale(
				Vector3.Scale(
					-rayDir,
					Step(tN.YZX(), tN)
				),
				Step(tN.ZXY(), tN)
			);

			return true;
		}

		/// <summary>
		///     Rounded box intersection axis aligned(use object space rays)
		/// </summary>
		/// <param name="rayOrigin">Ray origin point in object(box) space</param>
		/// <param name="rayDir">Ray direction in object(box) space</param>
		/// <param name="size">Box size</param>
		/// <param name="rad">Round radius</param>
		/// <param name="t">Intersection point along <see cref="rayDir" /></param>
		/// <returns>Return true if rounded box is intersected and false if not</returns>
		public static bool RoundedBoxIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 size,
			float rad,
			out float t)
		{
			// bounding box
			var m = new Vector3(1f / rayDir.x, 1f / rayDir.y, 1f / rayDir.z);
			Vector3 n = Vector3.Scale(m, rayOrigin);
			Abs(ref m);
			Vector3 k = Vector3.Scale(m, size + new Vector3(rad, rad, rad));
			Vector3 t1 = -n - k;
			Vector3 t2 = -n + k;
			t = Mathf.Max(Mathf.Max(t1.x, t1.y), t1.z);
			float tF = Mathf.Min(Mathf.Min(t2.x, t2.y), t2.z);

			if(t > tF || tF < 0.0)
			{
				return false;
			}

			// convert to first octant
			Vector3 pos = rayOrigin + t * rayDir;
			Vector3 s = Sign(pos);
			rayOrigin.Scale(s);
			rayDir.Scale(s);
			pos.Scale(s);

			// faces
			pos -= size;
			pos = Max(pos, pos.YZX());
			if(Mathf.Min(Mathf.Min(pos.x, pos.y), pos.z) < 0.0)
			{
				return true;
			}

			// some precomputation
			Vector3 oc = rayOrigin - size;
			Vector3 dd = Vector3.Scale(rayDir, rayDir);
			Vector3 oo = Vector3.Scale(oc, oc);
			Vector3 od = Vector3.Scale(oc, rayDir);
			float ra2 = rad * rad;

			t = Mathf.Infinity;

			// corner
			{
				float b = od.x + od.y + od.z;
				float c = oo.x + oo.y + oo.z - ra2;
				float h = b * b - c;
				if(h > 0.0)
				{
					t = -b - Mathf.Sqrt(h);
				}
			}
			// edge X
			{
				float a = dd.y + dd.z;
				float b = od.y + od.z;
				float c = oo.y + oo.z - ra2;
				float h = b * b - a * c;
				if(h > 0.0)
				{
					h = (-b - Mathf.Sqrt(h)) / a;
					if(h > 0.0 && h < t && Mathf.Abs(rayOrigin.x + rayDir.x * h) < size.x)
					{
						t = h;
					}
				}
			}
			// edge Y
			{
				float a = dd.z + dd.x;
				float b = od.z + od.x;
				float c = oo.z + oo.x - ra2;
				float h = b * b - a * c;
				if(h > 0.0)
				{
					h = (-b - Mathf.Sqrt(h)) / a;
					if(h > 0.0 && h < t && Mathf.Abs(rayOrigin.y + rayDir.y * h) < size.y)
					{
						t = h;
					}
				}
			}
			// edge Z
			{
				float a = dd.x + dd.y;
				float b = od.x + od.y;
				float c = oo.x + oo.y - ra2;
				float h = b * b - a * c;
				if(h > 0.0)
				{
					h = (-b - Mathf.Sqrt(h)) / a;
					if(h > 0.0 && h < t && Mathf.Abs(rayOrigin.z + rayDir.z * h) < size.z)
					{
						t = h;
					}
				}
			}

			return !float.IsInfinity(t);
		}

		/// <summary>
		///     Calculates normal of a rounded box
		/// </summary>
		/// <param name="localPosition">Point on rounded box</param>
		/// <param name="size">Box size</param>
		/// <returns>Calculated normal vector</returns>
		public static Vector3 RoundedBoxNormal(Vector3 localPosition, Vector3 size)
		{
			return Vector3.Scale(Sign(localPosition), Vector3.Normalize(Max(Abs(localPosition) - size, Vector3.zero)));
		}

		/// <summary>
		///     Plane defined normal and offset
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="normal">Plane normal(must be normalized)</param>
		/// <param name="offset">Plane offset along <see cref="normal" /></param>
		/// <returns>Intersection point along <see cref="rayDir" /></returns>
		public static float PlaneIntersect(Vector3 rayOrigin, Vector3 rayDir, Vector3 normal, float offset = 0)
		{
			return -(Vector3.Dot(rayOrigin, normal) + offset) / Vector3.Dot(rayDir, normal);
		}

		/// <summary>
		///     Intersection with disk
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="center">Disk center point</param>
		/// <param name="normal">Disk normal</param>
		/// <param name="radius">Dist radius</param>
		/// <param name="t">Intersection point along <see cref="rayDir" /></param>
		/// <returns>Return true if disk is intersected and false if not</returns>
		public static bool DiskIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 center,
			Vector3 normal,
			float radius,
			out float t)
		{
			Vector3 o = rayOrigin - center;
			t = -Vector3.Dot(normal, o) / Vector3.Dot(rayDir, normal);
			Vector3 q = o + rayDir * t;
			return Vector3.Dot(q, q) < radius * radius;
		}

		/// <summary>
		///     Intersection with hexagonal prism
		/// </summary>
		/// <param name="rayOrigin">Ray origin point in object(box) space</param>
		/// <param name="rayDir">Ray direction in object(box) space</param>
		/// <param name="radius">Prism radius</param>
		/// <param name="height">Prism height</param>
		/// <param name="t">Intersection point along <see cref="rayDir" /></param>
		/// <param name="normal">Intersection hit normal</param>
		/// <returns>Return true if rounded box is intersected and false if not</returns>
		public static bool HexPrismIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			float radius,
			float height,
			out float t,
			out Vector3 normal)
		{
			// slabs intersections
			var t1 = new Vector2(radius, -radius).Subtract(-Vector3.Dot(rayOrigin, s_hexNormal1)).Divide(Vector3.Dot(rayDir, s_hexNormal1)).ToVector3(1f);
			var t2 = new Vector2(radius, -radius).Subtract(Vector3.Dot(rayOrigin, s_hexNormal2)).Divide(Vector3.Dot(rayDir, s_hexNormal2)).ToVector3(1f);
			var t3 = new Vector2(radius, -radius).Subtract(Vector3.Dot(rayOrigin, s_hexNormal3)).Divide(Vector3.Dot(rayDir, s_hexNormal3)).ToVector3(1f);
			var t4 = new Vector2(height, -height).Subtract(Vector3.Dot(rayOrigin, s_hexNormal4)).Divide(Vector3.Dot(rayDir, s_hexNormal4)).ToVector3(1f);

			// inetsection selection
			if(t1.y < t1.x)
			{
				t1 = new Vector3(t1.y, t1.x, -1f);
			}

			if(t2.y < t2.x)
			{
				t2 = new Vector3(t2.y, t2.x, -1f);
			}

			if(t3.y < t3.x)
			{
				t3 = new Vector3(t3.y, t3.x, -1f);
			}

			if(t4.y < t4.x)
			{
				t4 = new Vector3(t4.y, t4.x, -1f);
			}

			t = t1.x;
			normal = t1.z * s_hexNormal1;
			if(t2.x > t)
			{
				t = t2.x;
				normal = t2.z * s_hexNormal2;
			}

			if(t3.x > t)
			{
				t = t3.x;
				normal = t3.z * s_hexNormal3;
			}

			if(t4.x > t)
			{
				t = t4.x;
				normal = t4.z * s_hexNormal4;
			}

			// return tF too for exit point
			float tF = Mathf.Min(Mathf.Min(t1.y, t2.y), Mathf.Min(t3.y, t4.y));

			// no intersection
			return !(t > tF) && !(tF < 0.0);
		}

		/// <summary>
		///     Capsule defined by extremes and radius
		///     Note that only ONE of the two spherical caps is checked for intersections, which is a nice optimization
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="pointA">First extreme capsule point</param>
		/// <param name="pointB">Second extreme capsule point</param>
		/// <param name="radius">Capsule radius</param>
		/// <param name="t">Intersection point along <see cref="rayDir" /></param>
		/// <returns>Return true if capsule is intersected and false if not</returns>
		public static bool CapsuleIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 pointA,
			Vector3 pointB,
			float radius,
			out float t)
		{
			Vector3 ba = pointB - pointA;
			Vector3 oa = rayOrigin - pointA;
			float baba = Vector3.Dot(ba, ba);
			float bard = Vector3.Dot(ba, rayDir);
			float baoa = Vector3.Dot(ba, oa);
			float rdoa = Vector3.Dot(rayDir, oa);
			float oaoa = Vector3.Dot(oa, oa);
			float a = baba - bard * bard;
			float b = baba * rdoa - baoa * bard;
			float c = baba * oaoa - baoa * baoa - radius * radius * baba;
			float h = b * b - a * c;

			if(h < 0)
			{
				t = -1;
				return false;
			}

			t = (-b - Mathf.Sqrt(h)) / a;
			float y = baoa + t * bard;
			// body
			if(y > 0 && y < baba)
			{
				return true;
			}

			// caps
			Vector3 oc = y <= 0 ? oa : rayOrigin - pointB;
			b = Vector3.Dot(rayDir, oc);
			c = Vector3.Dot(oc, oc) - radius * radius;
			h = b * b - c;
			if(h > 0)
			{
				t = -b - Mathf.Sqrt(h);
				return true;
			}

			t = -1;
			return false;
		}

		/// <summary>
		///     Cylinder defined by extremes pa and pb, and radious ra
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="pointA">First extreme cylinder point</param>
		/// <param name="pointB">Second extreme cylinder point</param>
		/// <param name="radius">Cylinder radius</param>
		/// <param name="t">Intersection point along <see cref="rayDir" /></param>
		/// <param name="normal">Intersection hit normal</param>
		/// <returns>Return true if cylinder is intersected and false if not</returns>
		public static bool CylinderIntersect(
			in Vector3 rayOrigin,
			in Vector3 rayDir,
			in Vector3 pointA,
			in Vector3 pointB,
			float radius,
			out float t,
			out Vector3 normal)
		{
			Vector3 ca = pointB - pointA;
			Vector3 oc = rayOrigin - pointA;
			float caca = Vector3.Dot(ca, ca);
			float card = Vector3.Dot(ca, rayDir);
			float caoc = Vector3.Dot(ca, oc);
			float a = caca - card * card;
			float b = caca * Vector3.Dot(oc, rayDir) - caoc * card;
			float c = caca * Vector3.Dot(oc, oc) - caoc * caoc - radius * radius * caca;
			float h = b * b - a * c;
			if(h < 0.0)
			{
				t = -1;
				normal = default;

				return false;
			}

			h = Mathf.Sqrt(h);
			t = (-b - h) / a;
			// body
			float y = caoc + t * card;
			if(y > 0.0 && y < caca)
			{
				normal = (oc + t * rayDir - ca * y / caca) / radius;
				return true;
			}

			// caps
			t = ((y < 0 ? 0 : caca) - caoc) / card;
			if(Mathf.Abs(b + a * t) < h)
			{
				normal = ca * Mathf.Sign(y) / caca;
				return true;
			}

			normal = default;
			return false;
		}

		/// <summary>
		///     Infinite cylinder intersection
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="basePoint">Cylinder center point</param>
		/// <param name="axisDir">Cylinder axis</param>
		/// <param name="radius">Cylinder radius</param>
		/// <param name="t1">First intersection point along <see cref="rayDir" /></param>
		/// <param name="t2">Second intersection point along <see cref="rayDir" /></param>
		/// <returns>Return true if box is intersected and false if not</returns>
		public static bool InfCylinderIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 basePoint,
			Vector3 axisDir,
			float radius,
			out float t1,
			out float t2)
		{
			Vector3 oc = rayOrigin - basePoint;
			float card = Vector3.Dot(axisDir, rayDir);
			float caoc = Vector3.Dot(axisDir, oc);
			float a = 1.0f - card * card;
			float b = Vector3.Dot(oc, rayDir) - caoc * card;
			float c = Vector3.Dot(oc, oc) - caoc * caoc - radius * radius;
			float h = b * b - a * c;

			if(h < 0.0f)
			{
				t1 = -1;
				t2 = -1;
				return false;
			}

			h = Mathf.Sqrt(h);
			t1 = (-b - h) / a;
			t2 = (-b + h) / a;
			return true;
		}

		/// <summary>
		///     Cone defined by extremes and radii.
		///     Only one square root and one division is employed in the worst case.
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="pointA">Cone's first extreme point</param>
		/// <param name="pointB">Cone's second extreme point</param>
		/// <param name="radiusA">Cone's first extreme radius</param>
		/// <param name="radiusB">Cone's second extreme radius</param>
		/// <param name="t">Intersection point along <see cref="rayDir" /></param>
		/// <param name="normal">Intersection hit normal</param>
		/// <returns>Return true if cone is intersected and false if not</returns>
		public static bool ConeIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 pointA,
			Vector3 pointB,
			float radiusA,
			float radiusB,
			out float t,
			out Vector3 normal)
		{
			Vector3 ba = pointB - pointA;
			Vector3 oa = rayOrigin - pointA;
			Vector3 ob = rayOrigin - pointB;
			float m0 = Vector3.Dot(ba, ba);
			float m1 = Vector3.Dot(oa, ba);
			float m2 = Vector3.Dot(rayDir, ba);
			float m3 = Vector3.Dot(rayDir, oa);
			float m5 = Vector3.Dot(oa, oa);
			float m9 = Vector3.Dot(ob, ba);

			// caps
			if(m1 < 0.0)
			{
				if(Vector3.SqrMagnitude(oa * m2 - rayDir * m1) < radiusA * radiusA * m2 * m2) // delayed division
				{
					t = -m1 / m2;
					normal = -ba / Mathf.Sqrt(m0);
					return true;
				}
			}
			else if(m9 > 0.0)
			{
				t = -m9 / m2; // NOT delayed division
				if(Vector3.SqrMagnitude(ob + rayDir * t) < radiusB * radiusB)
				{
					normal = ba / Mathf.Sqrt(m0);
					return true;
				}
			}

			// body
			float rr = radiusA - radiusB;
			float hy = m0 + rr * rr;
			float k2 = m0 * m0 - m2 * m2 * hy;
			float k1 = m0 * m0 * m3 - m1 * m2 * hy + m0 * radiusA * (rr * m2 * 1.0f);
			float k0 = m0 * m0 * m5 - m1 * m1 * hy + m0 * radiusA * (rr * m1 * 2.0f - m0 * radiusA);
			float h = k1 * k1 - k2 * k0;
			if(h < 0.0)
			{
				t = -1;
				normal = Vector3.zero;
				return false;
			}

			t = (-k1 - Mathf.Sqrt(h)) / k2;
			float y = m1 + t * m2;
			if(y < 0.0 || y > m0)
			{
				normal = default;
				return false;
			}

			normal = Vector3.Normalize(m0 * (m0 * (oa + t * rayDir) + rr * ba * radiusA) - ba * hy * y);
			return true;
		}

		/// <summary>
		///     Cone defined by extremes pa and pb, and radious ra and rb.
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="pointA">Cone's first point</param>
		/// <param name="pointB">Cone's second point</param>
		/// <param name="radiusA">Cone's first radius</param>
		/// <param name="radiusB">Cone's second radius</param>
		/// <param name="t">Hit point along <see cref="rayDir" /></param>
		/// <param name="normal">Intersection hit normal</param>
		/// <returns>Return true if rounded cone is intersected and false if not</returns>
		public static bool RoundedConeIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 pointA,
			Vector3 pointB,
			float radiusA,
			float radiusB,
			out float t,
			out Vector3 normal)
		{
			Vector3 ba = pointB - pointA;
			Vector3 oa = rayOrigin - pointA;
			Vector3 ob = rayOrigin - pointB;
			float rr = radiusA - radiusB;
			float m0 = Vector3.Dot(ba, ba);
			float m1 = Vector3.Dot(ba, oa);
			float m2 = Vector3.Dot(ba, rayDir);
			float m3 = Vector3.Dot(rayDir, oa);
			float m5 = Vector3.Dot(oa, oa);
			float m6 = Vector3.Dot(ob, rayDir);
			float m7 = Vector3.Dot(ob, ob);

			// body
			float d2 = m0 - rr * rr;
			float k2 = d2 - m2 * m2;
			float k1 = d2 * m3 - m1 * m2 + m2 * rr * radiusA;
			float k0 = d2 * m5 - m1 * m1 + m1 * rr * radiusA * 2f - m0 * radiusA * radiusA;
			float h = k1 * k1 - k0 * k2;
			if(h < 0f)
			{
				t = -1;
				normal = default;
				return false;
			}

			t = (-Mathf.Sqrt(h) - k1) / k2;

			float y = m1 - radiusA * rr + t * m2;
			if(y > 0.0 && y < d2)
			{
				normal = Vector3.Normalize(d2 * (oa + t * rayDir) - ba * y);
				return true;
			}

			// caps
			float h1 = m3 * m3 - m5 + radiusA * radiusA;
			float h2 = m6 * m6 - m7 + radiusB * radiusB;

			normal = default;

			if(Mathf.Max(h1, h2) < 0)
			{
				return false;
			}

			if(h1 > 0)
			{
				t = -m3 - Mathf.Sqrt(h1);
				normal = (oa + t * rayDir) / radiusA;
			}

			if(h2 > 0)
			{
				float t2 = -m6 - Mathf.Sqrt(h2);
				if(t2 < t)
				{
					t = t2;
					normal = (ob + t * rayDir) / radiusB;
				}
			}

			return true;
		}

		/// <summary>
		///     Ellipsoid centered at the origin
		/// </summary>
		/// <param name="rayOrigin">Ray origin point(relative)</param>
		/// <param name="rayDir">Ray direction(relative)</param>
		/// <param name="radius">Radius along 3D axis</param>
		/// <param name="t1">First hit point along <see cref="rayDir" /></param>
		/// <param name="t2">Second hit point along <see cref="rayDir" /></param>
		/// <returns>Return true if ellipsoid is intersected and false if not</returns>
		public static bool EllipsoidIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 radius,
			out float t1,
			out float t2)
		{
			Vector3 ocn = rayOrigin.Divide(radius);
			Vector3 rdn = rayDir.Divide(radius);

			float a = Vector3.Dot(rdn, rdn);
			float b = Vector3.Dot(ocn, rdn);
			float c = Vector3.Dot(ocn, ocn);
			float h = b * b - a * (c - 1f);
			if(h < 0.0)
			{
				t1 = t2 = -1;
				return false;
			}

			h = Mathf.Sqrt(h);
			t1 = (-b - h) / a;
			t2 = (-b + h) / a;
			return true;
		}

		public static Vector3 EllipsoidNormal(Vector3 pos, Vector3 radius)
		{
			return Vector3.Normalize(pos.Divide(Vector3.Scale(radius, radius)));
		}

		/// <summary>
		///     Triangle defined by vertices v0, v1 and  v2
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="v0">First vertex</param>
		/// <param name="v1">Second vertex</param>
		/// <param name="v2">Third vertex</param>
		/// <param name="t">Hit point along <see cref="rayDir" /></param>
		/// <param name="uv">Barycentric coordinates</param>
		/// <returns>Return true if triangle is intersected and false if not</returns>
		public static bool TriangleIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 v0,
			Vector3 v1,
			Vector3 v2,
			out float t,
			out Vector2 uv)
		{
			Vector3 v1v0 = v1 - v0;
			Vector3 v2v0 = v2 - v0;
			Vector3 rov0 = rayOrigin - v0;
			Vector3 n = Vector3.Cross(v1v0, v2v0);
			Vector3 q = Vector3.Cross(rov0, rayDir);
			float d = 1f / Vector3.Dot(rayDir, n);
			uv.x = d * Vector3.Dot(-q, v2v0);
			uv.y = d * Vector3.Dot(q, v1v0);
			t = d * Vector3.Dot(-n, rov0);

			return uv.x >= 0 && uv.y >= 0 && uv.x + uv.y <= 1f;
		}

		/// <summary>
		///     Ellipse defined by its center c and its two axis u and v
		/// </summary>
		/// <param name="rayOrigin">Ray origin point</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="center"></param>
		/// <param name="firstAxis">First axis</param>
		/// <param name="secondAxis">Second axis</param>
		/// <param name="t">Hit point along <see cref="rayDir" /></param>
		/// <param name="align">Align by first and second axis</param>
		/// <returns>Return true if ellipse is intersected and false if not</returns>
		public static bool EllipseIntersect(
			Vector3 rayOrigin,
			Vector3 rayDir,
			Vector3 center,
			Vector3 firstAxis,
			Vector3 secondAxis,
			out float t,
			out Vector2 align)
		{
			Vector3 q = rayOrigin - center;
			Vector3 n = Vector3.Cross(firstAxis, secondAxis);
			t = -Vector3.Dot(n, q) / Vector3.Dot(rayDir, n);
			align.x = Vector3.Dot(firstAxis, q + rayDir * t);
			align.y = Vector3.Dot(secondAxis, q + rayDir * t);
			return align.x * align.x + align.y * align.y <= 1f;
		}

		public static Vector3 EllipseNormal(Vector2 firstAxis, Vector2 secondAxis)
		{
			return Vector3.Normalize(Vector3.Cross(firstAxis, secondAxis));
		}

		/// <summary>
		///     Torus intersection
		/// </summary>
		/// <param name="rayOrigin">Ray origin point in object(box) space</param>
		/// <param name="rayDir">Ray direction in object(box) space</param>
		/// <param name="torusRadius">Torus radius</param>
		/// <param name="tubeRadius">Tube radius</param>
		/// <returns>Return true if torus is intersected and false if not</returns>
		public static float TorusIntersect(Vector3 rayOrigin, Vector3 rayDir, float torusRadius, float tubeRadius)
		{
			var po = 1f;
			float sqrTorusRadius = torusRadius * torusRadius;
			float sqrTubeRadius = tubeRadius * tubeRadius;
			float m = Vector3.Dot(rayOrigin, rayOrigin);
			float n = Vector3.Dot(rayOrigin, rayDir);
			float k = (m + sqrTorusRadius - sqrTubeRadius) / 2f;
			float k3 = n;
			float k2 = n * n - sqrTorusRadius * Vector2.Dot(rayDir, rayDir) + k;
			float k1 = n * k - sqrTorusRadius * Vector2.Dot(rayDir, rayOrigin);
			float k0 = k * k - sqrTorusRadius * Vector2.Dot(rayOrigin, rayOrigin);

			if(Mathf.Abs(k3 * (k3 * k3 - k2) + k1) < 0.01)
			{
				po = -1.0f;
				(k1, k3) = (k3, k1);
				k0 = 1.0f / k0;
				k1 = k1 * k0;
				k2 = k2 * k0;
				k3 = k3 * k0;
			}

			float c2 = k2 * 2.0f - 3.0f * k3 * k3;
			float c1 = k3 * (k3 * k3 - k2) + k1;
			float c0 = k3 * (k3 * (c2 + 2.0f * k2) - 8.0f * k1) + 4.0f * k0;
			c2 /= 3.0f;
			c1 *= 2.0f;
			c0 /= 3.0f;
			float Q = c2 * c2 + c0;
			float R = c2 * c2 * c2 - 3.0f * c2 * c0 + c1 * c1;
			float h = R * R - Q * Q * Q;

			if(h >= 0.0)
			{
				h = Mathf.Sqrt(h);
				float v = Mathf.Sign(R + h) * Mathf.Pow(Mathf.Abs(R + h), 1f / 3f); // cube root
				float u = Mathf.Sign(R - h) * Mathf.Pow(Mathf.Abs(R - h), 1.0f / 3.0f); // cube root
				var s = new Vector2(v + u + 4f * c2, (v - u) * Mathf.Sqrt(3f));
				float y = Mathf.Sqrt(0.5f * (s.magnitude + s.x));
				float x = 0.5f * s.y / y;
				float r = 2.0f * c1 / (x * x + y * y);
				float t1 = x - r - k3;
				t1 = po < 0.0 ? 2.0f / t1 : t1;
				float t2 = -x - r - k3;
				t2 = po < 0.0f ? 2.0f / t2 : t2;
				var t = 1e20f;
				if(t1 > 0.0)
				{
					t = t1;
				}

				if(t2 > 0.0)
				{
					t = Mathf.Min(t, t2);
				}

				return t;
			}
			else
			{
				float sQ = Mathf.Sqrt(Q);
				float w = sQ * Mathf.Cos(Mathf.Acos(-R / (sQ * Q)) / 3.0f);
				float d2 = -(w + c2);
				if(d2 < 0.0)
				{
					return -1.0f;
				}

				float d1 = Mathf.Sqrt(d2);
				float h1 = Mathf.Sqrt(w - 2.0f * c2 + c1 / d1);
				float h2 = Mathf.Sqrt(w - 2.0f * c2 - c1 / d1);
				float t1 = -d1 - h1 - k3;
				t1 = po < 0.0f ? 2.0f / t1 : t1;
				float t2 = -d1 + h1 - k3;
				t2 = po < 0.0f ? 2.0f / t2 : t2;
				float t3 = d1 - h2 - k3;
				t3 = po < 0.0f ? 2.0f / t3 : t3;
				float t4 = d1 + h2 - k3;
				t4 = po < 0.0f ? 2.0f / t4 : t4;
				var t = 1e20f;
				if(t1 > 0.0)
				{
					t = t1;
				}

				if(t2 > 0.0)
				{
					t = Mathf.Min(t, t2);
				}

				if(t3 > 0.0)
				{
					t = Mathf.Min(t, t3);
				}

				if(t4 > 0.0)
				{
					t = Mathf.Min(t, t4);
				}

				return t;
			}
		}

		/// <summary>
		///     Calculate torus normal
		/// </summary>
		/// <param name="pos">Point in object(box) space</param>
		/// <param name="torusRadius">Torus radius</param>
		/// <param name="tubeRadius">Tube radius</param>
		/// <returns></returns>
		public static Vector3 TorusNormal(Vector3 pos, float torusRadius, float tubeRadius)
		{
			return Vector3.Normalize(
				Vector3.Scale(pos, Subtract(Vector3.Dot(pos, pos) - tubeRadius * tubeRadius, torusRadius * torusRadius * new Vector3(1.0f, 1.0f, -1.0f)))
			);
		}

		public static float Sphere4Intersect(Vector3 rayOrigin, Vector3 rayDir, in float radius)
		{
			float r2 = radius * radius;
			Vector3 d2 = Vector3.Scale(rayDir, rayDir);
			Vector3 d3 = Vector3.Scale(d2, rayDir);
			Vector3 o2 = Vector3.Scale(rayOrigin, rayOrigin);
			Vector3 o3 = Vector3.Scale(o2, rayOrigin);
			float ka = 1f / Vector3.Dot(d2, d2);
			float k3 = ka * Vector3.Dot(rayOrigin, d3);
			float k2 = ka * Vector3.Dot(o2, d2);
			float k1 = ka * Vector3.Dot(o3, rayDir);
			float k0 = ka * (Vector3.Dot(o2, o2) - r2 * r2);
			float c2 = k2 - k3 * k3;
			float c1 = k1 + 2.0f * k3 * k3 * k3 - 3.0f * k3 * k2;
			float c0 = k0 - 3.0f * k3 * k3 * k3 * k3 + 6.0f * k3 * k3 * k2 - 4.0f * k3 * k1;
			float p = c2 * c2 + c0 / 3.0f;
			float q = c2 * c2 * c2 - c2 * c0 + c1 * c1;
			float h = q * q - p * p * p;
			if(h < 0.0)
			{
				return -1.0f; //no intersection
			}

			float sh = Mathf.Sqrt(h);
			float s = Mathf.Sign(q + sh) * Mathf.Pow(Mathf.Abs(q + sh), 1.0f / 3.0f); // cuberoot
			float t = Mathf.Sign(q - sh) * Mathf.Pow(Mathf.Abs(q - sh), 1.0f / 3.0f); // cuberoot
			var w = new Vector2(s + t, s - t);
			Vector2 v = new Vector2(w.x + c2 * 4.0f, w.y * Mathf.Sqrt(3.0f)) * 0.5f;
			float r = v.magnitude;
			return -Mathf.Abs(v.y) / Mathf.Sqrt(r + v.x) - c1 / r - k3;
		}

		public static Vector3 Sphere4Normal(Vector3 pos)
		{
			return Vector3.Normalize(Vector3.Scale(Vector3.Scale(pos, pos), pos));
		}

		public static float GoursatIntersect(Vector3 rayOrigin, Vector3 rayDir, float ka, float kb)
		{
			var po = 1.0f;
			Vector3 rd2 = Vector3.Scale(rayDir, rayDir);
			Vector3 rd3 = Vector3.Scale(rd2, rayDir);
			Vector3 ro2 = Vector3.Scale(rayOrigin, rayOrigin);
			Vector3 ro3 = Vector3.Scale(ro2, rayOrigin);
			float k4 = Vector3.Dot(rd2, rd2);
			float k3 = Vector3.Dot(rayOrigin, rd3);
			float k2 = Vector3.Dot(ro2, rd2) - kb / 6.0f;
			float k1 = Vector3.Dot(ro3, rayDir) - kb * Vector3.Dot(rayDir, rayOrigin) / 2.0f;
			float k0 = Vector3.Dot(ro2, ro2) + ka - kb * Vector3.Dot(rayOrigin, rayOrigin);
			k3 /= k4;
			k2 /= k4;
			k1 /= k4;
			k0 /= k4;
			float c2 = k2 - k3 * k3;
			float c1 = k1 + k3 * (2.0f * k3 * k3 - 3.0f * k2);
			float c0 = k0 + k3 * (k3 * (c2 + k2) * 3.0f - 4.0f * k1);

			if(Mathf.Abs(c1) < 0.1 * Mathf.Abs(c2))
			{
				po = -1.0f;
				(k1, k3) = (k3, k1);
				k0 = 1.0f / k0;
				k1 *= k0;
				k2 *= k0;
				k3 *= k0;
				c2 = k2 - k3 * k3;
				c1 = k1 + k3 * (2.0f * k3 * k3 - 3.0f * k2);
				c0 = k0 + k3 * (k3 * (c2 + k2) * 3.0f - 4.0f * k1);
			}

			c0 /= 3.0f;
			float Q = c2 * c2 + c0;
			float R = c2 * c2 * c2 - 3.0f * c0 * c2 + c1 * c1;
			float h = R * R - Q * Q * Q;

			if(h > 0.0) // 2 intersections
			{
				h = Mathf.Sqrt(h);
				float s = Mathf.Sign(R + h) * Mathf.Pow(Mathf.Abs(R + h), 1.0f / 3.0f); // cube root
				float u = Mathf.Sign(R - h) * Mathf.Pow(Mathf.Abs(R - h), 1.0f / 3.0f); // cube root
				float x = s + u + 4.0f * c2;
				float y = s - u;
				float ks = x * x + y * y * 3.0f;
				float k = Mathf.Sqrt(ks);
				float t = -0.5f * po * Mathf.Abs(y) * Mathf.Sqrt(6.0f / (k + x)) - 2.0f * c1 * (k + x) / (ks + x * k) - k3;
				return po < 0.0f ? 1.0f / t : t;
			}
			else
			{
				// 4 intersections
				float sQ = Mathf.Sqrt(Q);
				float w = sQ * Mathf.Cos(Mathf.Acos(-R / (sQ * Q)) / 3.0f);
				float d2 = -w - c2;
				if(d2 < 0.0)
				{
					return -1.0f; //no intersection
				}

				float d1 = Mathf.Sqrt(d2);
				float h1 = Mathf.Sqrt(w - 2.0f * c2 + c1 / d1);
				float h2 = Mathf.Sqrt(w - 2.0f * c2 - c1 / d1);
				float t1 = -d1 - h1 - k3;
				t1 = po < 0.0f ? 1.0f / t1 : t1;
				float t2 = -d1 + h1 - k3;
				t2 = po < 0.0f ? 1.0f / t2 : t2;
				float t3 = d1 - h2 - k3;
				t3 = po < 0.0f ? 1.0f / t3 : t3;
				float t4 = d1 + h2 - k3;
				t4 = po < 0.0f ? 1.0f / t4 : t4;
				var t = 1e20f;
				if(t1 > 0.0)
				{
					t = t1;
				}

				if(t2 > 0.0)
				{
					t = Mathf.Min(t, t2);
				}

				if(t3 > 0.0)
				{
					t = Mathf.Min(t, t3);
				}

				if(t4 > 0.0)
				{
					t = Mathf.Min(t, t4);
				}

				return t;
			}
		}

		public static Vector3 GoursatNormal(Vector3 pos, float kb)
		{
			return Vector3.Normalize(4.0f * Vector3.Scale(Vector3.Scale(pos, pos), pos) - 2.0f * pos * kb);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Subtract(float a, Vector3 b)
		{
			return new Vector3(a - b.x, a - b.y, a - b.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 YZX(this Vector3 input)
		{
			return new Vector3(input.y, input.z, input.x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 ZXY(this Vector3 input)
		{
			return new Vector3(input.z, input.x, input.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Step(Vector3 edge, Vector3 x)
		{
			return new Vector3(Step(edge.x, x.x), Step(edge.y, x.y), Step(edge.z, x.z));
		}

		private static float Step(float edge, float x)
		{
			return x < edge ? 0 : 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Sign(ref Vector3 v)
		{
			v.x = Mathf.Sign(v.x);
			v.y = Mathf.Sign(v.y);
			v.z = Mathf.Sign(v.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Sign(Vector3 v)
		{
			Sign(ref v);
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Abs(ref Vector3 v)
		{
			v.x = Mathf.Abs(v.x);
			v.y = Mathf.Abs(v.y);
			v.z = Mathf.Abs(v.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Abs(Vector3 v)
		{
			Abs(ref v);
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Max(Vector3 a, Vector3 b)
		{
			return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector2 Divide(this Vector2 a, float b)
		{
			return new Vector2(a.x / b, a.y / b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector2 Subtract(this Vector2 a, float b)
		{
			return new Vector2(a.x - b, a.y - b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 ToVector3(this Vector2 xy, float z)
		{
			return new Vector3(xy.x, xy.y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 Divide(this Vector3 a, Vector3 b)
		{
			return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
		}
	}
}