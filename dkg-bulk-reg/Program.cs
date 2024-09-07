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

using System.IO.Compression;
using dkg_bulk_reg.Models;
using dkg_bulk_reg.Services;

namespace dkg_bulk_reg
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to Dkg Bulk register test");

            string? fileName = args.Length == 0 ? null : args[0];

            do
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    Console.Write("Please provide the name of the file to read from: ");
                    // R:\Projects\19.Projects\dkg-load\wallets.csv
                    fileName = Console.ReadLine();
                    fileName = fileName?.Trim();
                }
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    if (!File.Exists(fileName))
                    {
                        Console.WriteLine($"The file '{fileName}' does not exist.");
                        fileName = null;
                    }
                }
            } while (string.IsNullOrWhiteSpace(fileName));

            try
            {
                // Read and parse the file to generate BulkNodeConfig array
                BulkNodeConfig[] bulkNodeConfigs = ReadBulkNodeConfigsFromFile(fileName);
                Console.WriteLine($"\rRead {bulkNodeConfigs.Length} wallets from file.");
                var registrar = new BulkNodeRegister("https://dkg.samsonov.net:8081");
                //var registrar = new BulkNodeRegister("http://localhost:8080");
                await registrar.BulkRegister(bulkNodeConfigs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        static BulkNodeConfig[] ReadBulkNodeConfigsFromFile(string fileName)
        {
            // Check if the file is a zip file
            if (Path.GetExtension(fileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                string extractPath = Path.Combine(Path.GetDirectoryName(fileName) ?? string.Empty, Path.GetFileNameWithoutExtension(fileName));

                // Extract the zip file
                ZipFile.ExtractToDirectory(fileName, extractPath);

                // Assume the extracted file has the same name without the .zip extension
                fileName = Path.Combine(extractPath, Path.GetFileNameWithoutExtension(fileName));
            }

            List<BulkNodeConfig> bulkNodeConfigs = new();

            // Read and parse the contents of the file
            using (StreamReader reader = new(fileName))
            {
                string? line;
                int index = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.Write("\rReading wallet ... ");
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        string publicKey = parts[0];
                        string privateKey = parts[1];

                        BulkNodeConfig config = new(index++, publicKey, privateKey);
                        bulkNodeConfigs.Add(config);
                        Console.Write($"#{index} Ok");
                    }
                    else
                    {
                       Console.WriteLine($"invalid line: {line}");
                    }
                }
            }

            return [.. bulkNodeConfigs];
        }
    }
}
