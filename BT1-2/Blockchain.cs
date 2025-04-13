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

            if (PendingTransactions.Count >= 5)
            {
                CreateBlock();
            }
        }

        public void CreateBlock()
        {
            if (PendingTransactions.Count == 0)
                return;

            Block latestBlock = GetLatestBlock();
            Block newBlock = new Block(
                latestBlock.Index + 1,
                DateTime.Now,
                latestBlock.Hash,
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
                string calculatedMerkleRoot = null;
                currentBlock.CalculateMerkleRoot();
                calculatedMerkleRoot = currentBlock.MerkleRoot;
                if (calculatedMerkleRoot != currentBlock.MerkleRoot)
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

        public void SaveToFile(string filePath)
        {
            string json = JsonConvert.SerializeObject(Chain, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                Chain = JsonConvert.DeserializeObject<List<Block>>(json);

                // Make sure at least the genesis block exists
                if (Chain == null || Chain.Count == 0)
                {
                    Chain = new List<Block>();
                    CreateGenesisBlock();
                }

                // Reset pending transactions
                PendingTransactions = new List<Transaction>();
            }
        }
    }
}
