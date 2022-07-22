using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Tools
{
    public static class Pretty64Helper
    {
        public static string Encode(byte[] inArray, char[] Atlas = null)
        {
            if (Atlas == null) Atlas = ReadableAtlas;

            byte[] raw64 = toRaw64(inArray);

            string final = "";

            foreach(byte b in raw64)
            {
                string yourByteString = Convert.ToString(b, 2).PadLeft(8, '0');
                Debug.Log($"{b} - {yourByteString}");

                try
                {
                    final += Atlas[b];
                }
                catch(IndexOutOfRangeException e)
                {
                    Debug.LogError($"Failed to index byte {b}\n" + e.ToString());
                    final += '?';
                }
            }

            return final;
        }

        static byte[] toRaw64(byte[] inArray)
        {
            // size is 4/3 the input size, but rounded up
            // stackOverflow question 17944 says this rounds up
            int size = (inArray.Length * 4 + 2) / 3;

            byte[] result = new byte[size];

            // go through our input array in groups of 3 bytes
            for (int inIndex = 0, outIndex = 0; inIndex < inArray.Length; inIndex += 3, outIndex += 4)
            {
                // name these for our convenience
                byte left_  = inArray[inIndex];
                // if inArray isn't an even multiple of 3, we will pad with zeroes
                byte mid__   = (inIndex + 1 > inArray.Length - 1) ? (byte)0 : inArray[inIndex + 1];
                byte right = (inIndex + 2 > inArray.Length - 1) ? (byte)0 : inArray[inIndex + 2];

                result[outIndex    ] = (byte)(left_ >> 2 & 0b_0011_1111);
                if (outIndex + 1 >= size) break;
                result[outIndex + 1] = (byte)(((left_ << 4) & 0b_0011_0000) | ((mid__ >> 4) & 0b_0000_1111));
                if (outIndex + 2 >= size) break;
                result[outIndex + 2] = (byte)(((mid__ << 2) & 0b_0011_1100) | ((right >> 6) & 0b_0000_0011));
                if (outIndex + 3 >= size) break;
                result[outIndex + 3] = (byte)(((right     ) & 0b_0011_1111));
            }

            return result;
        }


        /// <summary>
        /// RFC 4648. maybe compatible out-of-the-box with other stuff
        /// </summary>
        public static char[] CompatibleAtlas => new char[64]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', // 0-7
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', // 8-15
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', // 16-23
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', // 24-31
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', // 32-39
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', // 40-47
            'w', 'x', 'y', 'z', '0', '1', '2', '3', // 48-55
            '4', '5', '6', '7', '8', '9', '+', '/', // 56-63
        };

        /// <summary>
        /// RFC 4648, but with only URL-compatible characters
        /// </summary>
        public static char[] URLCompatibleAtlas => new char[64]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', // 0-7
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', // 8-15
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', // 16-23
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', // 24-31
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', // 32-39
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', // 40-47
            'w', 'x', 'y', 'z', '0', '1', '2', '3', // 48-55
            '4', '5', '6', '7', '8', '9', '-', '_', // 56-63
        };

        /// <summary>
        /// RFC 4648, but with similar characters removed, and special characters chosen that aren't often used in markup languages like markdown
        /// </summary>
        public static char[] ReadableAtlas => new char[64]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', // 0-7
            '!', 'J', 'K', '$', 'M', 'N', '%', 'P', // 8-15
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', // 16-23
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', // 24-31
            'g', 'h', '&', 'j', 'k', '<', 'm', 'n', // 32-39
            '>', 'p', 'q', 'r', 's', 't', 'u', 'v', // 40-47
            'w', 'x', 'y', 'z', '=', '?', '2', '3', // 48-55
            '4', '5', '6', '7', '8', '9', '+', '/', // 56-63
        };
    }
}
