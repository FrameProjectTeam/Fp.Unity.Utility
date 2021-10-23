using UnityEngine;

namespace Fp.Utility
{
    public readonly struct BarycentricCoordinates
    {
        public BarycentricCoordinates(float u, float v, float w)
        {
            U = u;
            V = v;
            W = w;
        }

        public float U { get; }

        public float V { get; }

        public float W { get; }

        public bool IsValid => Mathf.Approximately(U + V + W, 1);

        public override string ToString()
        {
            return $"{nameof(U)}: {U}, {nameof(V)}: {V}, {nameof(W)}: {W}, {nameof(IsValid)}: {IsValid}";
        }

        public static bool FromTriangle2D(
            in Vector2 a,
            in Vector2 b,
            in Vector2 c,
            in Vector2 point,
            out BarycentricCoordinates coord)
        {
            Vector2 ab = b - a;
            Vector2 ac = c - a;
            Vector2 q = point - a;

            float cross = VectorMath.CrossProduct2D(ab, ac);

            float u = VectorMath.CrossProduct2D(q, ac) / cross;
            float v = VectorMath.CrossProduct2D(q, ab) / cross;
            float uv = u + v;

            coord = new BarycentricCoordinates(u, v, 1 - uv);
            return u >= 0 && v >= 0 && uv <= 1;
        }

        public static bool FromTriangle(
            in Vector3 a,
            in Vector3 b,
            in Vector3 c,
            in Vector3 point,
            out BarycentricCoordinates coord)
        {
            Vector3 bc = c - b;

            Vector3 nrmVector = Vector3.Cross(b - a, bc);
            float sqrLen = Vector3.SqrMagnitude(nrmVector);
            Vector3 nrm = nrmVector / sqrLen;

            float u = Vector3.Dot(Vector3.Cross(bc, point - b), nrm);
            float v = Vector3.Dot(Vector3.Cross(a - c, point - c), nrm);
            float uv = u + v;
            float w = 1 - uv;

            coord = new BarycentricCoordinates(u, v, w);
            return u >= 0 && v >= 0 && uv <= 1;
        }
    }
}