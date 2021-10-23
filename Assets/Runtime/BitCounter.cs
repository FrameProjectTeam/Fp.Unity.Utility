namespace Fp.Utility
{
    public static class BitCounter
    {
        private static byte[] _bitCountsLookupTable;

        static BitCounter()
        {
            InitializeBitCounts();
        }

        public static int PrecomputedBitCount(int value)
        {
            return
                _bitCountsLookupTable[value & 255] + _bitCountsLookupTable[(value >> 8) & 255] +
                _bitCountsLookupTable[(value >> 16) & 255] + _bitCountsLookupTable[(value >> 24) & 255];
        }

        public static int PrecomputedBitCount(short value)
        {
            return _bitCountsLookupTable[value & 255] + _bitCountsLookupTable[(value >> 8) & 255];
        }

        public static int IteratedBitCount(int n)
        {
            int test = n;
            var count = 0;

            while (test != 0)
            {
                if ((test & 1) == 1)
                {
                    count++;
                }

                test >>= 1;
            }

            return count;
        }

        public static int SparseBitCount(int n)
        {
            var count = 0;
            while (n != 0)
            {
                count++;
                n &= n - 1;
            }

            return count;
        }

        public static int BitCount(int i)
        {
            i -= (i >> 1) & 0x55555555;
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        private static void InitializeBitCounts()
        {
            _bitCountsLookupTable = new byte[256];
            int position1 = -1;
            int position2 = -1;

            for (var i = 1; i < 256; i++, position1++)
            {
                if (position1 == position2)
                {
                    position1 = 0;
                    position2 = i;
                }

                _bitCountsLookupTable[i] = (byte)(_bitCountsLookupTable[position1] + 1);
            }
        }
    }
}