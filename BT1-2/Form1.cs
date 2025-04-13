using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;

namespace BT1_2
{
    public partial class Form1 : Form
    {
        private Blockchain blockchain;
        private Transaction currentTransaction;
        private string encryptionKey;
        //private RichTextBox rtbMerkleTreeDisplay;

        public Form1()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            encryptionKey = CryptoHelper.GenerateRandomKey(32);
            //rtbMerkleTreeDisplay = rtbMerkleTree;
            UpdateBlockchainList();
        }

        private void btnCreateTransaction_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSender.Text) || string.IsNullOrWhiteSpace(txtReceiver.Text))
            {
                MessageBox.Show("Please enter both sender and receiver information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentTransaction = new Transaction(
                txtSender.Text,
                txtReceiver.Text,
                dtpDate.Value,
                numAmount.Value
            );

            rtbProcessing.Clear();
            rtbProcessing.AppendText("Transaction created:\n");
            rtbProcessing.AppendText($"Sender: {currentTransaction.Sender}\n");
            rtbProcessing.AppendText($"Receiver: {currentTransaction.Receiver}\n");
            rtbProcessing.AppendText($"Date: {currentTransaction.Date}\n");
            rtbProcessing.AppendText($"Amount: {currentTransaction.Amount:C}\n\n");

            btnHash.Enabled = true;
            btnEncrypt.Enabled = false;
            btnSign.Enabled = false;
            btnVerify.Enabled = false;
        }

        private void btnHash_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null)
                return;

            string hash = currentTransaction.CalculateHash();
            rtbProcessing.AppendText($"Hash calculation:\n");
            rtbProcessing.AppendText($"Input: {currentTransaction.Sender}|{currentTransaction.Receiver}|{currentTransaction.Date}|{currentTransaction.Amount}\n");
            rtbProcessing.AppendText($"Hash: {hash}\n\n");

            btnHash.Enabled = false;
            btnEncrypt.Enabled = true;
            btnSign.Enabled = false;
            btnVerify.Enabled = false;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null || string.IsNullOrEmpty(currentTransaction.Hash))
                return;

            string encrypted = CryptoHelper.Encrypt(currentTransaction.Hash, encryptionKey);
            rtbProcessing.AppendText($"Encryption:\n");
            rtbProcessing.AppendText($"Input: {currentTransaction.Hash}\n");
            rtbProcessing.AppendText($"Encrypted: {encrypted}\n\n");

            btnHash.Enabled = false;
            btnEncrypt.Enabled = false;
            btnSign.Enabled = true;
            btnVerify.Enabled = false;
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null || string.IsNullOrEmpty(currentTransaction.Hash))
                return;

            string signature = currentTransaction.Sign(blockchain.PrivateKey);
            rtbProcessing.AppendText($"Digital Signature:\n");
            rtbProcessing.AppendText($"Data signed: {currentTransaction.Hash}\n");
            rtbProcessing.AppendText($"Signature: {signature}\n\n");

            btnHash.Enabled = false;
            btnEncrypt.Enabled = false;
            btnSign.Enabled = false;
            btnVerify.Enabled = true;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null || string.IsNullOrEmpty(currentTransaction.Signature))
                return;

            bool isValid = currentTransaction.VerifySignature(blockchain.PublicKey);
            rtbProcessing.AppendText($"Signature Verification:\n");
            rtbProcessing.AppendText($"Verification result: {(isValid ? "Valid ✓" : "Invalid ✗")}\n\n");

            if (isValid)
            {
                blockchain.AddTransaction(currentTransaction);
                rtbProcessing.AppendText($"Transaction added to pending transactions.\n");
                rtbProcessing.AppendText($"Current pending transactions: {blockchain.PendingTransactions.Count}\n");

                if (blockchain.PendingTransactions.Count >= 5)
                {
                    rtbProcessing.AppendText($"Reached 5 transactions - Creating a new block...\n");

                    // Create the block
                    blockchain.CreateBlock();

                    // Update the display
                    UpdateBlockchainList();

                    // Select the latest block to display its details
                    if (lstBlocks.Items.Count > 0)
                    {
                        lstBlocks.SelectedIndex = lstBlocks.Items.Count - 1;
                    }
                }

                // Reset form for next transaction
                txtSender.Clear();
                txtReceiver.Clear();
                dtpDate.Value = DateTime.Now;
                numAmount.Value = 0;
                currentTransaction = null;

                btnHash.Enabled = false;
                btnEncrypt.Enabled = false;
                btnSign.Enabled = false;
                btnVerify.Enabled = false;
            }
        }

        private void lstBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBlocks.SelectedIndex >= 0 && lstBlocks.SelectedIndex < blockchain.Chain.Count)
            {
                Block block = blockchain.Chain[lstBlocks.SelectedIndex];
                DisplayBlockDetails(block, rtbBlockDetails);
                DisplayMerkleTree(block);
            }
        }

        private void DisplayBlockDetails(Block block, RichTextBox rtb)
        {
            rtb.Clear();
            rtb.AppendText($"Block #{block.Index}\n");
            rtb.AppendText($"Timestamp: {block.Timestamp}\n");
            rtb.AppendText($"Previous Hash: {block.PreviousHash}\n");
            rtb.AppendText($"Hash: {block.Hash}\n");
            rtb.AppendText($"Nonce: {block.Nonce}\n");
            rtb.AppendText($"Merkle Root: {block.MerkleRoot}\n\n");

            rtb.AppendText($"Transactions ({block.Transactions.Count}):\n");
            foreach (var transaction in block.Transactions)
            {
                rtb.AppendText($"- {transaction}\n");
                rtb.AppendText($"  Hash: {transaction.Hash}\n");
                rtb.AppendText($"  Signature: {(transaction.Signature?.Substring(0, Math.Min(20, transaction.Signature?.Length ?? 0)) + "...")}\n\n");
            }
        }

        // Add this new method for displaying Merkle tree
        private void DisplayMerkleTree(Block block)
        {
            rtbMerkleTree.Clear();

            if (block.Transactions.Count == 0)
            {
                rtbMerkleTree.AppendText("No transactions in this block to build Merkle tree.");
                return;
            }

            List<string> transactionHashes = new List<string>();
            rtbMerkleTree.AppendText("Merkle Tree for Block #" + block.Index + "\n\n");

            // Display transaction hashes (leaves of the tree)
            rtbMerkleTree.AppendText("Transaction Hashes (Leaves):\n");
            foreach (var transaction in block.Transactions)
            {
                string hash = transaction.Hash ?? transaction.CalculateHash();
                transactionHashes.Add(hash);
                rtbMerkleTree.AppendText($"- {hash.Substring(0, Math.Min(12, hash.Length))}...\n");
            }

            // Build and display the Merkle tree
            rtbMerkleTree.AppendText("\nMerkle Tree Structure:\n");
            List<List<string>> merkleTreeLevels = BuildMerkleTreeLevels(transactionHashes);

            for (int i = 0; i < merkleTreeLevels.Count; i++)
            {
                rtbMerkleTree.AppendText($"Level {i}: ");
                foreach (var hash in merkleTreeLevels[i])
                {
                    rtbMerkleTree.AppendText($"{hash.Substring(0, Math.Min(8, hash.Length))}... ");
                }
                rtbMerkleTree.AppendText("\n");
            }

            rtbMerkleTree.AppendText("\nMerkle Root: " + block.MerkleRoot);
        }

        private List<List<string>> BuildMerkleTreeLevels(List<string> leaves)
        {
            List<List<string>> levels = new List<List<string>>();
            levels.Add(new List<string>(leaves)); // Add leaf nodes

            while (levels[levels.Count - 1].Count > 1)
            {
                List<string> currentLevel = levels[levels.Count - 1];
                List<string> nextLevel = new List<string>();

                for (int i = 0; i < currentLevel.Count; i += 2)
                {
                    if (i + 1 < currentLevel.Count)
                    {
                        // Hash pair of nodes
                        string combinedHash = HashPair(currentLevel[i], currentLevel[i + 1]);
                        nextLevel.Add(combinedHash);
                    }
                    else
                    {
                        // Odd node - promote to next level
                        nextLevel.Add(currentLevel[i]);
                    }
                }

                levels.Add(nextLevel);
            }

            return levels;
        }

        private string HashPair(string hash1, string hash2)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combinedHash = hash1 + hash2;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedHash));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void UpdateBlockchainList()
        {
            int previousSelectedIndex = lstBlocks.SelectedIndex;
            lstBlocks.Items.Clear();

            foreach (var block in blockchain.Chain)
            {
                lstBlocks.Items.Add($"Block #{block.Index} - {block.Transactions.Count} transactions - {block.Timestamp}");
            }

            // Maintain selection if possible
            if (previousSelectedIndex >= 0 && previousSelectedIndex < lstBlocks.Items.Count)
            {
                lstBlocks.SelectedIndex = previousSelectedIndex;
            }
            else if (lstBlocks.Items.Count > 0)
            {
                // Select the newest block (last in the list)
                lstBlocks.SelectedIndex = lstBlocks.Items.Count - 1;
            }
        }

        private void btnSaveChain_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Save Blockchain"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                blockchain.SaveToFile(saveFileDialog.FileName);
                MessageBox.Show(
                    $"Blockchain saved to {saveFileDialog.FileName}",
                    "Save Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void btnLoadChain_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Load Blockchain"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    rtbLoadedChain.Text = json;

                    // Load the blockchain from file
                    blockchain.LoadFromFile(openFileDialog.FileName);

                    // Update the blockchain list to display loaded blocks
                    UpdateBlockchainList();

                    MessageBox.Show(
                        "Blockchain file loaded successfully!",
                        "Load Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error loading blockchain: {ex.Message}",
                        "Load Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void btnVerifyLoadedChain_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(rtbLoadedChain.Text))
                {
                    MessageBox.Show(
                        "Please load a blockchain file first.",
                        "Verification Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Parse the loaded JSON to a list of blocks
                List<Block> loadedChain = JsonConvert.DeserializeObject<List<Block>>(rtbLoadedChain.Text);

                // Use the improved verification function
                string verificationResult = VerifyBlockchain(loadedChain);

                if (verificationResult == "Valid")
                {
                    MessageBox.Show(
                        "Loaded blockchain is valid! No tampering detected.",
                        "Blockchain Verification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"Blockchain validation failed: {verificationResult}",
                        "Blockchain Verification",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error verifying blockchain: {ex.Message}",
                    "Verification Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Added comprehensive verification method
        // Fix the verification function
        private string VerifyBlockchain(List<Block> chain)
        {
            if (chain == null || chain.Count == 0)
                return "Empty blockchain";

            // Check genesis block
            if (chain[0].PreviousHash != "0")
                return "Invalid genesis block";

            // Verify each block
            for (int i = 1; i < chain.Count; i++)
            {
                Block currentBlock = chain[i];
                Block previousBlock = chain[i - 1];

                // Verify block index
                if (currentBlock.Index != previousBlock.Index + 1)
                    return $"Block #{currentBlock.Index} has invalid index";

                // Verify the previous hash pointer
                if (currentBlock.PreviousHash != previousBlock.Hash)
                    return $"Block #{currentBlock.Index} has invalid previous hash pointer";

                // Verify transaction data integrity
                foreach (var transaction in currentBlock.Transactions)
                {
                    // Calculate hash based on transaction data to check if data was modified
                    string transactionData =
                        transaction.Sender +
                        transaction.Receiver +
                        transaction.Date.ToString() +
                        transaction.Amount.ToString();

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(transactionData));
                        StringBuilder builder = new StringBuilder();
                        for (int j = 0; j < bytes.Length; j++)
                        {
                            builder.Append(bytes[j].ToString("x2"));
                        }

                        string calculatedHash = builder.ToString();

                        if (calculatedHash != transaction.Hash)
                            return $"Transaction in Block #{currentBlock.Index} has been tampered with";
                    }
                }

                // Recalculate Merkle root to verify transaction list integrity
                List<string> transactionHashes = new List<string>();
                foreach (var transaction in currentBlock.Transactions)
                {
                    transactionHashes.Add(transaction.Hash);
                }

                string calculatedMerkleRoot = "";
                if (transactionHashes.Count > 0)
                {
                    calculatedMerkleRoot = CalculateMerkleRoot(transactionHashes);
                    if (calculatedMerkleRoot != currentBlock.MerkleRoot)
                        return $"Block #{currentBlock.Index} has invalid Merkle root. Transaction data was tampered with.";
                }

                // Finally verify the block hash
                string blockData =
                    currentBlock.Index.ToString() +
                    currentBlock.Timestamp.ToString() +
                    currentBlock.PreviousHash +
                    currentBlock.Nonce.ToString() +
                    currentBlock.MerkleRoot;

                string calculatedBlockHash = CalculateHash(blockData);
                if (calculatedBlockHash != currentBlock.Hash)
                    return $"Block #{currentBlock.Index} has invalid hash. Block data was tampered with.";
            }

            return "Valid";
        }

        private string CalculateMerkleRoot(List<string> transactionHashes)
        {
            if (transactionHashes.Count == 0)
                return "0";

            if (transactionHashes.Count == 1)
                return transactionHashes[0];

            List<string> newHashes = new List<string>();

            for (int i = 0; i < transactionHashes.Count; i += 2)
            {
                if (i + 1 < transactionHashes.Count)
                {
                    string combinedHash = HashPair(transactionHashes[i], transactionHashes[i + 1]);
                    newHashes.Add(combinedHash);
                }
                else
                {
                    newHashes.Add(transactionHashes[i]);
                }
            }

            return CalculateMerkleRoot(newHashes);
        }

        // Helper method to calculate hash for verification
        private string CalculateHash(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Build Merkle root from transaction hashes
        //private string BuildMerkleRoot(List<string> transactionHashes)
        //{
        //    if (transactionHashes.Count == 0)
        //        return "0";

        //    if (transactionHashes.Count == 1)
        //        return transactionHashes[0];

        //    List<string> newLevel = new List<string>();

        //    for (int i = 0; i < transactionHashes.Count; i += 2)
        //    {
        //        if (i + 1 < transactionHashes.Count)
        //        {
        //            // Hash the pair of nodes
        //            string combinedHash = HashPair(transactionHashes[i], transactionHashes[i + 1]);
        //            newLevel.Add(combinedHash);
        //        }
        //        else
        //        {
        //            // Odd number of nodes, promote the last one
        //            newLevel.Add(transactionHashes[i]);
        //        }
        //    }

        //    // Recursively build the next level
        //    return BuildMerkleRoot(newLevel);
        //}

        
    }
}
