using System;

using UnityEngine;

namespace Fp.Utility
{
    public static class QuaternionMath
    {
        public static bool IsNaN(Quaternion q)
        {
            return float.IsNaN(q.x * q.y * q.z * q.w);
        }

        public static Quaternion Normalize(Quaternion q)
        {
            double mag = Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w = (float)(q.w / mag);
            q.x = (float)(q.x / mag);
            q.y = (float)(q.y / mag);
            q.z = (float)(q.z / mag);
            return q;
        }

        /// <summary>
        ///     A cleaner version of FromToRotation, Quaternion.FromToRotation for some reason can only handle down to #.##
        ///     precision.
        ///     This will result in true 7 digits of precision down to depths of 0.00000# (depth tested so far).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Vector3 v1, Vector3 v2)
        {
            Vector3 a = Vector3.Cross(v1, v2);
            double w = Math.Sqrt(v1.sqrMagnitude * v2.sqrMagnitude) + Vector3.Dot(v1, v2);
            if (a.sqrMagnitude < Mathf.Epsilon)
            {
                //the vectors are parallel, check w to find direction
                //if w is 0 then values are opposite, and we should rotate 180 degrees around some axis
                //otherwise the vectors in the same direction and no rotation should occur
                return Math.Abs(w) < Mathf.Epsilon ? new Quaternion(0f, 1f, 0f, 0f) : Quaternion.identity;
            }

            return new Quaternion(a.x, a.y, a.z, (float)w).normalized;
        }

        public static Quaternion FromToRotation(Vector3 v1, Vector3 v2, Vector3 defaultAxis)
        {
            Vector3 a = Vector3.Cross(v1, v2);
            double w = Math.Sqrt(v1.sqrMagnitude * v2.sqrMagnitude) + Vector3.Dot(v1, v2);
            if (a.sqrMagnitude < Mathf.Epsilon)
            {
                //the vectors are parallel, check w to find direction
                //if w is 0 then values are opposite, and we should rotate 180 degrees around the supplied axis
                //otherwise the vectors in the same direction and no rotation should occur
                return Math.Abs(w) < Mathf.Epsilon ? new Quaternion(defaultAxis.x, defaultAxis.y, defaultAxis.z, 0f).normalized : Quaternion.identity;
            }

            return new Quaternion(a.x, a.y, a.z, (float)w).normalized;
        }

        /// <summary>
        ///     Get the rotation that would be applied to 'start' to end up at 'end'.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Quaternion start, Quaternion end)
        {
            return Quaternion.Inverse(start) * end;
        }

        /// <summary>
        /// </summary>
        /// <param name="from">Base rotation</param>
        /// <param name="to">Target rotation</param>
        /// <param name="angularSpeed">In degrease</param>
        /// <param name="dt">Delta time</param>
        /// <returns></returns>
        public static Quaternion SpeedSlerp(Quaternion from, Quaternion to, float angularSpeed, float dt)
        {
            float da = angularSpeed * dt;
            return Quaternion.RotateTowards(from, to, da);
        }

        public static Vector3 NormalizeEuler(Vector3 angle)
        {
            angle.x = MathUtils.NormalizeAngle(angle.x);
            angle.y = MathUtils.NormalizeAngle(angle.y);
            angle.z = MathUtils.NormalizeAngle(angle.z);
            return angle;
        }

        public static Vector3 NormalizeEulerRad(Vector3 angle)
        {
            angle.x = MathUtils.NormalizeAngleRad(angle.x);
            angle.y = MathUtils.NormalizeAngleRad(angle.y);
            angle.z = MathUtils.NormalizeAngleRad(angle.z);
            return angle;
        }

        /// <summary>
        ///     Create a LookRotation for a non-standard 'forward' axis.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="forwardAxis"></param>
        /// <returns></returns>
        public static Quaternion AltForwardLookRotation(Vector3 dir, Vector3 forwardAxis, Vector3 upAxis)
        {
            //return Quaternion.LookRotation(dir, upAxis) * Quaternion.FromToRotation(forwardAxis, Vector3.forward);
            return Quaternion.LookRotation(dir) * Quaternion.Inverse(Quaternion.LookRotation(forwardAxis, upAxis));
        }

        /// <summary>
        ///     Get the rotated forward axis based on some base forward.
        /// </summary>
        /// <param name="rot">The rotation</param>
        /// <param name="baseForward">Forward with no rotation</param>
        /// <returns></returns>
        public static Vector3 GetAltForward(Quaternion rot, Vector3 baseForward)
        {
            return rot * baseForward;
        }

        /// <summary>
        ///     Returns a rotation of up attempting to face in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static Quaternion FaceRotation(Vector3 forward, Vector3 up)
        {
            forward = VectorMath.GetForwardTangent(forward, up);
            return Quaternion.LookRotation(forward, up);
        }

        public static void GetAngleAxis(this Quaternion q, out Vector3 axis, out float angle)
        {
            if (q.w > 1)
            {
                q = Normalize(q);
            }

            //get as doubles for precision
            var qw = (double)q.w;
            var qx = (double)q.x;
            var qy = (double)q.y;
            var qz = (double)q.z;
            double ratio = Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        public static void GetShortestAngleAxisBetween(Quaternion a, Quaternion b, out Vector3 axis, out float angle)
        {
            Quaternion dq = Quaternion.Inverse(a) * b;
            if (dq.w > 1)
            {
                dq = Normalize(dq);
            }

            //get as doubles for precision
            var qw = (double)dq.w;
            var qx = (double)dq.x;
            var qy = (double)dq.y;
            var qz = (double)dq.z;
            double ratio = Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="angularVelocity">Exponential map</param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static Quaternion DeltaRotation(Vector3 angularVelocity, float deltaTime)
        {
            Vector3 ha = angularVelocity * deltaTime * 0.5f; // vector of half angle
            float l = ha.magnitude; // magnitude

            if (l > Mathf.Epsilon)
            {
                ha *= Mathf.Sin(l) / l;
                return new Quaternion(Mathf.Cos(l), ha.x, ha.y, ha.z);
            }

            return new Quaternion(1.0f, ha.x, ha.y, ha.z);
        }

        public static Vector3 AngularVelocity(Quaternion begin, Quaternion end, float deltaTime)
        {
            Quaternion q = Quaternion.Inverse(begin) * end;

            var qAxis = new Vector3(q.x, q.y, q.z);

            float len = qAxis.magnitude;
            if (len < Mathf.Epsilon)
            {
                return qAxis * 2;
            }

            float angle = 2 * Mathf.Atan2(len, q.w);
            return new Vector3(q.x, q.y, q.z) * (angle / (len * deltaTime));
        }

        public static Quaternion AngularVelocityToDerived(Quaternion current, Vector3 angularVelocity)
        {
            var spin = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0f);
            Quaternion result = spin * current;
            return new Quaternion(0.5f * result.x, 0.5f * result.y, 0.5f * result.z, 0.5f * result.w);
        }

        public static Vector3 DerivedToAngularVelocity(Quaternion current, Quaternion derived)
        {
            Quaternion result = derived * Quaternion.Inverse(current);
            return new Vector3(2f * result.x, 2f * result.y, 2f * result.z);
        }

        public static Quaternion IntegrateRotation(Quaternion rotation, Vector3 angularVelocity, float deltaTime)
        {
            if (deltaTime < Mathf.Epsilon)
            {
                return rotation;
            }

            Quaternion derived = AngularVelocityToDerived(rotation, angularVelocity);
            Vector4 pred = new Vector4(
                rotation.x + derived.x * deltaTime,
                rotation.y + derived.y * deltaTime,
                rotation.z + derived.z * deltaTime,
                rotation.w + derived.w * deltaTime
            ).normalized;
            return new Quaternion(pred.x, pred.y, pred.z, pred.w);
        }

        public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion derived, float time)
        {
            if (Time.deltaTime < Mathf.Epsilon)
            {
                return rot;
            }

            // account for double-cover
            float multi = Mathf.Sign(Quaternion.Dot(rot, target));
            target.x *= multi;
            target.y *= multi;
            target.z *= multi;
            target.w *= multi;
            // smooth damp (nlerp approx)
            Vector4 result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref derived.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref derived.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref derived.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref derived.w, time)
            ).normalized;

            // ensure derived is tangent
            Vector4 derivedError = Vector4.Project(new Vector4(derived.x, derived.y, derived.z, derived.w), result);
            derived.x -= derivedError.x;
            derived.y -= derivedError.y;
            derived.z -= derivedError.z;
            derived.w -= derivedError.w;

            return new Quaternion(result.x, result.y, result.z, result.w);
        }
    }
}