using System;

using UnityEngine.Assertions;

namespace Fp.Utility
{
    public static class ShootingUtility
    {
        /// <summary>
        ///     Calculate spread radius based on accuracy percentage
        /// </summary>
        /// <param name="accuracy">Accuracy in percentage [0, 100]%</param>
        /// <param name="spreadAngle">Spread angle in radiance</param>
        public static void AccuracyToSpreadAngle(float accuracy, out float spreadAngle)
        {
            spreadAngle = (1.0f - accuracy) * 90.0f * MathUtils.DegToRad;
        }

        /// <summary>
        ///     Calculate spread radius based on accuracy percentage
        /// </summary>
        /// <param name="accuracy">Accuracy in percentage [0, 100]%</param>
        /// <returns>Spread radius in radiance</returns>
        public static float AccuracyToSpreadAngle(float accuracy)
        {
            AccuracyToSpreadAngle(accuracy, out float spreadAngle);
            return spreadAngle;
        }

        /// <summary>
        ///     Calculate best shooting distance based on accuracy and target radius
        /// </summary>
        /// <param name="radius">Radius of target, must be grater than zero</param>
        /// <param name="accuracy">Percentage of accuracy in range [0, 1], where 0 - 0% and 1 - 100%</param>
        /// <param name="distance">Distance result</param>
        public static void AccuracyDistance(float radius, float accuracy, out float distance)
        {
            Assert.IsTrue(radius > 0, $"{nameof(radius)} > 0, {radius}");
            Assert.IsTrue(accuracy >= 0, $"{nameof(accuracy)} >= 0, {accuracy}");
            Assert.IsTrue(accuracy <= 1, $"{nameof(accuracy)} <= 1, {accuracy}");

            SpreadDistance(AccuracyToSpreadAngle(accuracy), radius, out distance);
        }

        /// <summary>
        ///     Calculate best shooting distance based on spread radius and target radius
        /// </summary>
        /// <param name="spreadAngle">Spread angle in radians</param>
        /// <param name="targetRadius">Radius of target in meters, must be grater than zero</param>
        /// <param name="distance">Distance result</param>
        public static void SpreadDistance(float spreadAngle, float targetRadius, out float distance)
        {
            Assert.IsTrue(targetRadius > 0, $"{nameof(targetRadius)} > 0, {targetRadius}");
            Assert.IsTrue(spreadAngle >= 0, $"{nameof(spreadAngle)} >= 0, {spreadAngle}");
            Assert.IsTrue(spreadAngle <= MathUtils.Pi2D, $"{nameof(spreadAngle)} <= Pi/2, {spreadAngle}");

            distance = (float)(targetRadius / Math.Sin(spreadAngle));
        }

        /// <summary>
        ///     Calculate spread radius in distance based on spread angle
        /// </summary>
        /// <param name="spreadAngle">Spread angle in radians</param>
        /// <param name="distance">Target distance</param>
        /// <param name="spreadRadius">Spread radius in the distance</param>
        public static void SpreadByDistance(float spreadAngle, float distance, out float spreadRadius)
        {
            Assert.IsTrue(distance >= 0, $"{nameof(distance)} >= 0, {distance}");
            Assert.IsTrue(spreadAngle >= 0, $"{nameof(spreadAngle)} >= 0, {spreadAngle}");
            Assert.IsTrue(spreadAngle <= MathUtils.Pi2D, $"{nameof(spreadAngle)} <= Pi/2, {spreadAngle}");

            spreadRadius = (float)(distance * Math.Sin(spreadAngle));
        }
    }
}