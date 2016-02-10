#include "base32.h"

const unsigned char encodelookup[] = {
//	'0'	  '1'	'2'	  '3'	'4'	  '5'	'6'	  '7'	'8'	  '9'
	0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
//	48	  49	50	  51	52	  53	54	  55	56	  57
//	00	  01	02	  03	04	  05	06	  07	08	  09

//	'A'	  'B'	'C'	  'D'	'E'	  'F'	'G'	  'H'	'I'	  'J'	'K'	  'L'	'M'	  'N'	'O'	  'P'	'Q'	  'R'	'S'	  'T'	'U'	  'V'
	0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56
//	65	  66	67	  68	69	  70	71	  72	73	  74	75	  76	77	  78	79	  80	81	  82	83	  84	85	  86
//	10	  11	12	  13	14	  15	16	  17	18	  19	20	  21	22	  23	24	  25	26	  27	28	  29	30	  31
//	0A	  0B	0C	  0D	0E	  0F	10	  11	12	  13	14	  15	16	  17	18	  19	1A	  1B	1C	  1D	1E	  1F
};

//use full matrix, don't truncate at pos 86 (safe decode) / 0xFF means unsupported character in encoded string
const unsigned char decodelookup[] = {
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//0-15
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//16-31
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//32-47
	0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//48-63
	0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, //64-79
	0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//80-95
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//96-111
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//112-127
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//128-143
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, //144-159
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//160-175
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//176-191
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//192-207
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//208-223
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,	//224-239
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF	//240-255
};

size_t base32_encode(const char* plain, size_t plain_length, char** encoded)
{
	//5 octets (bytes) match 8 quintets (bitcount -> 5*8 == 8*5)
	size_t bitcount = plain_length * 8;
	size_t remainder = plain_length % 5;
	size_t encoded_length = (bitcount % 5) > 0 ? 1 + bitcount / 5 : bitcount / 5;
	(*encoded) = new char[encoded_length];

	size_t limit = plain_length - remainder - 1;
	size_t i = 0, j = 0;
	for (; i < limit; i += 5, j += 8)
	{
		(*encoded)[j] = encodelookup[(plain[i] & 0xF8) >> 3];
		(*encoded)[j + 1] = encodelookup[((plain[i] & 0x07) << 2) | ((plain[i + 1] & 0xC0) >> 6)];
		(*encoded)[j + 2] = encodelookup[(plain[i + 1] & 0x3E) >> 1];
		(*encoded)[j + 3] = encodelookup[((plain[i + 1] & 0x01) << 4) | ((plain[i + 2] & 0xF0) >> 4)];
		(*encoded)[j + 4] = encodelookup[((plain[i + 2] & 0x0F) << 1) | ((plain[i + 3] & 0x80) >> 7)];
		(*encoded)[j + 5] = encodelookup[(plain[i + 3] & 0x7C) >> 2];
		(*encoded)[j + 6] = encodelookup[((plain[i + 3] & 0x03) << 3) | ((plain[i + 4] & 0xE0) >> 5)];
		(*encoded)[j + 7] = encodelookup[plain[i + 4] & 0x1F];
	}

	if (remainder-- > 0)
	{
		(*encoded)[j] = encodelookup[(plain[i] & 0xF8) >> 3];
		if (remainder-- > 0)
		{
			(*encoded)[j + 1] = encodelookup[((plain[i] & 0x07) << 2) | ((plain[i + 1] & 0xC0) >> 6)];
			(*encoded)[j + 2] = encodelookup[(plain[i + 1] & 0x3E) >> 1];
			if (remainder-- > 0)
			{
				(*encoded)[j + 3] = encodelookup[((plain[i + 1] & 0x01) << 4) | ((plain[i + 2] & 0xF0) >> 4)];
				if (remainder > 0)
				{
					(*encoded)[j + 4] = encodelookup[((plain[i + 2] & 0x0F) << 1) | ((plain[i + 3] & 0x80) >> 7)];
					(*encoded)[j + 5] = encodelookup[(plain[i + 3] & 0x7C) >> 2];
					(*encoded)[j + 6] = encodelookup[(plain[i + 3] & 0x03) << 3];
				}
				else
				{
					(*encoded)[j + 4] = encodelookup[(plain[i + 2] & 0x0F) << 1];
				}
			}
			else
			{
				(*encoded)[j + 3] = encodelookup[(plain[i + 1] & 0x01) << 4];
			}
		}
		else
		{
			(*encoded)[j + 1] = encodelookup[(plain[i] & 0x07) << 2];
		}
	}

	return encoded_length;
}

size_t base32_decode(const char* encoded, size_t encoded_length, char** plain)
{
	size_t error_index = -1;
	size_t bitcount = encoded_length * 5;
	size_t plain_length = bitcount / 8;
	(*plain) = new char[plain_length];
	
	size_t remainder = plain_length % 5;
	size_t limit = plain_length - remainder - 1;

	size_t i = 0, j = 0;
	for (; i < limit; i += 5, j += 8)
	{
		if (decodelookup[encoded[j]] == 0xFF) { error_index = j; goto error; }
		if (decodelookup[encoded[j + 1]] == 0xFF) { error_index = j + 1; goto error; }
		if (decodelookup[encoded[j + 2]] == 0xFF) { error_index = j + 2; goto error; }
		if (decodelookup[encoded[j + 3]] == 0xFF) { error_index = j + 3; goto error; }
		if (decodelookup[encoded[j + 4]] == 0xFF) { error_index = j + 4; goto error; }
		if (decodelookup[encoded[j + 5]] == 0xFF) { error_index = j + 5; goto error; }
		if (decodelookup[encoded[j + 6]] == 0xFF) { error_index = j + 6; goto error; }
		if (decodelookup[encoded[j + 7]] == 0xFF) { error_index = j + 7; goto error; }

		(*plain)[i] = (char)(((decodelookup[encoded[j]] & 0x1F) << 3) | ((decodelookup[encoded[j + 1]] & 0x1C) >> 2));
		(*plain)[i + 1] = (char)(((decodelookup[encoded[j + 1]] & 0x03) << 6) | ((decodelookup[encoded[j + 2]] & 0x1F) << 1) | ((decodelookup[encoded[j + 3]] & 0x10) >> 4));
		(*plain)[i + 2] = (char)(((decodelookup[encoded[j + 3]] & 0x0F) << 4) | ((decodelookup[encoded[j + 4]] & 0x1E) >> 1));
		(*plain)[i + 3] = (char)(((decodelookup[encoded[j + 4]] & 0x01) << 7) | ((decodelookup[encoded[j + 5]] & 0x1F) << 2) | ((decodelookup[encoded[j + 6]] & 0x18) >> 3));
		(*plain)[i + 4] = (char)(((decodelookup[encoded[j + 6]] & 0x07) << 5) | ((decodelookup[encoded[j + 7]] & 0x1F)));
	}

	if (remainder-- > 0)
	{
		if (decodelookup[encoded[j]] == 0xFF) { error_index = j; goto error; }
		if (decodelookup[encoded[j + 1]] == 0xFF) { error_index = j + 1; goto error; }

		(*plain)[i] = (char)(((decodelookup[encoded[j]] & 0x1F) << 3) | ((decodelookup[encoded[j + 1]] & 0x1C) >> 2));
		if (remainder-- > 0)
		{
			if (decodelookup[encoded[j + 2]] == 0xFF) { error_index = j + 2; goto error; }
			if (decodelookup[encoded[j + 3]] == 0xFF) { error_index = j + 3; goto error; }

			(*plain)[i + 1] = (char)(((decodelookup[encoded[j + 1]] & 0x03) << 6) | ((decodelookup[encoded[j + 2]] & 0x1F) << 1) | ((decodelookup[encoded[j + 3]] & 0x10) >> 4));
			if (remainder-- > 0)
			{
				if (decodelookup[encoded[j + 4]] == 0xFF) { error_index = j + 4; goto error; }

				(*plain)[i + 2] = (char)(((decodelookup[encoded[j + 3]] & 0x0F) << 4) | ((decodelookup[encoded[j + 4]] & 0x1E) >> 1));
				if (remainder > 0)
				{

					if (decodelookup[encoded[j + 5]] == 0xFF) { error_index = j + 5; goto error; }
					if (decodelookup[encoded[j + 6]] == 0xFF) { error_index = j + 6; goto error; }

					(*plain)[i + 3] = (char)(((decodelookup[encoded[j + 4]] & 0x01) << 7) | ((decodelookup[encoded[j + 5]] & 0x1F) << 2) | ((decodelookup[encoded[j + 6]] & 0x18) >> 3));
				}
			}
		}
	}

	return plain_length;

error:
	delete[](*plain);
	*plain = 0;
	return error_index;
}
