#ifndef BASE64_H
#define BASE64_H


#include <stdlib.h>

/*
encodes 'plain' bytes as base64 into 'encoded', appending a null character at the end
'encoded' must be an unallocated pointer to char, this function will allocate the necessary memory.
it is the caller's responsibility to free the memory allocated in 'encoded' using 'delete[]'

'plain' is treated as an array of bytes of 'plain_length' length,
thus pay attention if your string has a NULL end, it will might be encoded as well

the function returns the length of the array (excluding the null character)
*/
size_t base64_encode(const char* plain, size_t plain_length, char** encoded);

/*
decodes 'encoded' bytes as base64 into 'plain', appending a null character at the end
'plain' must be an unallocated pointer to char, this function will allocate the necessary memory.
it is the caller's responsibility to free the memory allocated in 'plain' using 'delete[]'

'encoded_length' must be the length of bytes in the 'encoded' array that must be encoded.
careful not to pass the array size, as it may contain invalid characters that cannot be decoded (NULL terminator, for instance)

the function returns the length of the array (excluding the null character)

if the function fails, the memory allocated to 'plain' is freed and the pointer is set to null.
the function then returns an index in 'encoded' that triggered the error.
*/
size_t base64_decode(const char* encoded, size_t encoded_length, char** plain);

#endif