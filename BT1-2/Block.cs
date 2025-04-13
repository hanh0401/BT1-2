using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Security.Policy;

namespace BT1_2
{
    public class Block
    {
        public int Index { get; set; }
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = value.ToUniversalTime(); // Chuyển sang UTC
        }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }
        public string MerkleRoot { get; set; }
        public List<Transaction> Transactions { get; set; }

        public Block(int index, DateTime timestamp, string previousHash, List<Transaction> transactions)
        {
            Index = index;
            Timestamp = timestamp;
            PreviousHash = previousHash;
            Transactions = transactions;
            Nonce = 0;
            CalculateMerkleRoot();
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            //string blockData =
            //    Index.ToString() +
            //    Timestamp.ToString() +
            //    PreviousHash +
            //    Nonce.ToString() +
            //    MerkleRoot;

            string blockData = Index + Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") + PreviousHash + Nonce + MerkleRoot;
            return CryptoHelper.ComputeHash(blockData);
            //using (SHA256 sha256 = SHA256.Create())
            //{
            //    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockData));
            //    StringBuilder builder = new StringBuilder();
            //    for (int i = 0; i < bytes.Length; i++)
            //    {
            //        builder.Append(bytes[i].ToString("x2"));
            //    }
            //    return builder.ToString();
            //}
        }

        public void MineBlock(int difficulty)
        {
            string target = new string('0', difficulty);
            while (Hash == null || Hash.Substring(0, difficulty) != target)
            {
                Nonce++;
                Hash = CalculateHash();
            }
        }

        public void CalculateMerkleRoot()
        {
            List<string> transactionHashes = Transactions.ConvertAll(t => t.Hash ?? t.CalculateHash());
            MerkleRoot = transactionHashes.Count == 0 ? new string('0', 64) : BuildMerkleTree(transactionHashes);
        }

        //public void CalculateMerkleRoot()
        //{
        //    List<string> transactionHashes = new List<string>();
        //    foreach (var transaction in Transactions)
        //    {
        //        transactionHashes.Add(transaction.Hash ?? transaction.CalculateHash());
        //    }

        //    MerkleRoot = BuildMerkleTree(transactionHashes);
        //}

        private string BuildMerkleTree(List<string> transactionHashes)
        {
            if (transactionHashes.Count == 0)
                return new string('0', 64);

            if (transactionHashes.Count == 1)
                return transactionHashes[0];

            List<string> newHashes = new List<string>();

            for (int i = 0; i < transactionHashes.Count; i += 2)
            {
                //if (i + 1 < transactionHashes.Count)
                //{
                //    string combinedHash = HashPair(transactionHashes[i], transactionHashes[i + 1]);
                //    newHashes.Add(combinedHash);
                //}
                //else
                //{
                //    newHashes.Add(transactionHashes[i]);
                //}
                string hash = i + 1 < transactionHashes.Count
                ? CryptoHelper.HashPair(transactionHashes[i], transactionHashes[i + 1])
                : transactionHashes[i];
                newHashes.Add(hash);
            }

            return BuildMerkleTree(newHashes);
        }

        //private string HashPair(string hash1, string hash2)
        //{
        //    using (SHA256 sha256 = SHA256.Create())
        //    {
        //        string combinedHash = hash1 + hash2;
        //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedHash));
        //        StringBuilder builder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            builder.Append(bytes[i].ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}
    }
}
