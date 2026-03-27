using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

class Decoder
{
    static void Main()
    {
        Console.WriteLine("Paste encoded string (before #):");
        string encoded = Console.ReadLine();

        Console.WriteLine("Checksum (after #):");
        string checksum = Console.ReadLine();

        byte[] step2 = Convert.FromBase64String(encoded);

        // Verify checksum
        string calc;
        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(step2);
            calc = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
        }

        if (calc != checksum)
        {
            Console.WriteLine($"Checksum mismatch! Expected {checksum}, got {calc}");
            return;
        }

        // Step 1: Undo rotation
        var step1 = step2
            .Select((b, i) => (byte)((b - (i % 5) + 256) % 256))
            .ToArray();

        // Step 2: Undo reverse
        Array.Reverse(step1);

        // Step 3: Undo XOR
        byte[] key = { 7, 19, 3, 11 };
        var original = step1
            .Select((b, i) => (byte)(b ^ key[i % key.Length]))
            .ToArray();

        string result = Encoding.UTF8.GetString(original);

        Console.WriteLine("\nDecoded message:");
        Console.WriteLine(result);
    }
}