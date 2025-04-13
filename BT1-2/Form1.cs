using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BT1_2
{
    public partial class Form1 : Form
    {
        private Blockchain blockchain;
        private Transaction currentTransaction;
        private string encryptionKey;
        private string encryptedHash;
        private string selectedImageBase64;

        public Form1()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            encryptionKey = CryptoHelper.GenerateRandomKey(32);
            selectedImageBase64 = null;
            UpdateBlockchainList();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp",
                Title = "Chọn ảnh"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Đọc ảnh và chuyển thành Base64
                        byte[] imageBytes = File.ReadAllBytes(dialog.FileName);
                        // Giới hạn kích thước ảnh (ví dụ: 1MB)
                        if (imageBytes.Length > 1_000_000)
                        {
                            MessageBox.Show("Ảnh quá lớn. Vui lòng chọn ảnh dưới 1MB.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        selectedImageBase64 = Convert.ToBase64String(imageBytes);

                        // Hiển thị ảnh trong PictureBox
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            pictureBoxImage.Image = Image.FromStream(ms);
                        }

                        rtbProcessing.AppendText("Ảnh đã được chọn.\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnCreateTransaction_Click(object sender, EventArgs e)
        {
            string send = txtSender.Text.Trim();
            string receiver = txtReceiver.Text.Trim();
            if (string.IsNullOrWhiteSpace(send) || string.IsNullOrWhiteSpace(receiver))
            {
                MessageBox.Show("Vui lòng nhập thông tin người gửi và người nhận.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (numAmount.Value <= 0)
            {
                MessageBox.Show("Số tiền phải lớn hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            currentTransaction = new Transaction(
                send,
                receiver,
                dtpDate.Value,
                numAmount.Value,
                selectedImageBase64
            );

            rtbProcessing.Clear();
            rtbProcessing.AppendText("Giao dịch được tạo:\n");
            rtbProcessing.AppendText($"Người nhận: {currentTransaction.Sender}\n");
            rtbProcessing.AppendText($"Người gửi: {currentTransaction.Receiver}\n");
            rtbProcessing.AppendText($"Ngày gửi: {currentTransaction.Date}\n");
            rtbProcessing.AppendText($"Số tiền: {currentTransaction.Amount:C}\n\n");
            rtbProcessing.AppendText($"Ảnh: {(string.IsNullOrEmpty(selectedImageBase64) ? "Không có" : "Có")}\n\n");

            btnHash.Enabled = true;
            btnEncrypt.Enabled = false;
            btnSign.Enabled = false;
            btnVerify.Enabled = false;

            // Reset ảnh sau khi tạo giao dịch
            selectedImageBase64 = null;
            pictureBoxImage.Image = null;
        }

        private void btnHash_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null)
                return;

            string hash = currentTransaction.CalculateHash();
            rtbProcessing.AppendText($"Băm:\n{hash}\n\n");

            btnHash.Enabled = false;
            btnEncrypt.Enabled = true;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null || string.IsNullOrEmpty(currentTransaction.Hash))
                return;
            encryptedHash = CryptoHelper.Encrypt(currentTransaction.Hash, encryptionKey);
            rtbProcessing.AppendText($"Mã hóa:\n");
            rtbProcessing.AppendText($"Băm: {currentTransaction.Hash}\n");
            rtbProcessing.AppendText($"Mã hóa: {encryptedHash}\n\n");

            btnEncrypt.Enabled = false;
            btnSign.Enabled = true;
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null || string.IsNullOrEmpty(currentTransaction.Hash))
                return;

            string signature = currentTransaction.Sign(blockchain.PrivateKey);
            rtbProcessing.AppendText($"Chữ ký số:\n");
            rtbProcessing.AppendText($"Dữ liệu ký: {currentTransaction.Hash}\n");
            rtbProcessing.AppendText($"Chữ ký: {signature.Substring(0, Math.Min(20, signature.Length))}...\n\n");

            btnSign.Enabled = false;
            btnVerify.Enabled = true;
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (currentTransaction == null || string.IsNullOrEmpty(currentTransaction.Signature))
                return;

            bool isValid = currentTransaction.VerifySignature(blockchain.PublicKey);
            rtbProcessing.AppendText($"Xác minh chữ ký:\nKết quả: {(isValid ? "Hợp lệ ✓" : "Không hợp lệ ✗")}\n\n");

            if (isValid)
            {
                blockchain.AddTransaction(currentTransaction);
                rtbProcessing.AppendText($"Giao dịch đã thêm vào danh sách chờ.\n");
                rtbProcessing.AppendText($"Số giao dịch chờ: {blockchain.PendingTransactions.Count}\n\n");

                bool newBlockCreated = blockchain.PendingTransactions.Count == 0;
                UpdateBlockchainList();

                if (newBlockCreated)
                {
                    rtbProcessing.AppendText("Đạt 2 giao dịch - Tạo khối mới...\n");
                    rtbProcessing.AppendText("Khối mới đã được tạo.\n\n");;
                }
                if (lstBlocks.Items.Count > 0)
                {
                    lstBlocks.SelectedIndex = lstBlocks.Items.Count - 1;
                }

                txtSender.Clear();
                txtReceiver.Clear();
                dtpDate.Value = DateTime.Now;
                numAmount.Value = 0;
                currentTransaction = null;

                btnHash.Enabled = false;
                btnEncrypt.Enabled = false;
                btnSign.Enabled = false;
                btnVerify.Enabled = false;
                CheckDisplay();
            }
        }

        private void CheckDisplay()
        {
            if (lstBlocks.Items.Count != blockchain.Chain.Count)
            {
                string errorMsg = $"Lỗi: lstBlocks hiển thị {lstBlocks.Items.Count} khối, thực tế có {blockchain.Chain.Count} khối.\n";
                rtbProcessing.AppendText(errorMsg);
                MessageBox.Show(errorMsg, "Lỗi hiển thị", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lstBlocks.SelectedIndex >= 0)
            {
                Block block = blockchain.Chain[lstBlocks.SelectedIndex];
                string blockDetails = rtbBlockDetails.Text;
                if (!blockDetails.Contains(block.Hash) || !blockDetails.Contains(block.MerkleRoot))
                    rtbProcessing.AppendText("Lỗi: rtbBlockDetails không hiển thị đúng chi tiết khối.\n");

                string merkleTree = rtbMerkleTree.Text;
                if (!merkleTree.Contains(block.MerkleRoot))
                    rtbProcessing.AppendText("Lỗi: rtbMerkleTree không hiển thị đúng gốc Merkle.\n");
            }
        }

        private void UpdateBlockchainList()
        {
            lstBlocks.Items.Clear();
            foreach (var block in blockchain.Chain)
                lstBlocks.Items.Add($"Khối #{block.Index} - {block.Transactions.Count} giao dịch - {block.Timestamp:O}");
            if (lstBlocks.Items.Count > 0 && lstBlocks.SelectedIndex == -1)
                lstBlocks.SelectedIndex = lstBlocks.Items.Count - 1;
        }
        private void lstBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBlocks.SelectedIndex >= 0)
            {
                Block block = blockchain.Chain[lstBlocks.SelectedIndex];
                rtbBlockDetails.Clear();
                rtbBlockDetails.AppendText($"Khối #{block.Index}\n");
                rtbBlockDetails.AppendText($"Thời gian: {block.Timestamp:yyyy-MM-ddTHH:mm:ss.fffffffZ}\n");
                rtbBlockDetails.AppendText($"Băm trước: {block.PreviousHash}\n");
                rtbBlockDetails.AppendText($"Băm: {block.Hash}\n");
                rtbBlockDetails.AppendText($"Nonce: {block.Nonce}\n");
                rtbBlockDetails.AppendText($"Gốc Merkle: {block.MerkleRoot}\n\n");
                rtbBlockDetails.AppendText($"Giao dịch ({block.Transactions.Count}):\n");
                foreach (var tx in block.Transactions)
                {
                    rtbBlockDetails.AppendText($"- {tx}\n");
                    if (!string.IsNullOrEmpty(tx.ImageData))
                    {
                        rtbBlockDetails.AppendText("  Có ảnh đính kèm.\n");
                    }
                }


                rtbMerkleTree.Clear();
                if (block.Transactions.Count == 0)
                {
                    rtbMerkleTree.AppendText("Không có giao dịch trong khối này.\n");
                    return;
                }

                rtbMerkleTree.AppendText($"Cây Merkle cho Khối #{block.Index}\n\n");
                List<string> txHashes = block.Transactions.ConvertAll(t => t.Hash ?? t.CalculateHash());
                rtbMerkleTree.AppendText("Băm giao dịch:\n");
                foreach (var hash in txHashes)
                    rtbMerkleTree.AppendText($"- {hash.Substring(0, Math.Min(12, hash.Length))}...\n");

                rtbMerkleTree.AppendText("\nCấu trúc cây Merkle:\n");
                var levels = BuildMerkleTreeLevels(txHashes);
                for (int i = 0; i < levels.Count; i++)
                {
                    rtbMerkleTree.AppendText($"Tầng {i}: ");
                    foreach (var hash in levels[i])
                        rtbMerkleTree.AppendText($"{hash.Substring(0, Math.Min(8, hash.Length))}... ");
                    rtbMerkleTree.AppendText("\n");
                }
                rtbMerkleTree.AppendText($"\nGốc Merkle: {block.MerkleRoot}\n");

                // Hiển thị ảnh nếu có
                pictureBoxImage.Image = null;
                if (block.Transactions.Any(tx => !string.IsNullOrEmpty(tx.ImageData)))
                {
                    var firstImageTx = block.Transactions.First(tx => !string.IsNullOrEmpty(tx.ImageData));
                    try
                    {
                        byte[] imageBytes = Convert.FromBase64String(firstImageTx.ImageData);
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            pictureBoxImage.Image = Image.FromStream(ms);
                        }
                    }
                    catch
                    {
                        rtbProcessing.AppendText("Không thể hiển thị ảnh từ giao dịch.\n");
                    }
                }
            }
        }

        private void DisplayBlockDetails(Block block)
        {
            rtbBlockDetails.Clear();
            rtbBlockDetails.AppendText($"Khối #{block.Index}\n");
            rtbBlockDetails.AppendText($"Timestamp: {block.Timestamp}\n");
            rtbBlockDetails.AppendText($"Băm trước: {block.PreviousHash}\n");
            rtbBlockDetails.AppendText($"Băm: {block.Hash}\n");
            rtbBlockDetails.AppendText($"Nonce: {block.Nonce}\n");
            rtbBlockDetails.AppendText($"Merkle Root: {block.MerkleRoot}\n\n");

            rtbBlockDetails.AppendText($"Giao dịch ({block.Transactions.Count}):\n");
            foreach (var transaction in block.Transactions)
            {
                rtbBlockDetails.AppendText($"- {transaction}\n");
                rtbBlockDetails.AppendText($"  Băm: {transaction.Hash}\n");
                rtbBlockDetails.AppendText($"  Chữ ký: {(transaction.Signature?.Substring(0, Math.Min(20, transaction.Signature?.Length ?? 0)) + "...")}\n\n");
            }
            rtbMerkleTree.Clear();
            if (block.Transactions.Count == 0)
            {
                rtbMerkleTree.AppendText("Không có giao dịch trong khối này.\n");
                return;
            }

            rtbMerkleTree.AppendText($"Cây Merkle cho Khối #{block.Index}\n\n");
            List<string> txHashes = block.Transactions.ConvertAll(t => t.Hash ?? t.CalculateHash());
            rtbMerkleTree.AppendText("Băm giao dịch:\n");
            foreach (var hash in txHashes)
                rtbMerkleTree.AppendText($"- {hash.Substring(0, Math.Min(12, hash.Length))}...\n");

            rtbMerkleTree.AppendText("\nCấu trúc cây Merkle:\n");
            var levels = BuildMerkleTreeLevels(txHashes);
            for (int i = 0; i < levels.Count; i++)
            {
                rtbMerkleTree.AppendText($"Tầng {i}: ");
                foreach (var hash in levels[i])
                    rtbMerkleTree.AppendText($"{hash.Substring(0, Math.Min(8, hash.Length))}... ");
                rtbMerkleTree.AppendText("\n");
            }
            rtbMerkleTree.AppendText($"\nGốc Merkle: {block.MerkleRoot}\n");
        }       
        private List<List<string>> BuildMerkleTreeLevels(List<string> hashes)
        {
            List<List<string>> levels = new List<List<string>> { new List<string>(hashes) };
            while (levels.Last().Count > 1)
            {
                List<string> current = levels.Last();
                List<string> nextLevel = new List<string>();
                for (int i = 0; i < current.Count; i += 2)
                {
                    string hash = i + 1 < current.Count
                        ? CryptoHelper.HashPair(current[i], current[i + 1])
                        : current[i];
                    nextLevel.Add(hash);
                }
                levels.Add(nextLevel);
            }
            return levels;
        }

        private void btnSaveChain_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", Title = "Lưu Blockchain" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    blockchain.SaveToFile(dialog.FileName);
                    MessageBox.Show($"Blockchain đã lưu vào {dialog.FileName}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnLoadChain_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog { Filter = "JSON files (*.json)|*.json", Title = "Tải Blockchain" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string json = File.ReadAllText(dialog.FileName);
                        rtbLoadedChain.Text = json;
                        blockchain.LoadFromFile(dialog.FileName);
                        UpdateBlockchainList();
                        MessageBox.Show("Tải blockchain thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (lstBlocks.Items.Count != blockchain.Chain.Count)
                            rtbProcessing.AppendText($"Lỗi: lstBlocks hiển thị {lstBlocks.Items.Count} khối, thực tế có {blockchain.Chain.Count} khối sau khi tải.\n");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnVerifyLoadedChain_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rtbLoadedChain.Text))
            {
                MessageBox.Show("Vui lòng tải tệp blockchain trước.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<Block> loadedChain = JsonConvert.DeserializeObject<List<Block>>(rtbLoadedChain.Text);
                string result = VerifyBlockchain(loadedChain);
                MessageBox.Show(
                    result == "Valid" ? "Blockchain hợp lệ!" : $"Blockchain không hợp lệ: {result}",
                    "Kết quả xác minh",
                    MessageBoxButtons.OK,
                    result == "Valid" ? MessageBoxIcon.Information : MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xác minh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string VerifyBlockchain(List<Block> chain)
        {
            if (chain == null || chain.Count == 0)
                return "Blockchain rỗng";

            if (chain[0].PreviousHash != "0" || chain[0].Index != 0)
                return "Khối gốc không hợp lệ";

            for (int i = 0; i < chain.Count; i++)
            {
                Block block = chain[i];
                if (block.Index != i)
                    return $"Khối #{block.Index} có chỉ số không hợp lệ (dự kiến: {i})";

                if (i > 0 && block.PreviousHash != chain[i - 1].Hash)
                    return $"Khối #{block.Index} có băm trước không khớp (PreviousHash: {block.PreviousHash}, Hash trước: {chain[i - 1].Hash})";

                foreach (var tx in block.Transactions)
                {
                    string hash = tx.CalculateHash();
                    if (hash != tx.Hash)
                        return $"Giao dịch trong khối #{block.Index} có băm không hợp lệ (tính toán: {hash}, lưu trữ: {tx.Hash})";
                }

                List<string> txHashes = block.Transactions.ConvertAll(t => t.Hash ?? t.CalculateHash());
                string merkleRoot = txHashes.Count == 0 ? new string('0', 64) : BuildMerkleTree(txHashes);
                if (merkleRoot != block.MerkleRoot)
                    return $"Khối #{block.Index} có gốc Merkle không hợp lệ (tính toán: {merkleRoot}, lưu trữ: {block.MerkleRoot})";

                string blockData = block.Index.ToString() + block.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ") + block.PreviousHash + block.Nonce.ToString() + block.MerkleRoot;
                string calculatedHash = CryptoHelper.ComputeHash(blockData);
                if (calculatedHash != block.Hash)
                    return $"Khối #{block.Index} có băm không hợp lệ (tính toán: {calculatedHash}, lưu trữ: {block.Hash})";
            }
            return "Valid";
        }

        private string BuildMerkleTree(List<string> hashes)
        {
            if (hashes.Count == 0)
                return new string('0', 64);
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
    }
}
