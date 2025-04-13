using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Globalization;

namespace BT1_2
{
    public class Transaction
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => _date = value.ToUniversalTime();
        }
        public decimal Amount { get; set; }
        public string ImageData { get; set; }
        public string Hash { get; set; }
        public string Signature { get; set; }

        public Transaction()
        {
            Date = DateTime.UtcNow;
        }

        public Transaction(string sender, string receiver, DateTime date, decimal amount, string imageData = null)
        {
            Sender = sender;
            Receiver = receiver;
            Date = date;
            Amount = amount;
            ImageData = imageData;
            CalculateHash();
        }

        public string CalculateHash()
        {
            string data = Sender + Receiver + Date.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") + Amount.ToString("F2", CultureInfo.InvariantCulture) + ImageData;
            Hash = CryptoHelper.ComputeHash(data);
            return Hash;
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
