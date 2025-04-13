using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BT1_2;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Globalization;

namespace BT1_2
{
    public class Blockchain
    {
        public List<Block> Chain { get; set; }
        public List<Transaction> PendingTransactions { get; set; }
        public int Difficulty { get; set; }
        public RSAParameters PrivateKey { get; private set; }
        public RSAParameters PublicKey { get; private set; }

        public Blockchain()
        {
            Chain = new List<Block>();
            PendingTransactions = new List<Transaction>();
            Difficulty = 2;
            GenerateKeyPair();
            CreateGenesisBlock();
        }

        private void GenerateKeyPair()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                PrivateKey = rsa.ExportParameters(true);
                PublicKey = rsa.ExportParameters(false);
            }
        }

        private void CreateGenesisBlock()
        {
            Block genesisBlock = new Block(0, DateTime.Now, "0", new List<Transaction>());
            genesisBlock.MineBlock(Difficulty);
            Chain.Add(genesisBlock);
        }

        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        public void AddTransaction(Transaction transaction)
        {
            transaction.CalculateHash();
            transaction.Sign(PrivateKey);
            PendingTransactions.Add(transaction);

            if (PendingTransactions.Count >= 2)
            {
                CreateBlock();
            }
        }

        public void CreateBlock()
        {
            if (PendingTransactions.Count == 0)
                return;

            //Block latestBlock = GetLatestBlock();
            Block newBlock = new Block(
                GetLatestBlock().Index + 1,
                DateTime.Now,
                GetLatestBlock().Hash,
                new List<Transaction>(PendingTransactions)
            );

            newBlock.MineBlock(Difficulty);
            Chain.Add(newBlock);
            PendingTransactions.Clear();
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                // Verify the current block's hash
                if (currentBlock.Hash != currentBlock.CalculateHash())
                    return false;

                // Verify that the current block points to the previous block's hash
                if (currentBlock.PreviousHash != previousBlock.Hash)
                    return false;

                // Verify the Merkle root
                //string calculatedMerkleRoot = null;
                //currentBlock.CalculateMerkleRoot();
                //calculatedMerkleRoot = currentBlock.MerkleRoot;
                //if (calculatedMerkleRoot != currentBlock.MerkleRoot)
                //    return false;

                List<string> txHashes = currentBlock.Transactions.ConvertAll(t => t.Hash ?? t.CalculateHash());
                if (currentBlock.MerkleRoot != (txHashes.Count == 0 ? new string('0', 64) : BuildMerkleTree(txHashes)))
                    return false;

                // Verify each transaction's signature
                foreach (var transaction in currentBlock.Transactions)
                {
                    if (!transaction.VerifySignature(PublicKey))
                        return false;
                }
            }
            return true;
        }

        private string BuildMerkleTree(List<string> hashes)
        {
            if (hashes.Count == 1)
                return hashes[0];

            List<string> newHashes = new List<string>();
            for (int i = 0; i < hashes.Count; i += 2)
            {
                string hash = i + 1 < hashes.Count
                    ? CryptoHelper.HashPair(hashes[i], hashes[i + 1])
                    : hashes[i];
                newHashes.Add(hash);
            }
            return BuildMerkleTree(newHashes);
        }

        public void SaveToFile(string filePath)
        {
            foreach (var block in Chain)
            {
                if (block.MerkleRoot == null)
                {
                    block.CalculateMerkleRoot(); // Tính lại nếu null
                }
                foreach (var tx in block.Transactions)
                {
                    if (tx.Hash == null)
                    {
                        tx.CalculateHash();
                    }
                }
            }
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffZ", 
                Formatting = Newtonsoft.Json.Formatting.Indented,
                //Converters = new List<JsonConverter> { new UtcDateTimeConverter() },
                NullValueHandling = NullValueHandling.Include,
                Converters = new List<JsonConverter> { new DecimalFormatConverter() }
            };
            //string json = JsonConvert.SerializeObject(Chain, settings);
            //File.WriteAllText(filePath, json);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(Chain, settings));
        }

        public class UtcDateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string dateString = reader.Value?.ToString();
                return DateTime.Parse(dateString).ToUniversalTime();
            }

            public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
            }
        }
        public class DecimalFormatConverter : JsonConverter<decimal>
        {
            public override decimal ReadJson(JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string value = reader.Value?.ToString();
                return decimal.Parse(value, CultureInfo.InvariantCulture);
            }

            public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString("F2", CultureInfo.InvariantCulture));
            }
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                    Converters = new List<JsonConverter> { new DecimalFormatConverter() }
                };
                //string json = File.ReadAllText(filePath);
                Chain = JsonConvert.DeserializeObject<List<Block>>(File.ReadAllText(filePath), settings) ?? new List<Block>();

                // Make sure at least the genesis block exists
                if (Chain == null || Chain.Count == 0)
                {
                    //Chain = new List<Block>();
                    CreateGenesisBlock();
                }
                else
                {
                    foreach (var block in Chain)
                    {
                        if (block.MerkleRoot == null)
                        {
                            block.CalculateMerkleRoot(); // Tính lại nếu null
                        }
                        foreach (var tx in block.Transactions)
                        {
                            if (tx.Hash == null)
                            {
                                tx.CalculateHash();
                            }
                        }
                    }
                }
                PendingTransactions.Clear();
                // Reset pending transactions
                //PendingTransactions = new List<Transaction>();
            }
        }
    }
}
