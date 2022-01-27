#define TWO_CALLS_MODULUS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.Assertions;

using Random = System.Random;

namespace Fp.Utility
{
    public static class MathUtils
    {
        /// <summary>
        ///     Number Pi
        /// </summary>
        public const float Pi = 3.14159265358979f;

        /// <summary>
        ///     Number Pi (double precision)
        /// </summary>
        public const double PiD = Math.PI;

        /// <summary>
        ///     Pi/2 OR 90 deg
        /// </summary>
        public const float Pi2 = Pi / 2f;

        /// <summary>
        ///     Pi/2 OR 90 deg
        /// </summary>
        public const double Pi2D = PiD / 2.0;

        /// <summary>
        ///     Pi/2 OR 60 deg
        /// </summary>
        public const float Pi3 = 1.04719755119659666667f;

        /// <summary>
        ///     Pi/4 OR 45 deg
        /// </summary>
        public const float Pi4 = 0.785398163397448f;

        /// <summary>
        ///     Pi/8 OR 22.5 deg
        /// </summary>
        public const float Pi8 = 0.392699081698724f;

        /// <summary>
        ///     Pi/16 OR 11.25 deg
        /// </summary>
        public const float Pi16 = 0.196349540849362f;

        /// <summary>
        ///     2Pi OR 180 deg
        /// </summary>
        public const float TwoPi = 6.28318530717959f;

        /// <summary>
        ///     3 * Pi/2 OR 270 deg
        /// </summary>
        public const float ThreePi2 = 4.71238898038469f;

        /// <summary>
        ///     Number e
        /// </summary>
        public const float E = 2.71828182845905f;

        /// <summary>
        ///     ln(10)
        /// </summary>
        public const float Ln10 = 2.30258509299405f;

        /// <summary>
        ///     ln(2)
        /// </summary>
        public const float Ln2 = 0.693147180559945f;

        /// <summary>
        ///     logB10(e)
        /// </summary>
        public const float Log10E = 0.434294481903252f;

        /// <summary>
        ///     logB2(e)
        /// </summary>
        public const float Log2E = 1.44269504088896f;

        /// <summary>
        ///     sqrt( 1 / 2 )
        /// </summary>
        public const float Sqrt12 = 0.707106781186548f;

        /// <summary>
        ///     sqrt( 2 )
        /// </summary>
        public const float Sqrt2 = 1.4142135623731f;

        /// <summary>
        ///     PI / 180
        /// </summary>
        public const float DegToRad = Pi / 180f;

        /// <summary>
        ///     PI / 180 (double precision)
        /// </summary>
        public const double DegToRadD = PiD / 180.0;

        /// <summary>
        ///     180.0 / PI
        /// </summary>
        public const float RadToDeg = 180f / Pi;

        /// <summary>
        ///     180.0 / PI (double precision)
        /// </summary>
        public const double RadToDegD = 180.0 / PiD;

        /// <summary>
        ///     2^16
        /// </summary>
        public const int B16 = 65536;

        /// <summary>
        ///     2^31
        /// </summary>
        public const long B31 = 2147483648L;

        /// <summary>
        ///     2^32
        /// </summary>
        public const long B32 = 4294967296L;

        /// <summary>
        ///     2^48
        /// </summary>
        public const long B48 = 281474976710656L;

        /// <summary>
        ///     2^53 !!NOTE!! largest accurate double floating point whole value
        /// </summary>
        public const long B53 = 9007199254740992L;

        /// <summary>
        ///     2^63
        /// </summary>
        public const ulong B63 = 9223372036854775808;

        /// <summary>
        ///     18446744073709551615 or 2^64 - 1 or ULong.MaxValue...
        /// </summary>
        public const ulong B64M1 = ulong.MaxValue;

        /// <summary>
        ///     1.0/3.0
        /// </summary>
        public const float OneThird = 0.333333333333333f;

        /// <summary>
        ///     2.0/3.0
        /// </summary>
        public const float TwoThirds = 0.666666666666667f;

        /// <summary>
        ///     1.0/6.0
        /// </summary>
        public const float OneSixth = 0.166666666666667f;

        /// <summary>
        ///     Cos( PI / 3 )
        /// </summary>
        public const float CosPi3 = 0.866025403784439f;

        /// <summary>
        ///     Sin( 2*PI/3 )
        /// </summary>
        public const float Sin2Pi3 = 0.03654595f;

        /// <summary>
        ///     4*(Math.Sqrt(2)-1)/3.0
        /// </summary>
        public const float CircleAlpha = 0.552284749830793f;

        public const bool On = true;

        public const bool Off = false;

        /// <summary>
        ///     round integer epsilon
        /// </summary>
        public const float ShortEpsilon = 0.1f;

        /// <summary>
        ///     percentage epsilon
        /// </summary>
        public const float PctEpsilon = 0.001f;

        /// <summary>
        ///     Represents the machine precision for single-precision floating-point numbers.
        /// </summary>
        public const float EpsilonSingle = 1.192093E-07f;

        /// <summary>
        ///     Represents the machine precision for double-precision floating-point numbers.
        /// </summary>
        public const double EpsilonDouble = 2.22044604925031E-16;

        /// <summary>
        ///     Represents the smallest single-precision floating-point number that is greater than zero.
        /// </summary>
        public const float MinSingle = 1.175494E-38f;

        public const float MinNegativeSingle = -EpsilonSingle;

        /// <summary>
        ///     Represents the smallest double-precision floating-point number that is greater than zero.
        /// </summary>
        public const double MinDouble = 4.94065645841247E-324;

        public const double MinNegativeDouble = -MinDouble;

        public static readonly float MachineSngEpsilon = ComputeMachineEpsilon();

        public static bool IsReal(this float f)
        {
            return !float.IsNaN(f) && !float.IsNegativeInfinity(f) && !float.IsPositiveInfinity(f);
        }

        public static bool IsReal(this double f)
        {
            return !double.IsNaN(f) && !double.IsNegativeInfinity(f) && !double.IsPositiveInfinity(f);
        }

        public static bool IsPowerOfTwo(this ulong value)
        {
            return value != 0 && (value & (value - 1)) == 0;
        }

        /// <summary>
        ///     roundTo some place comparative to a 'base', default is 10 for decimal place
        ///     'place' is represented by the power applied to 'base' to get that place
        /// </summary>
        /// <param name="value">the value to round</param>
        /// <param name="place">the place to round to</param>
        /// <param name="numericSystem">the base to round in... default is 10 for decimal</param>
        /// <returns>The value rounded</returns>
        /// <remarks>
        ///     e.g.
        ///     2000/7 ~= 285.714285714285714285714 ~= (bin)100011101.1011011011011011
        ///     roundTo(2000/7,-3) == 0
        ///     roundTo(2000/7,-2) == 300
        ///     roundTo(2000/7,-1) == 290
        ///     roundTo(2000/7,0) == 286
        ///     roundTo(2000/7,1) == 285.7
        ///     roundTo(2000/7,2) == 285.71
        ///     roundTo(2000/7,3) == 285.714
        ///     roundTo(2000/7,4) == 285.7143
        ///     roundTo(2000/7,5) == 285.71429
        ///     roundTo(2000/7,-3,2)  == 288       -- 100100000
        ///     roundTo(2000/7,-2,2)  == 284       -- 100011100
        ///     roundTo(2000/7,-1,2)  == 286       -- 100011110
        ///     roundTo(2000/7,0,2)  == 286       -- 100011110
        ///     roundTo(2000/7,1,2) == 285.5     -- 100011101.1
        ///     roundTo(2000/7,2,2) == 285.75    -- 100011101.11
        ///     roundTo(2000/7,3,2) == 285.75    -- 100011101.11
        ///     roundTo(2000/7,4,2) == 285.6875  -- 100011101.1011
        ///     roundTo(2000/7,5,2) == 285.71875 -- 100011101.10111
        ///     note what occurs when we round to the 3rd space (8ths place), 100100000, this is to be assumed
        ///     because we are rounding 100011.1011011011011011 which rounds up.
        /// </remarks>
        public static float RoundTo(float value, int place = 0, uint numericSystem = 10)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return (float)Math.Round(value);
            }

            if (numericSystem == 10 && place > 0 && place <= 15)
            {
                //'Math.Round has a rounding to decimal spaces that is very efficient
                //'only useful for base 10 if places are from 1 to 15
                return (float)Math.Round(value, place);
            }

            var p = (float)Math.Pow(numericSystem, place);
            return (float)Math.Round(value * p) / p;
        }

        /// <summary>
        ///     FloorTo some place comparative to a 'base', default is 10 for decimal place
        ///     'place' is represented by the power applied to 'base' to get that place
        /// </summary>
        /// <param name="value"></param>
        /// <param name="place"></param>
        /// <param name="numericSystem"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float FloorTo(float value, int place = 0, uint numericSystem = 10)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return (float)Math.Floor(value);
            }

            var p = (float)Math.Pow(numericSystem, place);
            return (float)Math.Floor(value * p) / p;
        }

        /// <summary>
        ///     CeilTo some place comparative to a 'base', default is 10 for decimal place
        ///     'place' is represented by the power applied to 'base' to get that place
        /// </summary>
        /// <param name="value"></param>
        /// <param name="place"></param>
        /// <param name="numericSystem"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float CeilTo(float value, int place = 0, uint numericSystem = 10)
        {
            if (place == 0)
            {
                //'if zero no reason going through the math hoops
                return (float)Math.Ceiling(value);
            }

            var p = (float)Math.Pow(numericSystem, place);
            return (float)Math.Ceiling(value * p) / p;
        }

        /// <summary>
        ///     Calculates the integral part of a float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Truncate(float value)
        {
            return (float)Math.Truncate(value);
        }

        /// <summary>
        ///     Rounds to the nearest interval. This can allow you to round to the nearest repeating range, for instance every 45
        ///     degrees for an angle.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static float RoundToInterval(float value, float interval)
        {
            if (interval < EpsilonSingle && interval > -EpsilonSingle)
            {
                return value;
            }

            return (float)Math.Round(value / interval) * interval;
        }

        /// <summary>
        ///     Test if float is kind of equal to some other value by some epsilon.
        ///     Due to float error, two values may be considered similar... but the computer considers them different.
        ///     By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyEqual(float a, float b, float epsilon = EpsilonSingle)
        {
            return Math.Abs(a - b) < epsilon;
        }

        /// <summary>
        ///     Test if float is less than some other value by some degree of error in epsilon.
        ///     Due to float error, two values may be considered similar... but the computer considers them different.
        ///     By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyLessThan(float a, float b, float epsilon, bool exclusive)
        {
            //exclusive means we prefer to easily exclude a true result
            if (exclusive)
            {
                return a < b - epsilon;
            }

            return a < b + epsilon;
        }

        public static bool FuzzyLessThan(float a, float b, bool exclusive)
        {
            return FuzzyLessThan(a, b, EpsilonSingle, exclusive);
        }

        /// <summary>
        ///     Test if float is less than some other value by some degree of error in epsilon.
        ///     Due to float error, two values may be considered similar... but the computer considers them different.
        ///     By using some epsilon (degree of error) once can test if the two values are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool FuzzyGreaterThan(float a, float b, float epsilon, bool exclusive)
        {
            //exclusive means we prefer to easily exclude a true result
            if (exclusive)
            {
                return a > b + epsilon;
            }

            return a > b - epsilon;
        }

        public static bool FuzzyGreaterThan(float a, float b, bool exclusive)
        {
            return FuzzyGreaterThan(a, b, EpsilonSingle, exclusive);
        }

        /// <summary>
        ///     Test if a value is near some target value, if with in some range of 'epsilon', the target is returned.
        ///     eg:
        ///     Slam(1.52,2,0.1) == 1.52
        ///     Slam(1.62,2,0.1) == 1.62
        ///     Slam(1.72,2,0.1) == 1.72
        ///     Slam(1.82,2,0.1) == 1.82
        ///     Slam(1.92,2,0.1) == 2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Slam(float value, float target, float epsilon = EpsilonSingle)
        {
            return Math.Abs(value - target) < epsilon ? target : value;
        }

        /// <summary>
        ///     convert radians to degrees
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float RadiansToDegrees(float angle)
        {
            return angle * RadToDeg;
        }

        /// <summary>
        ///     convert degrees to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float DegreesToRadians(float angle)
        {
            return angle * DegToRad;
        }

        /// <summary>
        ///     Find the angle in radians of a segment from (x1, y1) -> (x2, y2 )
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float AngleBetween(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Atan2(y2 - y1, x2 - x1);
        }

        /// <summary>
        ///     Wraps a value around some significant range.
        ///     Similar to modulo, but works in a unary direction over any range (including negative values).
        ///     ex:
        ///     Wrap(8,6,2) == 4
        ///     Wrap(4,2,0) == 0
        ///     Wrap(4,2,-2) == 0
        /// </summary>
        /// <param name="value">value to wrap</param>
        /// <param name="min">min in range</param>
        /// <param name="max">max in range</param>
        /// <returns>A value wrapped around min to max</returns>
        /// <remarks></remarks>
        public static int Wrap(int value, int min, int max)
        {
            max -= min;
            if (max == 0)
            {
                return min;
            }

            return value - max * (int)Math.Floor((double)(value - min) / max);
        }

        public static long Wrap(long value, long min, long max)
        {
            max -= min;
            if (max == 0)
            {
                return min;
            }

            return value - max * (long)Math.Floor((double)(value - min) / max);
        }

        public static float Wrap(float value, float min, float max)
        {
            max -= min;
            if (Math.Abs(max) < EpsilonSingle)
            {
                return min;
            }

            return value - max * (float)Math.Floor((value - min) / max);
        }

        /// <summary>
        ///     set an angle with in the bounds of -PI to PI
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NormalizeAngle(float angle)
        {
            const float rd = 180;
            return Wrap(angle, -rd, rd);
        }

        public static float NormalizeAngleRad(float angle)
        {
            const float rd = Pi;
            return Wrap(angle, -rd, rd);
        }

        /// <summary>
        ///     closest angle from a1 to a2
        ///     absolute value the return for exact angle
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NearestAngleBetween(float a1, float a2)
        {
            const float rd = 180f;
            float ra = Wrap(a2 - a1, 0, rd * 2f);
            if (ra > rd)
            {
                ra -= rd * 2f;
            }

            return ra;
        }

        /// <summary>
        ///     closest angle from a1 to a2
        ///     absolute value the return for exact angle
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float NearestAngleRadBetween(float a1, float a2)
        {
            const float rd = Pi;
            float ra = Wrap(a2 - a1, 0, rd * 2f);
            if (ra > rd)
            {
                ra -= rd * 2f;
            }

            return ra;
        }

        /// <summary>
        ///     Returns a value for dependant that is a value that is the shortest angle between dep and ind from ind.
        ///     for instance if dep=-190 degrees and ind=10 degrees then 170 degrees will be returned
        ///     since the shortest path from 10 to -190 is to rotate to 170.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float ShortenAngleToAnother(float dep, float ind)
        {
            return ind + NearestAngleBetween(ind, dep);
        }

        /// <summary>
        ///     Returns a value for dependant that is a value that is the shortest angle between dep and ind from ind.
        ///     for instance if dep=-190 degrees and ind=10 degrees then 170 degrees will be returned
        ///     since the shortest path from 10 to -190 is to rotate to 170.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float ShortenAngleRadToAnother(float dep, float ind)
        {
            return ind + NearestAngleRadBetween(ind, dep);
        }

        /// <summary>
        ///     Returns a value for dependant that is the shortest angle in the positive direction from ind.
        ///     for instance if dep=-170 degrees, and ind=10 degrees, then 190 degrees will be returned as an alternative to -170.
        ///     Since 190 is the smallest angle > 10 equal to -170.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        public static float NormalizeAngleToAnother(float dep, float ind)
        {
            const float div = 360f;
            float v = (dep - ind) / div;
            return dep - (float)Math.Floor(v) * div;
        }

        /// <summary>
        ///     Returns a value for dependant that is the shortest angle in the positive direction from ind.
        ///     for instance if dep=-170 degrees, and ind=10 degrees, then 190 degrees will be returned as an alternative to -170.
        ///     Since 190 is the smallest angle > 10 equal to -170.
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="ind"></param>
        /// <returns></returns>
        public static float NormalizeAngleRadToAnother(float dep, float ind)
        {
            const float div = TwoPi;
            float v = (dep - ind) / div;
            return dep - (float)Math.Floor(v) * div;
        }

        /// <summary>
        ///     interpolate across the shortest arc between two angles
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float InterpolateAngle(float a, float b, float weight)
        {
            const float rd = 180f;
            float delta = (b - a) % (rd * 2f);
            return Wrap(a + delta * weight, rd, -rd);
        }

        /// <summary>
        ///     interpolate across the shortest arc between two angles
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float InterpolateAngleRad(float a, float b, float weight)
        {
            const float rd = Pi;
            float delta = (b - a) % (rd * 2f);
            return Wrap(a + delta * weight, rd, -rd);
        }

        /// <summary>
        ///     Compute the logarithm of any value of any base
        /// </summary>
        /// <param name="value"></param>
        /// <param name="base"></param>
        /// <returns></returns>
        /// <remarks>
        ///     a logarithm is the exponent that some constant (base) would have to be raised to
        ///     to be equal to value.
        ///     i.e.
        ///     4 ^ x = 16
        ///     can be rewritten as to solve for x
        ///     logB4(16) = x
        ///     which with this function would be
        ///     LoDMath.logBaseOf(16,4)
        ///     which would return 2, because 4^2 = 16
        /// </remarks>
        public static float LogBaseOf(float value, float @base)
        {
            return (float)(Math.Log(value) / Math.Log(@base));
        }

        /// <summary>
        ///     Check if a value is prime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        ///     In this method to increase speed we first check if the value is ltOReq 1, because values ltOReq 1 are not prime by
        ///     definition.
        ///     Then we check if the value is even but not equal to 2. If so the value is most certainly not prime.
        ///     Lastly we loop through all odd divisors. No point in checking 1 or even divisors, because if it were divisible by
        ///     an even
        ///     number it would be divisible by 2. If any divisor existed when i > value / i then its compliment would have already
        ///     been located. And lastly the loop will never reach i == val because i will never be > sqrt(val).
        ///     proof of validity for algorithm:
        ///     all trivial values are thrown out immediately by checking if even or less then 2
        ///     all remaining possibilities MUST be odd, an odd is resolved as the multiplication of 2 odd values only. (even *
        ///     anyValue == even)
        ///     in resolution a * b = val, a = val / b. As every compliment a for b, b and a can be swapped resulting in b being
        ///     ltOReq a. If a compliment for b
        ///     exists then that compliment would have already occured (as it is odd) in the swapped addition at the even split.
        ///     Example...
        ///     16
        ///     1 * 16
        ///     2 * 8
        ///     4 * 4
        ///     8 * 2
        ///     16 * 1
        ///     checks for 1, 2, and 4 would have already checked the validity of 8 and 16.
        ///     Thusly we would only have to loop as long as i ltOReq val / i. Once we've reached the middle compliment, all
        ///     subsequent factors have been resolved.
        ///     This shrinks the number of loops for odd values from [ floor(val / 2) - 1 ] down to [ ceil(sqrt(val) / 2) - 1 ]
        ///     example, if we checked EVERY odd number for the validity of the prime 7927, we'd loop 3962 times
        ///     but by this algorithm we loop only 43 times. Significant improvement!
        /// </remarks>
        public static bool IsPrime(long value)
        {
            // check if value is in prime number range
            if (value < 2)
            {
                return false;
            }

            // check if even, but not equal to 2
            if ((value % 2 == 0) & (value != 2))
            {
                return false;
            }

            // if 2 or odd, check if any non-trivial divisors exist
            var sqrrt = (long)Math.Floor(Math.Sqrt(value));
            for (long i = 3; i <= sqrrt; i += 2)
            {
                if (value % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Relative Primality between two integers
        ///     By definition two integers are considered relatively prime if their
        ///     'greatest common divisor' is 1. So thusly we simply just check if
        ///     the GCD of m and n is 1.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsRelativelyPrime(short m, short n)
        {
            return Gcd(m, n) == 1;
        }

        public static bool IsRelativelyPrime(int m, int n)
        {
            return Gcd(m, n) == 1;
        }

        public static bool IsRelativelyPrime(long m, long n)
        {
            return Gcd((int)m, (int)n) == 1;
        }

        public static int[] DividersOf(int value)
        {
            value = Math.Abs(value);
            var arr = new List<int>();
            var sqrrt = (int)Math.Sqrt(value);
            var c = 0;

            for (var i = 1; i <= sqrrt; i++)
            {
                if (value % i == 0)
                {
                    arr.Add(i);
                    c = value / i;
                    if (c != i)
                    {
                        arr.Add(c);
                    }
                }
            }

            arr.Sort();

            return arr.ToArray();
        }

        public static int[] CommonDividersOf(int m, int n)
        {
            var i = 0;
            var j = 0;
            if (m < 0)
            {
                m = -m;
            }

            if (n < 0)
            {
                n = -n;
            }

            if (m > n)
            {
                i = m;
                m = n;
                n = i;
            }

            var set = new HashSet<int>(); //ensures no duplicates

            var r = (int)Math.Sqrt(m);
            for (i = 1; i <= r; i++)
            {
                if (m % i == 0 && n % i == 0)
                {
                    set.Add(i);
                    j = m / i;
                    if (n % j == 0)
                    {
                        set.Add(j);
                    }

                    j = n / i;
                    if (m % j == 0)
                    {
                        set.Add(j);
                    }
                }
            }

            int[] arr = set.ToArray();
            Array.Sort(arr);
            return arr;
        }

        /// <summary>
        ///     Greatest Common Divisor using Euclid's algorithm
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int Gcd(int m, int n)
        {
            var r = 0;

            // make sure positive, GCD is always positive
            if (m < 0)
            {
                m = -m;
            }

            if (n < 0)
            {
                n = -n;
            }

            // m must be >= n
            if (m < n)
            {
                r = m;
                m = n;
                n = r;
            }

            // now start loop, loop is infinite... we will cancel out sooner or later
            while (true)
            {
                r = m % n;
                if (r == 0)
                {
                    return n;
                }

                m = n;
                n = r;
            }

            // fail safe
            //return 1;
        }

        public static long Gcd(long m, long n)
        {
            long r = 0;

            // make sure positive, GCD is always positive
            if (m < 0)
            {
                m = -m;
            }

            if (n < 0)
            {
                n = -n;
            }

            // m must be >= n
            if (m < n)
            {
                r = m;
                m = n;
                n = r;
            }

            // now start loop, loop is infinite... we will cancel out sooner or later
            while (true)
            {
                r = m % n;
                if (r == 0)
                {
                    return n;
                }

                m = n;
                n = r;
            }

            // fail safe
            //return 1;
        }

        /// <summary>
        ///     Least common multiple
        /// </summary>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Lcm(int m, int n)
        {
            return m * n / Gcd(m, n);
        }

        /// <summary>
        ///     Factorial - N!
        ///     Simple product series
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        ///     By definition 0! == 1
        ///     Factorial assumes the idea that the value is an integer >= 0... thusly UInteger is used
        /// </remarks>
        public static long Factorial(uint value)
        {
            if (value <= 0)
            {
                return 1;
            }

            long res = value;

            while (--value != 0)
            {
                res *= value;
            }

            return res;
        }

        /// <summary>
        ///     Falling facotiral
        ///     defined: (N)! / (N - x)!
        ///     written subscript: (N)x OR (base)exp
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long FallingFactorial(uint n, uint x)
        {
            return Factorial(n) / Factorial(n - x);
        }

        /// <summary>
        ///     rising factorial
        ///     defined: (N + x - 1)! / (N - 1)!
        ///     written superscript N^(x) OR base^(exp)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long RisingFactorial(uint n, uint x)
        {
            return Factorial(n + x - 1) / Factorial(n - 1);
        }

        /// <summary>
        ///     binomial coefficient
        ///     defined: N! / (k!(N-k)!)
        ///     reduced: N! / (N-k)! == (N)k (fallingfactorial)
        ///     reduced: (N)k / k!
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long BinomialCoefficient(uint n, uint k)
        {
            Assert.IsTrue(n >= k); // >=0

            return FallingFactorial(n, k) / Factorial(k);
        }

        /// <summary>
        ///     rising binomial coefficient
        ///     as one can notice in the analysis of binCoef(...) that
        ///     binCoef is the (N)k divided by k!. Similarly rising binCoef
        ///     is merely N^(k) / k!
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long RisingBinomialCoefficient(uint n, uint k)
        {
            return RisingFactorial(n, k) / Factorial(k);
        }

        public static float ApproxCircumOfEllipse(float a, float b)
        {
            return (float)(Pi * Math.Sqrt((a * a + b * b) / 2));
        }

        /// <summary>
        ///     Returns the fractional part of a float.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static float Shear(float value)
        {
            return value % 1.0f;
        }

        /// <summary>
        ///     Returns if the value is in between or equal to max and min
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InRange(float value, float min, float max)
        {
            if (max < min)
            {
                return value >= max && value <= min;
            }

            return value >= min && value <= max;
        }

        /// <summary>
        ///     Returns if the value is in between or equal to max and min
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRange(int value, int min, int max)
        {
            if (max < min)
            {
                return value >= max && value <= min;
            }

            return value >= min && value <= max;
        }

        public static bool InRangeExclusive(float value, float min, float max)
        {
            if (max < min)
            {
                return value > max && value < min;
            }

            return value > min && value < max;
        }

        /// <summary>
        ///     Returns if the value is a valid index in list of length 'bound'.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bound">Higher limit exclusively</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool InBounds(int value, int bound)
        {
            return value >= 0 && value < bound;
        }

        /// <summary>
        ///     Clamp with generic argument.
        ///     Clamps a value between a minimum and maximum values.
        ///     Minimum must be less than maximum
        /// </summary>
        /// <param name="value">Input value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <typeparam name="T">This type must be inherited from <see cref="IComparable{T}" /></typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            Assert.IsTrue(min.CompareTo(max) <= 0, "min <= max");

            if (value.CompareTo(min) < 0)
            {
                value = min;
            }
            else if (value.CompareTo(max) > 0)
            {
                value = max;
            }

            return value;
        }

        /// <summary>
        ///     Clamp value of generic arguments.
        ///     Clamps a value between two passed arguments.
        /// </summary>
        /// <param name="value">Input value</param>
        /// <param name="argA">First clamp value</param>
        /// <param name="argB">Second clamp value</param>
        /// <typeparam name="T">This type must be inherited from <see cref="IComparable{T}" /></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SafeClamp<T>(T value, T argA, T argB)
            where T : IComparable<T>
        {
            return argA.CompareTo(argB) < 0 ? Min(Max(value, argA), argB) : Min(Max(value, argB), argA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(T argA, T argB)
            where T : IComparable<T>
        {
            return argA.CompareTo(argB) < 0 ? argB : argA;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(T argA, T argB)
            where T : IComparable<T>
        {
            return argA.CompareTo(argB) < 0 ? argA : argB;
        }

        /// <summary>
        ///     Maps a value from some arbitrary range to the 0 to 1 range and clamp it
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map01Clamped(float value, float min, float max)
        {
            return Mathf.Clamp01(Map01(value, min, max));
        }

        /// <summary>
        ///     Maps a value from some arbitrary range to the 0 to 1 range
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map01(float value, float min, float max)
        {
			Assert.IsTrue(max > min, $"max({max}) > min({min})");
            return (value - min) / (max - min);
        }

        /// <summary>
        ///     Convert input value from left range to right range
        /// </summary>
        /// <param name="value">Value from left range</param>
        /// <param name="leftMin">Left minimum</param>
        /// <param name="leftMax">Left maximum</param>
        /// <param name="rightMin">Right minimum.</param>
        /// <param name="rightMax">Right maximum</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(
            float value,
            float leftMin,
            float leftMax,
            float rightMin,
            float rightMax)
        {
            return rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
        }

        /// <summary>
        ///     Convert input value from left range to right range and loops in this range
        /// </summary>
        /// <param name="value">Value from left range</param>
        /// <param name="leftMin">Left minimum</param>
        /// <param name="leftMax">Left maximum</param>
        /// <param name="rightMin">Right minimum.</param>
        /// <param name="rightMax">Right maximum</param>
        /// <returns>Value in right range</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RepeatMap(
            float value,
            float leftMin,
            float leftMax,
            float rightMin,
            float rightMax)
        {
            float rightLen = rightMax - rightMin;
            return rightMin + (value - leftMin) * rightLen / (leftMax - leftMin) % rightLen;
        }

        /// <summary>
        ///     Convert input value from left range to right range
        ///     And clamp this value to right range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftMin"></param>
        /// <param name="leftMax"></param>
        /// <param name="rightMin"></param>
        /// <param name="rightMax"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MapClamped(
            float value,
            float leftMin,
            float leftMax,
            float rightMin,
            float rightMax)
        {
            Assert.IsTrue(rightMin <= rightMax, $"{nameof(rightMin)} <= {nameof(rightMax)}");

            return Mathf.Clamp(Map(value, leftMin, leftMax, rightMin, rightMax), rightMin, rightMax);
        }

        /// <summary>
        ///     Convert input value from left range to right range
        ///     And clamp this value to right range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftMin"></param>
        /// <param name="leftMax"></param>
        /// <param name="rightMin"></param>
        /// <param name="rightMax"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MapSafeClamped(
            float value,
            float leftMin,
            float leftMax,
            float rightMin,
            float rightMax)
        {
            return SafeClamp(Map(value, leftMin, leftMax, rightMin, rightMax), rightMin, rightMax);
        }

        /// <summary>
        ///     Convert input value from left range to integer right range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="leftMin"></param>
        /// <param name="leftMax"></param>
        /// <param name="rightMin"></param>
        /// <param name="rightMax"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MapToInt(
            float value,
            float leftMin,
            float leftMax,
            int rightMin,
            int rightMax)
        {
            return Mathf.RoundToInt(Map(value, leftMin, leftMax, rightMin, rightMax));
        }

        /// <summary>
        ///     Shuffle the array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="array">Array to shuffle.</param>
        /// <param name="random"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(T[] array, Random random)
        {
            int n = array.Length;
            for (var i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(random.NextDouble() * (n - i));
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        /// <summary>
        ///     Shuffle the array.
        /// </summary>
        /// <typeparam name="T">Array element type.</typeparam>
        /// <param name="list">Array to shuffle.</param>
        /// <param name="random"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(List<T> list, Random random)
        {
            int n = list.Count;
            for (var i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(random.NextDouble() * (n - i));
                T t = list[r];
                list[r] = list[i];
                list[i] = t;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Remaind(ushort a, ushort b)
        {
            return (ushort)(a - a / b * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Fibonacci(int n)
        {
            var a = 0;
            var b = 1;
            // In N steps compute Fibonacci sequence iteratively.
            for (var i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }

            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GeometricSequence(int n, float initialValue, float ratio)
        {
            return initialValue * Pow(ratio, n - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GeometricSequenceFirst(int n, float initialValue, float ratio)
        {
            return initialValue * (1 - Pow(ratio, n)) / (1 - ratio);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GeometricSequenceInverse(float progressionValue, float initialValue, float ratio)
        {
            return (float)((Math.Log(progressionValue) - Math.Log(initialValue)) / Math.Log(ratio)) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GeometricSequenceFirstInverse(float value, float initialValue, float ratio)
        {
            return (float)Math.Log(-((value * (1 - ratio) - initialValue) / initialValue), ratio);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceFoV(float fov, float height)
        {
            return height / 2 * Mathf.Tan((90 - fov / 2) * Mathf.Deg2Rad);
        }

        public static float Pow(float val, int exp)
        {
            var result = 1.0f;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                {
                    result *= val;
                }

                exp >>= 1;
                val *= val;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mod(this int a, int b)
        {
#if TWO_CALLS_MODULUS
            return (a % b + b) % b;
#else
            int mod = a % b;
            return mod < 0 ? mod + b : mod;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Mod(this float a, float b)
        {
#if TWO_CALLS_MODULUS
            return (a % b + b) % b;
#else
            float mod = a % b;
            return mod < 0 ? mod + b : mod;
#endif
        }

        /// <summary>
        ///     Makes point from circle on sliced sphere
        /// </summary>
        /// <param name="sliceAngleRad">Angle offset [0, Pi]</param>
        /// <param name="angleRad">Angle in radians [0, Pi*2]</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPointOnSlicedSphereByAngles(float sliceAngleRad, float angleRad)
        {
            Assert.IsTrue(sliceAngleRad >= 0, $"{nameof(sliceAngleRad)}({sliceAngleRad}) >= 0");
            Assert.IsTrue(sliceAngleRad <= Pi, $"{nameof(sliceAngleRad)}({sliceAngleRad}) <= Pi");

            float z = Mathf.Cos(sliceAngleRad);
            return GetPointOnSlicedSphere(z, angleRad);
        }

        /// <summary>
        ///     Makes point from circle on sliced sphere
        /// </summary>
        /// <param name="sliceZ">Z offset [-1, 1]</param>
        /// <param name="radiansAngle">Angle in radians [0, Pi*2]</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPointOnSlicedSphere(float sliceZ, float radiansAngle)
        {
            Assert.IsTrue(sliceZ >= -1, $"{nameof(sliceZ)}({sliceZ}) >= -1");
            Assert.IsTrue(sliceZ <= 1, $"{nameof(sliceZ)}({sliceZ}) <= 1");
            Assert.IsTrue(radiansAngle >= 0, $"{nameof(radiansAngle)}({radiansAngle}) >= 0");
            Assert.IsTrue(radiansAngle <= TwoPi, $"{nameof(radiansAngle)}({radiansAngle}) <= 2*Pi");

            float sqrt = Mathf.Sqrt(1 - sliceZ * sliceZ);
            return new Vector3(sqrt * Mathf.Cos(radiansAngle), sqrt * Mathf.Sin(radiansAngle), sliceZ);
        }

        /// <summary>
        ///     Makes accuracy vector
        /// </summary>
        /// <param name="fwdDirection">Look forward direction</param>
        /// <param name="upDirection">Look up direction</param>
        /// <param name="offsetAngleRad">Concrete accuracy angle offset on sphere. Pass value in range [0, Pi]</param>
        /// <param name="dirAngleRad">Concrete accuracy angle direction on sphere's circle. Pass value in range [0, Pi*2)</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetAccuracyDirection(Vector3 fwdDirection, Vector3 upDirection, float offsetAngleRad, float dirAngleRad)
        {
            Vector3 dir = GetPointOnSlicedSphereByAngles(offsetAngleRad, dirAngleRad);
            return Quaternion.LookRotation(fwdDirection, upDirection) * dir;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothDamp(
            float current,
            float target,
            ref float currentVelocity,
            float smoothTime,
            float maxSpeed = float.PositiveInfinity)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothDamp(
            float current,
            float target,
            ref float currentVelocity,
            float smoothTime,
            float maxSpeed,
            float deltaTime)
        {
            smoothTime = Math.Max(0.0001f, smoothTime);
            float omega = 2.0f / smoothTime;
            float x = omega * deltaTime;
            float exp = 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);
            float deltaX = target - current;
            float maxDelta = maxSpeed * smoothTime;

            // ensure we do not exceed our max speed
            deltaX = Clamp(deltaX, -maxDelta, maxDelta);
            float temp = (currentVelocity + omega * deltaX) * deltaTime;
            float result = current - deltaX + (deltaX + temp) * exp;
            currentVelocity = (currentVelocity - omega * temp) * exp;

            // ensure that we do not overshoot our target
            if (target - current > 0.0f == result > target)
            {
                result = target;
                currentVelocity = 0.0f;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeAngleRadToRange(float value, float start, float end)
        {
            return NormalizeAngleToRangeInternal(value, start, end, TwoPi);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeAngleToRange(float value, float start, float end)
        {
            return NormalizeAngleToRangeInternal(value, start, end, 360f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NormalizeAngleToRangeInternal(float value, float start, float end, float div)
        {
            float d = (end - start) / div;
            float v = (value - start) / div;
            if (InRange(v, 0, d))
            {
                return value;
            }

            if (Math.Abs(d) > 1.0)
            {
                //the start->end range is larger than 360, so just land inside it
                v = (float)Math.Round(v, MidpointRounding.AwayFromZero);
                value -= v * div;
            }
            else
            {
                //the start->end range is smaller than 360
                //so lets land as close as we can to the bounds of the range on either end
                v = (float)Math.Truncate(v);
                value -= v * div;
                if (Math.Abs(value - end) > 180.0)
                {
                    value -= Math.Sign(value - end) * div;
                }
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ComputeMachineEpsilon()
        {
            float fourThirds = 4.0f / 3.0f;
            float third = fourThirds - 1.0f;
            float one = third + third + third;
            return Math.Abs(1.0f - one);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Bias(double x, double bias)
        {
            double k = Math.Pow(1 - bias, 3);

            return (x * k) / (x * k - x + 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Bias(float x, float bias)
        {
            float k = Mathf.Pow(1 - bias, 3);

            return (x * k) / (x * k - x + 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Inverse(float v, float fallback = float.MaxValue)
        {
            return Mathf.Approximately(v, 0) ? fallback : 1f / v;
        }
    }
}