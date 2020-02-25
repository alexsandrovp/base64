/*
Copyright (c) 2016 Alex Vargas

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.
2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.
3. This notice may not be removed or altered from any source distribution.
*/

using System;

class Base32
{
    private static byte[] encodelookup = { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118 };
    private static byte[] decodelookup = { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

    public static byte[] encode(byte[] plain)
    {
        //5 octets (bytes) match 8 quintets (bitcount -> 5*8 == 8*5)
        int bitcount = plain.Length * 8;
        int remainder = plain.Length % 5;
        byte[] encoded = new byte[(bitcount % 5) > 0 ? 1 + bitcount / 5 : bitcount / 5];

        int limit = plain.Length - remainder - 1;
        int i = 0, j = 0;
        for (; i < limit; i += 5, j += 8)
        {
            encoded[j] = encodelookup[(plain[i] & 0xF8) >> 3];
            encoded[j + 1] = encodelookup[((plain[i] & 0x07) << 2) | ((plain[i + 1] & 0xC0) >> 6)];
            encoded[j + 2] = encodelookup[(plain[i + 1] & 0x3E) >> 1];
            encoded[j + 3] = encodelookup[((plain[i + 1] & 0x01) << 4) | ((plain[i + 2] & 0xF0) >> 4)];
            encoded[j + 4] = encodelookup[((plain[i + 2] & 0x0F) << 1) | ((plain[i + 3] & 0x80) >> 7)];
            encoded[j + 5] = encodelookup[(plain[i + 3] & 0x7C) >> 2];
            encoded[j + 6] = encodelookup[((plain[i + 3] & 0x03) << 3) | ((plain[i + 4] & 0xE0) >> 5)];
            encoded[j + 7] = encodelookup[plain[i + 4] & 0x1F];
        }

        if (remainder-- > 0)
        {
            encoded[j] = encodelookup[(plain[i] & 0xF8) >> 3];
            if (remainder-- > 0)
            {
                encoded[j + 1] = encodelookup[((plain[i] & 0x07) << 2) | ((plain[i + 1] & 0xC0) >> 6)];
                encoded[j + 2] = encodelookup[(plain[i + 1] & 0x3E) >> 1];
                if (remainder-- > 0)
                {
                    encoded[j + 3] = encodelookup[((plain[i + 1] & 0x01) << 4) | ((plain[i + 2] & 0xF0) >> 4)];
                    if (remainder > 0)
                    {
                        encoded[j + 4] = encodelookup[((plain[i + 2] & 0x0F) << 1) | ((plain[i + 3] & 0x80) >> 7)];
                        encoded[j + 5] = encodelookup[(plain[i + 3] & 0x7C) >> 2];
                        encoded[j + 6] = encodelookup[(plain[i + 3] & 0x03) << 3];
                    }
                    else
                    {
                        encoded[j + 4] = encodelookup[(plain[i + 2] & 0x0F) << 1];
                    }
                }
                else
                {
                    encoded[j + 3] = encodelookup[(plain[i + 1] & 0x01) << 4];
                }
            }
            else
            {
                encoded[j + 1] = encodelookup[(plain[i] & 0x07) << 2];
            }
        }

        return encoded;
    }

    public static byte[] decode(byte[] encoded)
    {
        int bitcount = encoded.Length * 5;
        byte[] plain = new byte[bitcount / 8];

        int remainder = plain.Length % 5;
        int limit = plain.Length - remainder - 1;

        int i = 0, j = 0;
        for (; i < limit; i += 5, j += 8)
        {
            if (decodelookup[encoded[j]] == 255) throw new Exception("encoded stream is corrupted at index " + j);
            if (decodelookup[encoded[j + 1]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 1));
            if (decodelookup[encoded[j + 2]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 2));
            if (decodelookup[encoded[j + 3]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 3));
            if (decodelookup[encoded[j + 4]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 4));
            if (decodelookup[encoded[j + 5]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 5));
            if (decodelookup[encoded[j + 6]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 6));
            if (decodelookup[encoded[j + 7]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 7));

            plain[i] = (byte)(((decodelookup[encoded[j]] & 0x1F) << 3) | ((decodelookup[encoded[j + 1]] & 0x1C) >> 2));
            plain[i + 1] = (byte)(((decodelookup[encoded[j + 1]] & 0x03) << 6) | ((decodelookup[encoded[j + 2]] & 0x1F) << 1) | ((decodelookup[encoded[j + 3]] & 0x10) >> 4));
            plain[i + 2] = (byte)(((decodelookup[encoded[j + 3]] & 0x0F) << 4) | ((decodelookup[encoded[j + 4]] & 0x1E) >> 1));
            plain[i + 3] = (byte)(((decodelookup[encoded[j + 4]] & 0x01) << 7) | ((decodelookup[encoded[j + 5]] & 0x1F) << 2) | ((decodelookup[encoded[j + 6]] & 0x18) >> 3));
            plain[i + 4] = (byte)(((decodelookup[encoded[j + 6]] & 0x07) << 5) | ((decodelookup[encoded[j + 7]] & 0x1F)));
        }

        if (remainder-- > 0)
        {
            if (decodelookup[encoded[j]] == 255) throw new Exception("encoded stream is corrupted at index " + j);
            if (decodelookup[encoded[j + 1]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 1));

            plain[i] = (byte)(((decodelookup[encoded[j]] & 0x1F) << 3) | ((decodelookup[encoded[j + 1]] & 0x1C) >> 2));
            if (remainder-- > 0)
            {
                if (decodelookup[encoded[j + 2]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 2));
                if (decodelookup[encoded[j + 3]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 3));

                plain[i + 1] = (byte)(((decodelookup[encoded[j + 1]] & 0x03) << 6) | ((decodelookup[encoded[j + 2]] & 0x1F) << 1) | ((decodelookup[encoded[j + 3]] & 0x10) >> 4));
                if (remainder-- > 0)
                {
                    if (decodelookup[encoded[j + 4]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 4));

                    plain[i + 2] = (byte)(((decodelookup[encoded[j + 3]] & 0x0F) << 4) | ((decodelookup[encoded[j + 4]] & 0x1E) >> 1));
                    if (remainder > 0)
                    {

                        if (decodelookup[encoded[j + 5]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 5));
                        if (decodelookup[encoded[j + 6]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 6));

                        plain[i + 3] = (byte)(((decodelookup[encoded[j + 4]] & 0x01) << 7) | ((decodelookup[encoded[j + 5]] & 0x1F) << 2) | ((decodelookup[encoded[j + 6]] & 0x18) >> 3));
                    }
                }
            }
        }

        return plain;
    }
}
