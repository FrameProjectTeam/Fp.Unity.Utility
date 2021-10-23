using System;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace Fp.Utility
{
    public static class VectorMath
    {
        private const float DoublePi = Mathf.PI * 2;

        public static bool IsNaN(Vector2 v)
        {
            return float.IsNaN(v.sqrMagnitude);
        }

        public static bool IsNaN(Vector3 v)
        {
            return float.IsNaN(v.sqrMagnitude);
        }

        public static bool IsNaN(Vector4 v)
        {
            return float.IsNaN(v.sqrMagnitude);
        }

        public static float SqrDistance(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        /// <summary>
        ///     Get Vector2 from angle
        /// </summary>
        /// <param name="angle">Rotation angle</param>
        /// <param name="useRadians">If angle in radians</param>
        /// <param name="yDominant"></param>
        /// <returns></returns>
        public static Vector2 AngleToVector2(float angle, bool useRadians = false, bool yDominant = false)
        {
            if (!useRadians)
            {
                angle *= MathUtils.DegToRad;
            }

            return yDominant ? new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) : new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Vector3 Clamp(Vector3 input, Vector3 min, Vector3 max)
        {
            input.x = MathUtils.Clamp(input.x, min.x, max.x);
            input.y = MathUtils.Clamp(input.y, min.y, max.y);
            input.z = MathUtils.Clamp(input.z, min.z, max.z);
            return input;
        }

        public static Vector3 Clamp(Vector2 input, Vector2 min, Vector2 max)
        {
            input.x = MathUtils.Clamp(input.x, min.x, max.x);
            input.y = MathUtils.Clamp(input.y, min.y, max.y);
            return input;
        }

        /// <summary>
        ///     Get the angle in degrees off the forward defined by x.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static float Angle(Vector2 dir)
        {
            return Angle(dir.y, dir.x);
        }

        /// <summary>
        ///     Get the angle in degrees off the forward defined by x.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Angle(float x, float y)
        {
            return Mathf.Atan2(y, x) * MathUtils.RadToDeg;
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            // // Due to float error the dot / mag can sometimes be ever so slightly over 1, which can cause NaN in acos.
            //return Mathf.Acos(Vector2.Dot(a, b) / (a.magnitude * b.magnitude)) * MathUtil.RAD_TO_DEG;
            double d = Vector2.Dot(a, b) / ((double)a.magnitude * b.magnitude);
            if (d >= 1d)
            {
                return 0f;
            }

            if (d <= -1d)
            {
                return 180f;
            }

            return (float)Math.Acos(d) * MathUtils.RadToDeg;
        }

        /// <summary>
        ///     Angle in degrees off some axis in the counter-clockwise direction. Think of like 'Angle' or 'Atan2' where you get
        ///     to control
        ///     which axis as opposed to only measuring off of <1,0>.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float AngleOff(Vector2 v, Vector2 axis)
        {
            if (axis.sqrMagnitude < MathUtils.EpsilonSingle)
            {
                return float.NaN;
            }

            axis.Normalize();
            var tang = new Vector2(-axis.y, axis.x);
            return AngleBetween(v, axis) * Mathf.Sign(Vector2.Dot(v, tang));
        }

        public static void Reflect(ref Vector2 v, Vector2 normal)
        {
            float dp = 2f * Vector2.Dot(v, normal);
            float ix = v.x - normal.x * dp;
            float iy = v.y - normal.y * dp;
            v.x = ix;
            v.y = iy;
        }

        public static Vector2 Reflect(Vector2 v, Vector2 normal)
        {
            float dp = 2 * Vector2.Dot(v, normal);
            return new Vector2(v.x - normal.x * dp, v.y - normal.y * dp);
        }

        public static void Mirror(ref Vector2 v, Vector2 axis)
        {
            v = 2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis - v;
        }

        public static Vector2 Mirror(Vector2 v, Vector2 axis)
        {
            return 2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis - v;
        }

        /// <summary>
        ///     Rotate Vector2 counter-clockwise by <see cref="angle" /> in degrease
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 RotateByDegrease(Vector2 vector, float angle)
        {
            return RotateByRadians(vector, angle * MathUtils.DegToRad);
        }

        /// <summary>
        ///     Rotate Vector2 counter-clockwise by <see cref="radians" /> angle in radians
        /// </summary>
        /// <param name="v"></param>
        /// <param name="radians">Angle in radians</param>
        /// <returns></returns>
        public static Vector2 RotateByRadians(Vector2 v, float radians)
        {
            double ca = Math.Cos(radians);
            double sa = Math.Sin(radians);
            double rx = v.x * ca - v.y * sa;

            return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
        }

        /// <summary>
        ///     Rotates a vector toward another. Magnitude of the from vector is maintained.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="radians">Angle in radians</param>
        /// <returns></returns>
        public static Vector2 RotateTowardRad(Vector2 from, Vector2 to, float radians)
        {
            float a1 = Mathf.Atan2(from.y, from.x);
            float a2 = Mathf.Atan2(to.y, to.x);
            a2 = MathUtils.ShortenAngleToAnother(a2, a1);
            float ra = a2 - a1 >= 0f ? a1 + radians : a1 - radians;
            float l = from.magnitude;
            return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
        }

        /// <summary>
        ///     Rotates a vector toward another. Magnitude of the from vector is maintained.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="degrease">Angle in degrease</param>
        /// <returns></returns>
        public static Vector2 RotateToward(Vector2 from, Vector2 to, float degrease)
        {
            return RotateTowardRad(from, to, degrease * MathUtils.DegToRad);
        }

        public static Vector2 RotateTowardRadClamped(Vector2 from, Vector2 to, float radians)
        {
            float a1 = Mathf.Atan2(from.y, from.x);
            float a2 = Mathf.Atan2(to.y, to.x);
            a2 = MathUtils.ShortenAngleToAnother(a2, a1);

            float da = a2 - a1;
            float ra = a1 + Mathf.Clamp(Mathf.Abs(radians), 0f, Mathf.Abs(da)) * Mathf.Sign(da);

            float l = from.magnitude;
            return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
        }

        public static Vector2 RotateTowardClamped(Vector2 from, Vector2 to, float degrease)
        {
            return RotateTowardRadClamped(from, to, degrease * MathUtils.DegToRad);
        }

        /// <summary>
        ///     Angular interpolates between two vectors.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="t"></param>
        /// <returns>
        ///     The vectors are 2 dimensional, so technically this is not a spherical linear interpolation. The name Slerp is kept
        ///     for consistency.
        ///     The result would be if you Slerped between 2 Vector3's that had a z value of 0. The direction interpolates at an
        ///     angular rate, where as the
        ///     magnitude interpolates at a linear rate.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Slerp(Vector2 from, Vector2 to, float t)
        {
            float a = MathUtils.NormalizeAngleRad(Mathf.Lerp(Mathf.Atan2(from.y, from.x), Mathf.Atan2(to.y, to.x), t));
            float l = Mathf.Lerp(from.magnitude, to.magnitude, t);
            return new Vector2(Mathf.Cos(a) * l, Mathf.Sin(a) * l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CrossProduct2D(Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 CrossProduct2D(Vector2 v)
        {
            return new Vector2(v.y, -v.x);
        }

        public static Vector2 Orthogonal(Vector2 v)
        {
            return new Vector2(-v.y, v.x);
        }

        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            double d = Math.Sqrt(a.sqrMagnitude * (double)b.sqrMagnitude);
            if (d < MathUtils.MinDouble)
            {
                return 0f;
            }

            d = Vector3.Dot(a, b) / d;
            if (d >= 1d)
            {
                return 0f;
            }

            if (d <= -1d)
            {
                return 180f;
            }

            return (float)Math.Acos(d) * MathUtils.RadToDeg;
        }

        /// <summary>
        ///     Returns a vector orthogonal to up in the general direction of forward.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Vector3 GetForwardTangent(Vector3 forward, Vector3 up)
        {
            return Vector3.Cross(Vector3.Cross(up, forward), up);
        }

        /// <summary>
        ///     Returns the closest point on the line. The line is treated as infinite.
        ///     ClosestPointOnSegment
        ///     ClosestPointOnLineFactor
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
            float dot = Vector3.Dot(point - lineStart, lineDirection);

            return lineStart + dot * lineDirection;
        }

        /// <summary>
        ///     Factor along the line which is closest to the point. Returned value is in the range [0,1] if the point lies on the
        ///     segment otherwise it just lies on the line. The closest point can be calculated using (end-start)*factor + start.
        ///     <see cref="ClosestPointOnLine" />
        ///     <see cref="ClosestPointOnSegment" />
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float ClosestPointOnLineFactor(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 dir = lineEnd - lineStart;
            float sqrMgn = dir.sqrMagnitude;

            if (sqrMgn <= MathUtils.EpsilonSingle)
            {
                return 0;
            }

            return Vector3.Dot(point - lineStart, dir) / sqrMgn;
        }

        /// <summary>
        ///     Factor along the line which is closest to the point. Returned value is in the range [0,1] if the point lies on the
        ///     segment otherwise it just lies on the line. The closest point can be calculated using (end-start)*factor + start
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float ClosestPointOnLineFactor(Vector3Int lineStart, Vector3Int lineEnd, Vector3Int point)
        {
            Vector3Int lineDirection = lineEnd - lineStart;
            float mgn = lineDirection.sqrMagnitude;

            float closestPoint = Vector3.Dot(point - lineStart, lineDirection);

            if (mgn > MathUtils.EpsilonSingle)
            {
                closestPoint /= mgn;
            }

            return closestPoint;
        }

        /// <summary>
        ///     Factor of the nearest point on the segment. Returned value is in the range [0,1] if the point lies on the segment
        ///     otherwise it just lies on the line. The closest point can be calculated using (end-start)*factor + start;
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float ClosestPointOnLineFactor(Vector2Int lineStart, Vector2Int lineEnd, Vector2Int point)
        {
            Vector2Int lineDirection = lineEnd - lineStart;
            float mgn = lineDirection.sqrMagnitude;

            float closestPoint = Vector2.Dot(point - lineStart, lineDirection);

            if (mgn > MathUtils.EpsilonSingle)
            {
                closestPoint /= mgn;
            }

            return closestPoint;
        }

        /// <summary>
        ///     Returns the closest point on the segment. The segment is NOT treated as infinite.
        ///     <see href="ClosestPointOnLine" />
        ///     <see cref="ClosestPointOnSegmentXZ" />
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ClosestPointOnSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 dir = lineEnd - lineStart;
            float sqrMgn = dir.sqrMagnitude;

            if (sqrMgn <= MathUtils.EpsilonSingle)
            {
                return lineStart;
            }

            float factor = Vector3.Dot(point - lineStart, dir) / sqrMgn;
            return lineStart + Mathf.Clamp01(factor) * dir;
        }

        /// <summary>
        ///     Returns the closest point on the segment in the XZ plane. The y coordinate of the result will be the same as the y
        ///     coordinate of the \a point parameter. The segment is NOT treated as infinite.
        ///     <see cref="ClosestPointOnSegment" />
        ///     <see cref="ClosestPointOnLine" />
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 ClosestPointOnSegmentXZ(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            lineStart.y = point.y;
            lineEnd.y = point.y;
            Vector3 fullDirection = lineEnd - lineStart;
            Vector3 fullDirection2 = fullDirection;
            fullDirection2.y = 0;
            float mgn = fullDirection2.magnitude;
            Vector3 lineDirection = mgn > float.Epsilon ? fullDirection2 / mgn : Vector3.zero;

            float closestPoint = Vector3.Dot(point - lineStart, lineDirection);
            return lineStart + Mathf.Clamp(closestPoint, 0.0f, fullDirection2.magnitude) * lineDirection;
        }

        /// <summary>
        ///     Returns the approximate shortest squared distance between x,z and the segment p-q. The segment is not considered
        ///     infinite. This function is not entirely exact, but it is about twice as fast as DistancePointSegment2.
        ///     TODO: Is this actually approximate? It looks exact.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="px"></param>
        /// <param name="pz"></param>
        /// <param name="qx"></param>
        /// <param name="qz"></param>
        /// <returns></returns>
        public static float SqrDistancePointSegmentApproximate(
            int x,
            int z,
            int px,
            int pz,
            int qx,
            int qz)
        {
            int pqx = qx - px;
            var pqz = (float)(qz - pz);
            var dx = (float)(x - px);
            var dz = (float)(z - pz);
            float d = pqx * pqx + pqz * pqz;
            float t = pqx * dx + pqz * dz;

            if (d > 0)
            {
                t /= d;
            }

            if (t < 0)
            {
                t = 0;
            }
            else if (t > 1)
            {
                t = 1;
            }

            dx = px + t * pqx - x;
            dz = pz + t * pqz - z;

            return dx * dx + dz * dz;
        }

        /// <summary>
        ///     Returns the approximate shortest squared distance between x,z and the segment p-q.
        ///     The segment is not considered infinite.
        ///     This function is not entirely exact, but it is about twice as fast as DistancePointSegment2.
        ///     TODO: Is this actually approximate? It looks exact.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float SqrDistancePointSegmentApproximate(Vector3Int a, Vector3Int b, Vector3Int p)
        {
            var pqx = (float)(b.x - a.x);
            var pqz = (float)(b.z - a.z);
            var dx = (float)(p.x - a.x);
            var dz = (float)(p.z - a.z);
            float d = pqx * pqx + pqz * pqz;
            float t = pqx * dx + pqz * dz;

            if (d > 0)
            {
                t /= d;
            }

            if (t < 0)
            {
                t = 0;
            }
            else if (t > 1)
            {
                t = 1;
            }

            dx = a.x + t * pqx - p.x;
            dz = a.z + t * pqz - p.z;

            return dx * dx + dz * dz;
        }

        /// <summary>
        ///     Returns the squared distance between p and the segment a-b.
        ///     The line is not considered infinite.
        /// </summary>
        public static float SqrDistancePointSegment(Vector3 a, Vector3 b, Vector3 p)
        {
            Vector3 nearest = ClosestPointOnSegment(a, b, p);

            return (nearest - p).sqrMagnitude;
        }

        /// <summary>
        ///     3D minimum distance between 2 segments.
        ///     Input: two 3D line segments S1 and S2
        ///     \returns the shortest squared distance between S1 and S2
        /// </summary>
        public static float SqrDistanceSegmentSegment(Vector3 s1, Vector3 e1, Vector3 s2, Vector3 e2)
        {
            Vector3 u = e1 - s1;
            Vector3 v = e2 - s2;
            Vector3 w = s1 - s2;
            float a = Vector3.Dot(u, u); // always >= 0
            float b = Vector3.Dot(u, v);
            float c = Vector3.Dot(v, v); // always >= 0
            float d = Vector3.Dot(u, w);
            float e = Vector3.Dot(v, w);
            float D = a * c - b * b; // always >= 0
            float sc, sN, sD = D; // sc = sN / sD, default sD = D >= 0
            float tc, tN, tD = D; // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < 0.000001f)
            {
                // the lines are almost parallel
                sN = 0.0f; // force using point P0 on segment S1
                sD = 1.0f; // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = b * e - c * d;
                tN = a * e - b * d;
                if (sN < 0.0f)
                {
                    // sc < 0 => the s=0 edge is visible
                    sN = 0.0f;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {
                    // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0f)
            {
                // tc < 0 => the t=0 edge is visible
                tN = 0.0f;
                // recompute sc for this edge
                if (-d < 0.0f)
                {
                    sN = 0.0f;
                }
                else if (-d > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if (-d + b < 0.0f)
                {
                    sN = 0;
                }
                else if (-d + b > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = -d + b;
                    sD = a;
                }
            }

            // finally do the division to get sc and tc
            sc = Mathf.Abs(sN) < 0.000001f ? 0.0f : sN / sD;
            tc = Mathf.Abs(tN) < 0.000001f ? 0.0f : tN / tD;

            // get the difference of the two closest points
            Vector3 dP = w + sc * u - tc * v; // =  S1(sc) - S2(tc)

            return dP.sqrMagnitude; // return the closest distance
        }

        /// <summary>
        ///     Squared distance between two points in the XZ plane ///
        /// </summary>
        public static float SqrDistanceXz(Vector3 a, Vector3 b)
        {
            Vector3 delta = a - b;

            return delta.x * delta.x + delta.z * delta.z;
        }

        /// <summary>
        ///     Signed area of a triangle in the XZ plane multiplied by 2.
        ///     This will be negative for clockwise triangles and positive for counter-clockwise ones
        /// </summary>
        public static long SignedTriangleAreaTimes2Xz(Vector3Int a, Vector3Int b, Vector3Int c)
        {
            return (b.x - a.x) * (long)(c.z - a.z) - (c.x - a.x) * (long)(b.z - a.z);
        }

        /// <summary>
        ///     Signed area of a triangle in the XZ plane multiplied by 2.
        ///     This will be negative for clockwise triangles and positive for counter-clockwise ones.
        /// </summary>
        public static float SignedTriangleAreaTimes2Xz(Vector3 a, Vector3 b, Vector3 c)
        {
            return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
        }

        /// <summary>
        ///     Returns if \a p lies on the right side of the line \a a - \a b.
        ///     Uses XZ space. Does not return true if the points are colinear.
        /// </summary>
        public static bool RightXz(Vector3 a, Vector3 b, Vector3 p)
        {
            return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) < -float.Epsilon;
        }

        /// <summary>
        ///     Returns if \a p lies on the right side of the line \a a - \a b.
        ///     Uses XZ space. Does not return true if the points are colinear.
        /// </summary>
        public static bool RightXz(Vector3Int a, Vector3Int b, Vector3Int p)
        {
            return (b.x - a.x) * (long)(p.z - a.z) - (p.x - a.x) * (long)(b.z - a.z) < 0;
        }

        /// <summary>
        ///     Returns if \a p lies on the right side of the line \a a - \a b.
        ///     Also returns true if the points are colinear.
        /// </summary>
        public static bool RightOrCollinear(Vector2 a, Vector2 b, Vector2 p)
        {
            return (b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y) <= 0;
        }

        /// <summary>
        ///     Returns if \a p lies on the right side of the line \a a - \a b.
        ///     Also returns true if the points are colinear.
        /// </summary>
        public static bool RightOrCollinear(Vector2Int a, Vector2Int b, Vector2Int p)
        {
            return (b.x - a.x) * (long)(p.y - a.y) - (p.x - a.x) * (long)(b.y - a.y) <= 0;
        }

        /// <summary>
        ///     Returns if \a p lies on the left side of the line \a a - \a b.
        ///     Uses XZ space. Also returns true if the points are colinear.
        /// </summary>
        public static bool RightOrColinearXZ(Vector3 a, Vector3 b, Vector3 p)
        {
            return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) <= 0;
        }

        /// <summary>
        ///     Returns if \a p lies on the left side of the line \a a - \a b.
        ///     Uses XZ space. Also returns true if the points are colinear.
        /// </summary>
        public static bool RightOrColinearXZ(Vector3Int a, Vector3Int b, Vector3Int p)
        {
            return (b.x - a.x) * (long)(p.z - a.z) - (p.x - a.x) * (long)(b.z - a.z) <= 0;
        }

        /// <summary>
        ///     Returns if the points a in a clockwise order.
        ///     Will return true even if the points are colinear or very slightly counter-clockwise
        ///     (if the signed area of the triangle formed by the points has an area less than or equals to float.Epsilon) ///
        /// </summary>
        public static bool IsClockwiseMarginXZ(Vector3 a, Vector3 b, Vector3 c)
        {
            return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z) <= float.Epsilon;
        }

        /// <summary>
        ///     Returns if the points a in a clockwise order ///
        /// </summary>
        public static bool IsClockwiseXZ(Vector3 a, Vector3 b, Vector3 c)
        {
            return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z) < 0;
        }

        /// <summary>
        ///     Returns if the points a in a clockwise order ///
        /// </summary>
        public static bool IsClockwiseXZ(Vector3Int a, Vector3Int b, Vector3Int c)
        {
            return RightXz(a, b, c);
        }

        /// <summary>
        ///     Returns true if the points a in a clockwise order or if they are colinear ///
        /// </summary>
        public static bool IsClockwiseOrColinearXZ(Vector3Int a, Vector3Int b, Vector3Int c)
        {
            return RightOrColinearXZ(a, b, c);
        }

        /// <summary>
        ///     Returns true if the points a in a clockwise order or if they are colinear ///
        /// </summary>
        public static bool IsClockwiseOrColinear(Vector2Int a, Vector2Int b, Vector2Int c)
        {
            return RightOrCollinear(a, b, c);
        }

        /// <summary>
        ///     Returns if the points are colinear (lie on a straight line) ///
        /// </summary>
        public static bool IsColinearXZ(Vector3Int a, Vector3Int b, Vector3Int c)
        {
            return (b.x - a.x) * (long)(c.z - a.z) - (c.x - a.x) * (long)(b.z - a.z) == 0;
        }

        /// <summary>
        ///     Returns if the points are colinear (lie on a straight line) ///
        /// </summary>
        public static bool IsColinearXZ(Vector3 a, Vector3 b, Vector3 c)
        {
            float v = (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);

            // Epsilon not chosen with much though, just that float.Epsilon was a bit too small.
            return v <= 0.0000001f && v >= -0.0000001f;
        }

        /// <summary>
        ///     Returns if the points are colinear (lie on a straight line) ///
        /// </summary>
        public static bool IsColinearAlmostXZ(Vector3Int a, Vector3Int b, Vector3Int c)
        {
            long v = (b.x - a.x) * (long)(c.z - a.z) - (c.x - a.x) * (long)(b.z - a.z);

            return v > -1 && v < 1;
        }

        /// <summary>
        ///     Returns if the line segment \a startPointB - \a endPointB intersects the line segment \a startPointA - \a
        ///     endPointA.
        ///     If only the endpoints coincide, the result is undefined (may be true or false).
        /// </summary>
        public static bool SegmentsIntersect(Vector2Int startPointA, Vector2Int endPointA, Vector2Int startPointB, Vector2Int endPointB)
        {
            return RightOrCollinear(startPointA, endPointA, startPointB) != RightOrCollinear(startPointA, endPointA, endPointB) &&
                RightOrCollinear(startPointB, endPointB, startPointA) != RightOrCollinear(startPointB, endPointB, endPointA);
        }

        /// <summary>
        ///     Returns if the line segment \a startPointB - \a endPointB intersects the line segment \a startPointA - \a
        ///     endPointA.
        ///     If only the endpoints coincide, the result is undefined (may be true or false).
        ///     \note XZ space
        /// </summary>
        public static bool SegmentsIntersectXZ(Vector3Int startPointA, Vector3Int endPointA, Vector3Int startPointB, Vector3Int endPointB)
        {
            return RightOrColinearXZ(startPointA, endPointA, startPointB) != RightOrColinearXZ(startPointA, endPointA, endPointB) &&
                RightOrColinearXZ(startPointB, endPointB, startPointA) != RightOrColinearXZ(startPointB, endPointB, endPointA);
        }

        /// <summary>
        ///     Returns if the two line segments intersects. The lines are NOT treated as infinite (just for clarification)
        ///     <see cref="IntersectionPoint" />
        /// </summary>
        public static bool SegmentsIntersectXz(Vector3 startPointA, Vector3 endPointA, Vector3 startPointB, Vector3 endPointB)
        {
            Vector3 directionA = endPointA - startPointA;
            Vector3 directionB = endPointB - startPointB;

            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                return false;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            float nom2 = directionA.x * (startPointA.z - startPointB.z) - directionA.z * (startPointA.x - startPointB.x);
            float u = nom / den;
            float u2 = nom2 / den;

            if (u < 0F || u > 1F || u2 < 0F || u2 > 1F)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Intersection point between two infinite lines.
        ///     Note that start points and directions are taken as parameters instead of start and end points.
        ///     Lines are treated as infinite. If the lines are parallel 'startPointA' will be returned.
        ///     Intersections are calculated on the XZ plane.
        ///     \see LineIntersectionPointXZ
        /// </summary>
        public static Vector3 LineDirIntersectionPointXz(Vector3 startPointA, Vector3 directionA, Vector3 startPointB, Vector3 directionB)
        {
            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                return startPointA;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            float u = nom / den;

            return startPointA + directionA * u;
        }

        /// <summary>
        ///     Intersection point between two infinite lines.
        ///     Note that start points and directions are taken as parameters instead of start and end points.
        ///     Lines are treated as infinite. If the lines are parallel 'startPointA' will be returned.
        ///     Intersections are calculated on the XZ plane.
        ///     \see LineIntersectionPointXZ
        /// </summary>
        public static Vector3 LineDirIntersectionPointXz(
            Vector3 startPointA,
            Vector3 directionA,
            Vector3 startPointB,
            Vector3 directionB,
            out bool intersects)
        {
            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                intersects = false;
                return startPointA;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            float u = nom / den;

            intersects = true;
            return startPointA + directionA * u;
        }

        /// <summary>
        ///     Returns if the ray (startPointA, endPointA) intersects the segment (startPointB, endPointB).
        ///     false is returned if the lines are parallel.
        ///     Only the XZ coordinates are used.
        ///     \todo Double check that this actually works
        /// </summary>
        public static bool RaySegmentIntersectXz(Vector3Int startPointA, Vector3Int endPointA, Vector3Int startPointB, Vector3Int endPointB)
        {
            Vector3Int directionA = endPointA - startPointA;
            Vector3Int directionB = endPointB - startPointB;

            long den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (den == 0)
            {
                return false;
            }

            long nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            long nom2 = directionA.x * (startPointA.z - startPointB.z) - directionA.z * (startPointA.x - startPointB.x);

            //factor1 < 0
            // If both have the same sign, then nom/den < 0 and thus the segment cuts the ray before the ray starts
            if (!((nom < 0) ^ (den < 0)))
            {
                return false;
            }

            //factor2 < 0
            if (!((nom2 < 0) ^ (den < 0)))
            {
                return false;
            }

            if (den >= 0 && nom2 > den || den < 0 && nom2 <= den)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Returns the intersection factors for line 1 and line 2. The intersection factors is a distance along the line \a
        ///     start - \a end where the other line intersects it.\n
        ///     \code intersectionPoint = startPointA + factor1 * (endPointA-startPointA) \endcode
        ///     \code intersectionPoVector2Int = startPointB + factor2 * (endPointB-startPointB) \endcode
        ///     Lines are treated as infinite.\n
        ///     false is returned if the lines are parallel and true if they are not.
        ///     Only the XZ coordinates are used.
        /// </summary>
        public static bool LineIntersectionFactorXz(
            Vector3Int startPointA,
            Vector3Int endPointA,
            Vector3Int startPointB,
            Vector3Int endPointB,
            out float factor1,
            out float factor2)
        {
            Vector3Int directionA = endPointA - startPointA;
            Vector3Int directionB = endPointB - startPointB;

            long den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (den == 0)
            {
                factor1 = 0;
                factor2 = 0;
                return false;
            }

            long nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            long nom2 = directionA.x * (startPointA.z - startPointB.z) - directionA.z * (startPointA.x - startPointB.x);

            factor1 = (float)nom / den;
            factor2 = (float)nom2 / den;

            return true;
        }

        /// <summary>
        ///     Returns the intersection factors for line 1 and line 2. The intersection factors is a distance along the line \a
        ///     start - \a end where the other line intersects it.\n
        ///     \code intersectionPoint = startPointA + factor1 * (endPointA-startPointA) \endcode
        ///     \code intersectionPoVector2Int = startPointB + factor2 * (endPointB-startPointB) \endcode
        ///     Lines are treated as infinite.\n
        ///     false is returned if the lines are parallel and true if they are not.
        ///     Only the XZ coordinates are used.
        /// </summary>
        public static bool LineIntersectionFactorXz(
            Vector3 startPointA,
            Vector3 endPointA,
            Vector3 startPointB,
            Vector3 endPointB,
            out float factor1,
            out float factor2)
        {
            Vector3 directionA = endPointA - startPointA;
            Vector3 directionB = endPointB - startPointB;

            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                factor1 = 0;
                factor2 = 0;
                return false;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            float nom2 = directionA.x * (startPointA.z - startPointB.z) - directionA.z * (startPointA.x - startPointB.x);

            float u = nom / den;
            float u2 = nom2 / den;

            factor1 = u;
            factor2 = u2;

            return true;
        }

        /// <summary>
        ///     Returns the intersection factor for line 1 with ray 2.
        ///     The intersection factors is a factor distance along the line \a start - \a end where the other line intersects
        ///     it.\n
        ///     \code intersectionPoint = startPointA + factor * (endPointA-startPointA) \endcode
        ///     Lines are treated as infinite.\n
        ///     The second "line" is treated as a ray, meaning only matches on startPointB or forwards towards endPointB (and
        ///     beyond) will be
        ///     returned
        ///     If the point lies on the wrong side of the ray start, Nan will be returned.
        ///     NaN is returned if the lines are parallel. ///
        /// </summary>
        public static float LineRayIntersectionFactorXz(Vector3Int startPointA, Vector3Int endPointA, Vector3Int startPointB, Vector3Int endPointB)
        {
            Vector3Int directionA = endPointA - startPointA;
            Vector3Int directionB = endPointB - startPointB;

            int den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (den == 0)
            {
                return float.NaN;
            }

            int nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            int nom2 = directionA.x * (startPointA.z - startPointB.z) - directionA.z * (startPointA.x - startPointB.x);

            if ((float)nom2 / den < 0)
            {
                return float.NaN;
            }

            return (float)nom / den;
        }

        /// <summary>
        ///     Returns the intersection factor for line 1 with line 2.
        ///     The intersection factor is a distance along the line \a startPointA - \a endPointA where the line \a startPointB -
        ///     \a endPointB
        ///     intersects it.\n
        ///     \code intersectionPoint = startPointA + intersectionFactor * (endPointA-startPointA) \endcode.
        ///     Lines are treated as infinite.\n
        ///     -1 is returned if the lines are parallel (note that this is a valid return value if they are not parallel too) ///
        /// </summary>
        public static float LineIntersectionFactorXz(Vector3 startPointA, Vector3 endPointA, Vector3 startPointB, Vector3 endPointB)
        {
            Vector3 directionA = endPointA - startPointA;
            Vector3 directionB = endPointB - startPointB;

            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                return -1;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            float u = nom / den;

            return u;
        }

        /// <summary>
        ///     Returns the intersection point between the two lines. Lines are treated as infinite.
        ///     <param name="startPointA" />
        ///     is returned if the lines are parallel
        /// </summary>
        public static Vector3 LineIntersectionPointXz(Vector3 startPointA, Vector3 endPointA, Vector3 startPointB, Vector3 endPointB)
        {
            return LineIntersectionPointXz(startPointA, endPointA, startPointB, endPointB, out bool _);
        }

        /// <summary>
        ///     Returns the intersection point between the two lines.
        ///     Lines are treated as infinite.
        ///     <param name="startPointA" />
        ///     is returned if the lines are parallel
        /// </summary>
        public static Vector3 LineIntersectionPointXz(
            Vector3 startPointA,
            Vector3 endPointA,
            Vector3 startPointB,
            Vector3 endPointB,
            out bool intersects)
        {
            Vector3 directionA = endPointA - startPointA;
            Vector3 directionB = endPointB - startPointB;

            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                intersects = false;
                return startPointA;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);

            float u = nom / den;

            intersects = true;
            return startPointA + directionA * u;
        }

        /// <summary>
        ///     Returns the intersection point between the two lines. Lines are treated as infinite.
        ///     <param name="startPointA" />
        ///     is returned if the
        ///     lines are parallel ///
        /// </summary>
        public static Vector2 LineIntersectionPoint(Vector2 startPointA, Vector2 endPointA, Vector2 startPointB, Vector2 endPointB)
        {
            return LineIntersectionPoint(startPointA, endPointA, startPointB, endPointB, out bool _);
        }

        /// <summary>
        ///     Returns the intersection point between the two lines. Lines are treated as infinite.
        ///     <param name="startPointA" />
        ///     is returned if the lines are parallel
        /// </summary>
        public static Vector2 LineIntersectionPoint(
            Vector2 startPointA,
            Vector2 endPointA,
            Vector2 startPointB,
            Vector2 endPointB,
            out bool intersects)
        {
            Vector2 directionA = endPointA - startPointA;
            Vector2 directionB = endPointB - startPointB;

            float den = directionB.y * directionA.x - directionB.x * directionA.y;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                intersects = false;
                return startPointA;
            }

            float nom = directionB.x * (startPointA.y - startPointB.y) - directionB.y * (startPointA.x - startPointB.x);

            float u = nom / den;

            intersects = true;
            return startPointA + directionA * u;
        }

        /// <summary>
        ///     Returns the intersection point between the two line segments in XZ space.
        ///     Lines are NOT treated as infinite. \a startPointA is returned if the line segments do not intersect
        ///     The point will be returned along the line [startPointA, endPointA] (this matters only for the y coordinate).
        /// </summary>
        public static Vector3 SegmentIntersectionPointXZ(
            Vector3 startPointA,
            Vector3 endPointA,
            Vector3 startPointB,
            Vector3 endPointB,
            out bool intersects)
        {
            Vector3 directionA = endPointA - startPointA;
            Vector3 directionB = endPointB - startPointB;

            float den = directionB.z * directionA.x - directionB.x * directionA.z;

            if (Math.Abs(den) < MathUtils.EpsilonSingle)
            {
                intersects = false;
                return startPointA;
            }

            float nom = directionB.x * (startPointA.z - startPointB.z) - directionB.z * (startPointA.x - startPointB.x);
            float nom2 = directionA.x * (startPointA.z - startPointB.z) - directionA.z * (startPointA.x - startPointB.x);
            float u = nom / den;
            float u2 = nom2 / den;

            if (u < 0F || u > 1F || u2 < 0F || u2 > 1F)
            {
                intersects = false;
                return startPointA;
            }

            intersects = true;
            return startPointA + directionA * u;
        }

        /// <summary>
        ///     Does the line segment intersect the bounding box.
        ///     The line is NOT treated as infinite.
        /// </summary>
        public static bool SegmentIntersectsBounds(Bounds bounds, Vector3 a, Vector3 b)
        {
            // Put segment in box space
            a -= bounds.center;
            b -= bounds.center;

            // Get line midpoint and extent
            Vector3 LMid = (a + b) * 0.5F;
            Vector3 L = a - LMid;
            var LExt = new Vector3(Mathf.Abs(L.x), Mathf.Abs(L.y), Mathf.Abs(L.z));

            Vector3 extent = bounds.extents;

            // Use Separating Axis Test
            // Separation vector from box center to segment center is LMid, since the line is in box space
            if (Mathf.Abs(LMid.x) > extent.x + LExt.x)
            {
                return false;
            }

            if (Mathf.Abs(LMid.y) > extent.y + LExt.y)
            {
                return false;
            }

            if (Mathf.Abs(LMid.z) > extent.z + LExt.z)
            {
                return false;
            }

            // Crossproducts of line and each axis
            if (Mathf.Abs(LMid.y * L.z - LMid.z * L.y) > extent.y * LExt.z + extent.z * LExt.y)
            {
                return false;
            }

            if (Mathf.Abs(LMid.x * L.z - LMid.z * L.x) > extent.x * LExt.z + extent.z * LExt.x)
            {
                return false;
            }

            if (Mathf.Abs(LMid.x * L.y - LMid.y * L.x) > extent.x * LExt.y + extent.y * LExt.x)
            {
                return false;
            }

            // No separating axis, the line intersects
            return true;
        }

        /// <summary>
        ///     True if the matrix will reverse orientations of faces.
        ///     Scaling by a negative value along an odd number of axes will reverse
        ///     the orientation of e.g faces on a mesh. This must be counter adjusted
        ///     by for example the recast rasterization system to be able to handle
        ///     meshes with negative scales properly.
        ///     We can find out if they are flipped by finding out how the signed
        ///     volume of a unit cube is transformed when applying the matrix
        ///     If the (signed) volume turns out to be negative
        ///     that also means that the orientation of it has been reversed.
        ///     see https://en.wikipedia.org/wiki/Normal_(geometry)"
        ///     see https://en.wikipedia.org/wiki/Parallelepiped
        /// </summary>
        public static bool ReversesFaceOrientations(Matrix4x4 matrix)
        {
            Vector3 dX = matrix.MultiplyVector(Vector3.right);
            Vector3 dY = matrix.MultiplyVector(Vector3.up);
            Vector3 dZ = matrix.MultiplyVector(Vector3.forward);

            // Calculate the signed volume of the parallelepiped
            float volume = Vector3.Dot(Vector3.Cross(dX, dY), dZ);

            return volume < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Vector2FromRad(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Vector2FromDeg(float angle)
        {
            angle *= Mathf.Deg2Rad;
            return Vector2FromRad(angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateRad(this Vector2 vector, float radians)
        {
            float cosAngle = Mathf.Cos(radians);
            float sinAngle = Mathf.Sin(radians);

            return new Vector2(cosAngle * vector.x - sinAngle * vector.y, sinAngle * vector.x + cosAngle * vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateDeg(this Vector2 vector, float angle)
        {
            return RotateRad(vector, angle * Mathf.Deg2Rad);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleInRad(this Vector2 guide, Vector2 diverged)
        {
            return Mathf.Atan2(guide.y, guide.x) - Mathf.Atan2(diverged.y, diverged.x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle180(this Vector2 guide, Vector2 diverged)
        {
            return AngleInRad(guide, diverged) * Mathf.Rad2Deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle360(this Vector2 guide, Vector2 diverged)
        {
            return Mathf.Repeat(AngleInRad(guide, diverged), DoublePi) * Mathf.Rad2Deg;
        }
    }
}