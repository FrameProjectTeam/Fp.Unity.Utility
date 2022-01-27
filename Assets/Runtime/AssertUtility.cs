using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Utility
{
    public static class AssertUtility
    {
        public static void IsVectorReal(Vector2 value, string paramName = nameof(Vector2))
        {
            Assert.IsTrue(value.IsReal(), $"{paramName}.{nameof(VectorMath.IsReal)}()");
        }

        public static void IsVectorReal(Vector3 value, string paramName = nameof(Vector3))
        {
            Assert.IsTrue(value.IsReal(), $"{paramName}.{nameof(VectorMath.IsReal)}()");
        }

        public static void IsNormalized(Vector3 value, string paramName = nameof(Vector3))
        {
            IsVectorReal(value);
            Assert.IsTrue(value.IsNormalized(), $"{paramName}.{nameof(VectorMath.IsNormalized)}()");
        }
        
        public static void IsNormalized(Vector2 value, string paramName = nameof(Vector3))
        {
            IsVectorReal(value);
            Assert.IsTrue(value.IsNormalized(), $"{paramName}.{nameof(VectorMath.IsNormalized)}()");
        }
    }
}