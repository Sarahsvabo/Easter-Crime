using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

class Encoder
{
    static void Main()
    {
        Console.WriteLine("Enter the secret phrase:");
        var input = Console.ReadLine();

        byte[] data = Encoding.UTF8.GetBytes(input);
        byte[] key = { 7, 19, 3, 11 };

        // Step 1: XOR with repeating key
        var step1 = data
            .Select((b, i) => (byte)(b ^ key[i % key.Length]))
            .ToArray();

        // Step 2: Reverse
        Array.Reverse(step1);

        // Step 3: Rotate by index (mod 256 ensures safe wrap)
        var step2 = step1
            .Select((b, i) => (byte)((b + (i % 5)) % 256))
            .ToArray();

        // Step 4: Encode to Base64 (printable)
        string encoded = Convert.ToBase64String(step2);

        // Step 5: Strong checksum (first 8 hex chars of SHA256)
        string checksum;
        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(step2);
            checksum = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
        }

        Console.WriteLine("\nEncoded output:");
        Console.WriteLine($"{encoded}#{checksum}");
    }
}