# base64
A pretty fast base64 encoder/decoder for C

## Encoding

```C
size_t base64_encode(const char* plain, size_t plain_length, char** encoded);
```

Encodes __plain__ bytes as base64 into __\*encoded__, appending a __NULL__ character at the end.

The output buffer __\*encoded__ must be an unallocated pointer to char. This function will allocate the necessary memory.

It is the caller's responsibility to free the memory allocated for __\*encoded__ by calling __free()__

The buffer __plain__ is treated as an array of bytes of length __plain_length__,
thus pay attention if your string has a __NULL__ terminator, it might be encoded as well if __plain_length__ is not set correctly.

The function returns the length of the encoded array (excluding the __NULL__ character).

Example:
```C
#include "base64.h"

...

size_t plainTextLength = 11;
char* plainText = "hello world";

size_t encodedLength;
char* encodedText = NULL;

encodedLength = base64_encode(plainText, plainTextLength, &encodedText);

...

free(encodedText);
```

## Decoding

```C
size_t base64_decode(const char* encoded, size_t encoded_length, char** plain);
```

Decodes __encoded__ base64 bytes into __\*plain__, appending a __NULL__ character at the end.

The buffer __\*plain__ must be an unallocated pointer to char. This function will allocate the necessary memory.

It is the caller's responsibility to free the memory allocated for __\*plain__ using __free()__.

The value of __encoded_length__ must be the length of bytes in the array __encoded__ that must be decoded.
Careful not to pass the array size, as it may contain invalid characters that cannot be decoded (__NULL__ terminator, for instance).

The function returns the length of the array __\*plain__ (excluding the __NULL__ character).

If the function fails, the memory allocated to __\*plain__ is freed and the pointer is set to __NULL__.
You recognize a decode failure when the buffer pointed by __\*plain__ is __NULL__.
In that case, the function returns the index of the character in __encoded__ that triggered the error.

Example:
```C
#include "base64.h"

...

size_t plainTextLength;
char* plainText = NULL;

size_t encodedLength = 16;
char* encodedText = "aGVsbG8gd29ybGQ=";

plainTextLength = base64_decode(encodedText, encodedLength, &plainText);

if (plainText == NULL)
{
  printf("invalid character at index %d: %c", plainTextLength, encodedText[plainTextLength]);
}
else
{
  ...
  free(plainText);
}
```
