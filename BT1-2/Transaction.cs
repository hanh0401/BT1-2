using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace BT1_2
{
    public class Transaction
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Hash { get; set; }
        public string Signature { get; set; }

        public Transaction()
        {
            Date = DateTime.Now;
        }

        public Transaction(string sender, string receiver, DateTime date, decimal amount)
        {
            Sender = sender;
            Receiver = receiver;
            Date = date;
            Amount = amount;
        }

        public string CalculateHash()
        {
            string data = Sender + Receiver + Date.ToString() + Amount.ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                Hash = builder.ToString();
                return Hash;
            }
        }

        public string Sign(RSAParameters privateKey)
        {
            string dataToSign = CalculateHash();
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);
                byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);
                byte[] signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                Signature = Convert.ToBase64String(signatureBytes);
                return Signature;
            }
        }

        public bool VerifySignature(RSAParameters publicKey)
        {
            if (string.IsNullOrEmpty(Signature) || string.IsNullOrEmpty(Hash))
                return false;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);
                byte[] dataBytes = Encoding.UTF8.GetBytes(Hash);
                byte[] signatureBytes = Convert.FromBase64String(Signature);
                return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public override string ToString()
        {
            return $"Sender: {Sender}, Receiver: {Receiver}, Date: {Date}, Amount: {Amount:C}";
        }
    }
}
