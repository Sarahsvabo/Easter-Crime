using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

class Easter2026tool 
{
    static void Main(string[] args)
    {
        var flags = ParseArgs(args);

        if (flags.ContainsKey("--help") || args.Length == 0)
        {
            ShowHelp();
            return;
        }

        if (flags.ContainsKey("--encode"))
        {
            string text = flags.GetValueOrDefault("--text", "");
            
            Encode(text);
            return;
        }

        if (flags.ContainsKey("--decode"))
        {
            string cipher = flags.GetValueOrDefault("--cipher", "");
            string checksum = flags.GetValueOrDefault("--checksum", "");
            Console.WriteLine($"Decoding with cipher='{cipher}', checksum={checksum}");
            Decode(cipher, checksum);
            return;
        }
    }

    static void Encode(string input)
    {
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
        Console.WriteLine($"cipher='{encoded}', checksum='{checksum}'");
    }

    static void Decode(string cipher, string checksum)
    {
        byte[] step2 = Convert.FromBase64String(cipher);

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

    static Dictionary<string, string> ParseArgs(string[] args)
    {
        var dict = new Dictionary<string, string>();

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (arg.StartsWith("--"))
            {
                // flags with values
                if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                {
                    dict[arg] = args[i + 1];
                    i++;
                }
                else
                {
                    // boolean flags
                    dict[arg] = "true";
                }
            }
        }

        return dict;
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  --encode       Encode input, requires --text");
        Console.WriteLine("  --text <x>     Specify the text to encode");
        Console.WriteLine("  --decode       Decode input, requires --cipher and --checksum");
        Console.WriteLine("  --cipher <x>   Specify cipher to decode");
        Console.WriteLine("  --checksum     Specify the checksum of the cipher");
        Console.WriteLine("  --help         Show this help message");
    }
}
