using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Fp.Utility
{
    public static class TextUtility
    {
        private const int RomanDigitsValuesLastIdx = 12; // RomanDigitsValues.Length - 1;
        
        private static readonly NumberFormatInfo DecimalSeparatorFormatInfo = new NumberFormatInfo { NumberGroupSeparator = "." };

        private static readonly int[] RomanDigitsValues = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
        private static readonly string[] RomanDigits = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
        
        private static readonly string[] OrdinalSuffixes = { "th", "st", "nd", "rd" };
        
        private static readonly StringBuilder s_sb = new StringBuilder();

        public static ulong CalculateHash(this string text)
        {
            var hashedValue = 3074457345618258791ul;
            foreach (char t in text)
            {
                hashedValue += t;
                hashedValue *= 3074457345618258799ul;
            }

            return hashedValue;
        }

        public static ulong CalculateHash(this IEnumerator<string> texts)
        {
            ulong hash = 0;

            do
            {
                if (string.IsNullOrWhiteSpace(texts.Current))
                {
                    continue;
                }

                hash ^= texts.Current.CalculateHash();
            }
            while (texts.MoveNext());

            return hash;
        }

        public static string GetMd5Hash(this string input)
        {
            using var md5Hash = MD5.Create();

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            try
            {
                foreach (byte t in data)
                {
                    s_sb.Append(t.ToString("x2"));
                }

                return s_sb.ToString();
            }
            finally
            {
                s_sb.Clear();
            }
        }

        public static bool VerifyMd5Hash(this string input, string hash)
        {
            string hashOfInput = GetMd5Hash(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }

            return false;
        }

        public static string OrdinalSuffix(int number)
        {
            int ord = number % 100;
            if (ord / 10 == 1)
            {
                ord = 0;
            }

            ord %= 10;
            if (ord > 3)
            {
                ord = 0;
            }

            return OrdinalSuffixes[ord];
        }

        public static string SeparateNumber(int value)
        {
            return value.ToString("#,##0", DecimalSeparatorFormatInfo);
        }

        public static string SeparateNumber(float value)
        {
            return value.ToString("#,##0,00", DecimalSeparatorFormatInfo);
        }

        private static string ToRomanNumber(int number)
        {
            try
            {
                s_sb.AppendRomanNumber(number);
                return s_sb.ToString();
            }
            finally
            {
                s_sb.Clear();
            }
        }

        private static void AppendRomanNumber(this StringBuilder sb, int number)
        {
            while (number > 0)
            {
                for (int i = RomanDigitsValuesLastIdx; i >= 0; i--)
                {
                    if (number / RomanDigitsValues[i] < 1)
                    {
                        continue;
                    }

                    number -= RomanDigitsValues[i];
                    sb.Append(RomanDigits[i]);
                    break;
                }
            }
        }
        
        private static string DictionaryToString(IReadOnlyDictionary<string, string> dictionary)
        {
            try
            {
                dictionary.DictionaryToString(s_sb);
                return s_sb.ToString();
            }
            finally
            {
                s_sb.Clear();
            }
        }

        private static void DictionaryToString(this IReadOnlyDictionary<string, string> dictionary, StringBuilder sb)
        {
            sb.Append("{");
            
            var first = true;
            foreach((string key, string value) in dictionary)
            {
                if(!first)
                {
                    sb.Append(", ");
                }

                sb.Append("{");
                sb.Append(key);
                sb.Append(": ");
                sb.Append(value);
                sb.Append("}");

                first = false;
            }

            sb.Append("}");
        }
    }
}