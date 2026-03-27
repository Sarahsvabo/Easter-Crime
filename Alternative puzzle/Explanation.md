## High-Level Overview

The encoder is basically doing this pipeline:
```text
Text → Bytes → Scramble → Base64 → Add checksum
```

### Step 1: XOR with key

*“Mix the data using a repeating secret key”*
Each byte is combined with a key byte using XOR
This is reversible (XOR again = original)

### Step 2: Reverse

*“Flip the order”*
Just reverses the array
Adds confusion, easy to undo
### Step 3: Rotate by index

*“Shift each byte slightly depending on position”*
```text
Byte 0 → +0
Byte 1 → +1
Byte 2 → +2
Byte 3 → +3
Byte 4 → +4
Byte 5 → +0 (wraps around)
```

### Step 4: Base64 encode

*“Make it safe to display”*
Converts binary data into readable text

### Step 5: Checksum

*“Verify correctness”*
If someone decodes incorrectly → checksum won’t match

## Line-by-Line Code Explanation
```C#
//Input → Bytes
byte[] data = Encoding.UTF8.GetBytes(input);
```
This converts the string into raw bytes

Example:
```text
"Hi" → [72, 105]
```

### Step 1: XOR with repeating key
```C#
var step1 = data
    .Select((b, i) => (byte)(b ^ key[i % key.Length]))
    .ToArray();
```

This is LINQ magic:
```text
b = current byte
i = index (position in array)
```
So if data = [72, 105, 33], you get:
```text
i	b
0	72
1	105
2	33
```

```C#
key[i % key.Length]
```
This repeats the key:
```C#
key = [7, 19, 3, 11]
```
```text
i	i % 4	key value
0	0	7
1	1	19
2	2	3
3	3	11
4	0	7
...	...	...
```

```C#
(byte)(b ^ key[...])
```
XOR combines the byte with the key

Example:
72 ^ 7 = 79
#### Summary of Step 1
Each byte is scrambled with a repeating key based on its position.

### Step 2: Reverse
```C#
Array.Reverse(step1);
```
In-place reversal
```text
[1, 2, 3] → [3, 2, 1]
```

### Step 3: Rotate by index
```C#
var step2 = step1
    .Select((b, i) => (byte)((b + (i % 5)) % 256))
    .ToArray();
```

Again, .Select((b, i)) gives:
```text
b = byte
i = index
```

```C#
(i % 5)
```
Creates repeating pattern:
```text
0, 1, 2, 3, 4, 0, 1, ...
```

```C#
(b + (i % 5)) % 256
```
Adds small offset to each byte
% 256 keeps it within byte range (0–255)

Example:
```text
i	b	i % 5	result
0	100	0	100
1	100	1	101
2	100	2	102
```

#### Summary of Step 3
Each byte is slightly shifted depending on its position.

### Step 4: Base64
```C#
string encoded = Convert.ToBase64String(step2);
```
Turns bytes into readable ASCII

Example:
```text
[72, 105] → "SGk="
```

### Step 5: Checksum
```C#
using (var sha = SHA256.Create())
{
    var hash = sha.ComputeHash(step2);
    checksum = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
}
```
What happens:
- Compute SHA-256 hash (32 bytes)
- Convert to hex string
- Take first 8 characters

Example:
```text
Full hash: A1B2C3D4E5F6...
Checksum:  A1B2C3D4
```

### Why This Is Reversible

Each step has an inverse:
```text
Step	Operation	Reverse
1	XOR	XOR again
2	Reverse	Reverse
3	+ (i % 5)	- (i % 5)
4	Base64	Base64 decode
```

One subtle LINQ insight

This pattern:

.Select((value, index) => ...)

is incredibly powerful because it lets you:

Use position-dependent transformations
Without writing a manual loop

Equivalent to:

var result = new List<byte>();
for (int i = 0; i < data.Length; i++)
{
    var b = data[i];
    result.Add((byte)(b ^ key[i % key.Length]));
}
🧾 TL;DR
LINQ .Select((value, index)) = map with position awareness
% is used to create repeating patterns
XOR = reversible scrambling
Reverse = simple obfuscation
Base64 = display-safe encoding
SHA256 = correctness check