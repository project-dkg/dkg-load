using dkgSolanaWalletGenerator.Services;
using System.IO.Compression;

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to Dkg Solana Wallet generator");

        while (true)
        {
            Console.Write("How many Solana wallets to generate? ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int numberOfWallets) && numberOfWallets > 0)
            {
                Console.Write("Enter the name of the file to write wallets to (default: wallets.csv): ");
                string? fileName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "wallets.csv";
                }

                Console.WriteLine($"Generating {numberOfWallets} Solana wallets to {fileName}");

                try
                {
                    using (StreamWriter writer = new(fileName))
                    {
                        for (int i = 0; i < numberOfWallets; i++)
                        {
                            var (solanaAddress, solanaPrivateKey) = KeyStoreService.Create();
                            writer.WriteLine($"{solanaAddress},{solanaPrivateKey}");

                            Console.Write($"\rGenerated wallet {i + 1} of {numberOfWallets}");
                        }
                    }

                    Console.WriteLine();

                    string zipFileName = $"{fileName}.zip";
                    using (FileStream zipToOpen = new(zipFileName, FileMode.Create))
                    {
                        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);

                        {
                            archive.CreateEntryFromFile(fileName, Path.GetFileName(fileName), CompressionLevel.SmallestSize);
                        }
                    }

                    Console.WriteLine($"File compressed to {zipFileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                    continue;
                }
                break;
            }
            else
            {
                Console.WriteLine("Error: Please enter a valid integer greater than 0.");
            }
        }
    }
}
