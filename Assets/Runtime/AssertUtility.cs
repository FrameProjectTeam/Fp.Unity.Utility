using UnityEngine;
using UnityEngine.Assertions;

namespace Fp.Utility
{
    public static class AssertUtility
    {
        public static void IsVectorValid(Vector2 value, string paramName = nameof(Vector2))
        {
            Assert.IsTrue(MathUtils.IsReal(value.x), $"IsReal({paramName}.x)");
            Assert.IsTrue(MathUtils.IsReal(value.y), $"IsReal({paramName}.y)");
        }

        public static void IsVectorValid(Vector3 value, string paramName = nameof(Vector3))
        {
            Assert.IsTrue(MathUtils.IsReal(value.x), $"IsReal({paramName}.x)");
            Assert.IsTrue(MathUtils.IsReal(value.y), $"IsReal({paramName}.y)");
            Assert.IsTrue(MathUtils.IsReal(value.z), $"IsReal({paramName}).z");
        }
    }
}