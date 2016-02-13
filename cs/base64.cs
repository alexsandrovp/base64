using System;

class Base64
{
    private static byte[] encodelookup = {
            0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A,
            0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A,
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x2B, 0x2F };

    private static byte[] decodelookup = {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x3E, 0xFF, 0xFF, 0xFF, 0x3F,
            0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
            0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28,
            0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33
        };

    public static byte[] encode(byte[] plain)
    {
        //3 octets (bytes) match 4 sextets (bitcount -> 3*8 == 4*6)
        int bitcount = plain.Length * 4;
        int remainder = plain.Length % 3;
        byte[] encoded = new byte[(bitcount % 3) > 0 ? 1 + bitcount / 3 : bitcount / 3];

        int limit = plain.Length - remainder - 1;
        int i = 0, j = 0;
        for (; i < limit; i += 3, j += 4)
        {
            encoded[j] = encodelookup[(plain[i] & 0xFC) >> 2];
            encoded[j + 1] = encodelookup[((plain[i] & 0x03) << 4) | ((plain[i + 1] & 0xF0) >> 4)];
            encoded[j + 2] = encodelookup[((plain[i + 1] & 0x0F) << 2) | ((plain[i + 2] & 0xC0) >> 6)];
            encoded[j + 3] = encodelookup[plain[i + 2] & 0x3F];
        }

        if (remainder-- > 0)
        {
            encoded[j] = encodelookup[(plain[i] & 0xFC) >> 2];
            if (remainder > 0)
            {
                encoded[j + 1] = encodelookup[((plain[i] & 0x03) << 4) | ((plain[i + 1] & 0xF0) >> 4)];
                encoded[j + 2] = encodelookup[((plain[i + 1] & 0x0F) << 2)];
            }
            else
            {
                encoded[j + 1] = encodelookup[((plain[i] & 0x03) << 4)];
            }
        }

        return encoded;
    }

    public static byte[] decode(byte[] encoded)
    {
        int bitcount = encoded.Length * 3;
        byte[] plain = new byte[bitcount / 4];

        int remainder = plain.Length % 3;
        int limit = plain.Length - remainder - 1;

        int i = 0, j = 0;
        for (; i < limit; i += 3, j += 4)
        {
            if (decodelookup[encoded[j]] == 255) throw new Exception("encoded stream is corrupted at index " + j);
            if (decodelookup[encoded[j + 1]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 1));
            if (decodelookup[encoded[j + 2]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 2));
            if (decodelookup[encoded[j + 3]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 3));

            plain[i] = (byte)(((decodelookup[encoded[j]] & 0x3F) << 2) | ((decodelookup[encoded[j + 1]] & 0x30) >> 4));
            plain[i + 1] = (byte)(((decodelookup[encoded[j + 1]] & 0x0F) << 4) | ((decodelookup[encoded[j + 2]] & 0x3C) >> 2));
            plain[i + 2] = (byte)(((decodelookup[encoded[j + 2]] & 0x03) << 6) | (decodelookup[encoded[j + 3]] & 0x3F));
        }

        if (remainder-- > 0)
        {
            if (decodelookup[encoded[j]] == 255) throw new Exception("encoded stream is corrupted at index " + j);
            if (decodelookup[encoded[j + 1]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 1));
            plain[i] = (byte)(((decodelookup[encoded[j]] & 0x3F) << 2) | ((decodelookup[encoded[j + 1]] & 0x30) >> 4));

            if (remainder > 0)
            {
                if (decodelookup[encoded[j + 2]] == 255) throw new Exception("encoded stream is corrupted at index " + (j + 2));
                plain[i + 1] = (byte)(((decodelookup[encoded[j + 1]] & 0x0F) << 4) | ((decodelookup[encoded[j + 2]] & 0x3C) >> 2));
            }
        }

        return plain;
    }
}
