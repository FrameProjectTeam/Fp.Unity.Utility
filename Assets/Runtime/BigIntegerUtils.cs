using System;
using System.Collections.Generic;
using System.Numerics;

namespace Fp.Utility
{
    public static class BigIntegerUtils
    {
        public static BigInteger Sqrt(this ref BigInteger n)
        {
            if (n == 0)
            {
                return 0;
            }

            if (n <= 0)
            {
                throw new ArithmeticException("NaN");
            }

            var bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
            BigInteger root = BigInteger.One << (bitLength / 2);

            while (!IsSqrt(n, root))
            {
                root += n / root;
                root /= 2;
            }

            return root;
        }

        private static bool IsSqrt(this BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return n >= lowerBound && n < upperBound;
        }

        private static void CalculateDivisors(this BigInteger n, ICollection<BigInteger> result)
        {
            for (BigInteger i = BigInteger.One; i * i < n; i++)
            {
                if (n % i == BigInteger.Zero)
                {
                    result.Add(i);
                }
            }

            for (BigInteger i = n.Sqrt(); i >= 1; i--)
            {
                if (n % i == BigInteger.Zero)
                {
                    result.Add(n / i);
                }
            }
        }
    }
}