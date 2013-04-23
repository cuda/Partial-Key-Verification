/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;
using System.Diagnostics;
using System.Text;

namespace PartialKeyVerification
{
    /// <summary>
    /// Utility to convert a byte array to/from a Base 32 string.
    /// </summary>
    internal static class Base32
    {
        private const string Map = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        /// <summary>
        /// Converts a byte array to a base 32 string.
        /// </summary>
        /// <param name="data">The array to convert.</param>
        /// <returns>Base 32 encoded string of the byte array.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="data"/> is null.</exception>
        public static string ToBase32(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var ret = new StringBuilder();
            var len = data.Length - 1;
            for (int i = 0, offset = 0; i <= len; i++)
            {
                byte ip1 = 0;
                if (i != len)
                {
                    ip1 = data[i + 1];
                }
                switch (offset)
                {
                    case 0:
                        ret.Append(Map[data[i] >> 3]);
                        ret.Append(Map[((data[i] << 2) & 0x1F) | (ip1 >> 6)]);
                        offset = 2;
                        break;
                    case 1:
                        ret.Append(Map[(data[i] >> 2) & 0x1F]);
                        ret.Append(Map[((data[i] << 3) & 0x1F) | (ip1 >> 5)]);
                        offset = 3;
                        break;
                    case 2:
                        ret.Append(Map[(data[i] >> 1) & 0x1F]);
                        ret.Append(Map[((data[i] << 4) & 0x1F) | (ip1 >> 4)]);
                        offset = 4;
                        break;
                    case 3:
                        ret.Append(Map[data[i] & 0x1F]);
                        offset = 0;
                        break;
                    case 4:
                        ret.Append(Map[((data[i] << 1) & 0x1F) | (ip1 >> 7)]);
                        offset = 1;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return ret.ToString();
        }

        /// <summary>
        /// Converts a base 32 encoded string into a byte array.
        /// </summary>
        /// <param name="data">The string to convert.</param>
        /// <returns>A byte array of the converted string.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="data"/> is null.</exception>
        public static byte[] FromBase32(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var ret = new byte[data.Length*5/8];
            byte b = 0;
            var offset = 0;
            for (int i = 0, j = 0; i < ret.Length; i++)
            {
                switch (offset)
                {
                    case 0:
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] = (byte) (b << 3);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= (byte) (b >> 2);
                        offset = 3;
                        break;
                    case 3:
                        ret[i] = (byte) (b << 6);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= (byte) (b << 1);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= (byte) (b >> 4);
                        ret[i] |= (byte) (b >> 4);
                        offset = 1;
                        break;
                    case 1:
                        ret[i] = (byte) (b << 4);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= (byte) (b >> 1);
                        offset = 4;
                        break;
                    case 4:
                        ret[i] = (byte) (b << 7);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= (byte) (b << 2);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= (byte) (b >> 3);
                        offset = 2;
                        break;
                    case 2:
                        ret[i] = (byte) (b << 5);
                        b = (byte) Map.IndexOf(data[j++]);
                        ret[i] |= b;
                        offset = 0;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return ret;
        }
    }
}