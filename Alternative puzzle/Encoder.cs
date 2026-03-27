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

        var buffer = data
            .Select((b, i) => (byte)(b ^ key[i % key.Length]))
            .ToArray();

        Array.Reverse(buffer);

        var result = buffer
            .Select((b, i) => (byte)((b + (i % 5)) % 256))
            .ToArray();

        string encoded = Convert.ToBase64String(result);

        string checksum;
        using (var sha = SHA256.Create())
        {
            var hash = sha.ComputeHash(result);
            checksum = BitConverter.ToString(hash).Replace("-", "").Substring(0, 8);
        }

        Console.WriteLine("\nEncoded output:");
        Console.WriteLine($"{encoded}#{checksum}");
    }
}