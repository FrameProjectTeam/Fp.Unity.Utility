using System;
using System.IO;
using System.IO.Compression;

namespace Fp.Utility
{
    public static class ByteArrayUtils
    {
        private const int BufferSize = 64 * 1024; //64kB

        public static byte[] FromBitConverterString(this string data)
        {
            string[] arr = data.Split('-');
            var array = new byte[arr.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                array[i] = Convert.ToByte(arr[i], 16);
            }

            return array;
        }

        public static byte[] Compress(this byte[] inputData)
        {
            if (inputData == null)
            {
                throw new ArgumentNullException(nameof(inputData));
            }

            using var compressIntoMs = new MemoryStream();
            using (var gzs = new BufferedStream(new GZipStream(compressIntoMs, CompressionMode.Compress), BufferSize))
            {
                gzs.Write(inputData, 0, inputData.Length);
            }

            return compressIntoMs.ToArray();
        }

        public static byte[] Decompress(this byte[] inputData)
        {
            if (inputData == null)
            {
                throw new ArgumentNullException(nameof(inputData));
            }

            using var compressedMs = new MemoryStream(inputData);
            using var decompressedMs = new MemoryStream();

            using (var gzs = new BufferedStream(new GZipStream(compressedMs,
                                                               CompressionMode.Decompress), BufferSize))
            {
                gzs.CopyTo(decompressedMs);
            }

            return decompressedMs.ToArray();
        }
    }
}