// Copyright (C) 2024 Maxim [maxirmx] Samsonov (www.sw.consulting)
// All rights reserved.
// This file is a part of dkg load tests
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

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

/*                    string zipFileName = $"{fileName}.zip";
                    using (FileStream zipToOpen = new(zipFileName, FileMode.Create))
                    {
                        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);

                        {
                            archive.CreateEntryFromFile(fileName, Path.GetFileName(fileName), CompressionLevel.SmallestSize);
                        }
                    }

                    Console.WriteLine($"File compressed to {zipFileName}");
*/
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
