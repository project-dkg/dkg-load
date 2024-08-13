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


using System.Diagnostics;
using System.Net;

namespace dkg_bulk_reg.Services
{
    public class BulkNodeRegister
    {
        private string ServiceNodeUrl;
        private HttpClient[] clients = [];

        private int recoverable;
        private int failures;

        public BulkNodeRegister(string serviceNodeUrl)
        {
            ServicePointManager.DefaultConnectionLimit = 2000;
            ServiceNodeUrl = serviceNodeUrl;
            clients = new HttpClient[5];

            for (int i = 0; i < clients.Length; i++)
            {
                clients[i] = new HttpClient();
            }
        }
        public async Task BulkRegister(BulkNodeConfig[] configs)
        {
            List<Task> tasks = [];

            failures = 0;
            recoverable = 0;

            Stopwatch sendwatch = new();
            Stopwatch processwatch = new();

            processwatch.Start();
            sendwatch.Start();

            for (int i = 0; i < configs.Length; i++)
            {
                Console.Write($"\rSending request {i+1} of {configs.Length}");
                tasks.Add(SendRegisterRequestAsync(configs[i]));
            }

            sendwatch.Stop();
            TimeSpan elapsed = sendwatch.Elapsed;
            Console.WriteLine($"\r{configs.Length} requests sent in {elapsed.TotalMilliseconds} milliseconds.");

            Console.WriteLine("Waiting for completion ...");
            await Task.WhenAll(tasks);
            processwatch.Stop();
            elapsed = processwatch.Elapsed;

            Console.WriteLine($"All requests completed in {elapsed.TotalMilliseconds} milliseconds.");
            Console.WriteLine($"Encountered {recoverable} recovered errord and {failures} unrecoverable errors.");
        }

        private async Task SendRegisterRequestAsync(BulkNodeConfig config)
        {
            int attempts = 0;
            bool success = false;
            while (attempts < 3 && !success)
            {
                try
                {
                    HttpResponseMessage response = await clients[config.Index % clients.Length]
                                                        .PostAsync(ServiceNodeUrl, config.registerRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        success = true;
                        // string responseBody = await response.Content.ReadAsStringAsync();
                        // Console.WriteLine($"Response: {responseBody}");
                    }
                    else
                    {
                        attempts++;
                        recoverable++;
                    }
                }
                catch (HttpRequestException)
                {
                    attempts++;
                    recoverable++;
                }
            }

            if (!success)
            {
                failures++;
            }
        }
    }
}


