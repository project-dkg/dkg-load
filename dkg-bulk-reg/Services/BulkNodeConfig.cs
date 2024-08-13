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

using System.Text;
using Solnet.Wallet;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

using dkg.group;
using System.Text.Json;


namespace dkg_bulk_reg.Services
{
    public class BulkNodeConfig
    {
        [JsonIgnore]
        internal static Secp256k1Group G = new();

        [JsonIgnore]
        public int Index { get; set; }

        [JsonIgnore]
        public StringContent registerRequest { get; set; }

        public string Address { get; set; }
        public string PublicKey { get; set; }
        public string? Signature { get; set; }
        public void SelfSign()
        {
            string msg = $"{Address}{PublicKey}{Name}";
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            byte[] SignatureBytes = new PrivateKey(PrivateKey).Sign(msgBytes);
            Signature = Convert.ToBase64String(SignatureBytes);
        }

        [JsonPropertyName("Name")]
        public string Name
        {
            get { return $"Bulk node {Index}"; }
        }
        [JsonIgnore]
        public string PrivateKey { get; set; }
        public BulkNodeConfig(int index, string solanaAddress, string solanaPrivateKey)
        {
            Index = index;
            PrivateKey = solanaPrivateKey;
            Address = solanaAddress;

            var dkgPrivateKey = G.Scalar();
            var dkgPublicKey = G.Base().Mul(dkgPrivateKey);
            PublicKey = Convert.ToBase64String(dkgPublicKey.GetBytes());
            SelfSign();

            var jsonPayload = JsonSerializer.Serialize(this);
            registerRequest = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        }
        /*        public BulkNodeConfig(BulkNodeConfig other)
                {
                    Index = other.Index;
                    PublicKey = other.PublicKey;
                    Address = other.Address;
                    PrivateKey = other.PrivateKey;
                    Signature = other.Signature;
                }
        */
    }
}


