namespace BT1_2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grpTransaction = new GroupBox();
            btnCreateTransaction = new Button();
            numAmount = new NumericUpDown();
            dtpDate = new DateTimePicker();
            txtReceiver = new TextBox();
            txtSender = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            grpProcessing = new GroupBox();
            rtbMerkleTree = new RichTextBox();
            label5 = new Label();
            btnVerify = new Button();
            btnSign = new Button();
            btnEncrypt = new Button();
            btnHash = new Button();
            rtbProcessing = new RichTextBox();
            grpBlockchain = new GroupBox();
            btnSaveChain = new Button();
            rtbBlockDetails = new RichTextBox();
            lstBlocks = new ListBox();
            grpFileOperations = new GroupBox();
            btnVerifyLoadedChain = new Button();
            btnLoadChain = new Button();
            rtbLoadedChain = new RichTextBox();
            btnSelectImage = new Button();
            pictureBoxImage = new PictureBox();
            grpTransaction.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAmount).BeginInit();
            grpProcessing.SuspendLayout();
            grpBlockchain.SuspendLayout();
            grpFileOperations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            SuspendLayout();
            // 
            // grpTransaction
            // 
            grpTransaction.Controls.Add(btnSelectImage);
            grpTransaction.Controls.Add(btnCreateTransaction);
            grpTransaction.Controls.Add(numAmount);
            grpTransaction.Controls.Add(dtpDate);
            grpTransaction.Controls.Add(txtReceiver);
            grpTransaction.Controls.Add(txtSender);
            grpTransaction.Controls.Add(label4);
            grpTransaction.Controls.Add(label3);
            grpTransaction.Controls.Add(label2);
            grpTransaction.Controls.Add(label1);
            grpTransaction.Location = new Point(12, 12);
            grpTransaction.Name = "grpTransaction";
            grpTransaction.Size = new Size(579, 150);
            grpTransaction.TabIndex = 0;
            grpTransaction.TabStop = false;
            grpTransaction.Text = "Tạo Transaction";
            // 
            // btnCreateTransaction
            // 
            btnCreateTransaction.Location = new Point(290, 110);
            btnCreateTransaction.Name = "btnCreateTransaction";
            btnCreateTransaction.Size = new Size(94, 29);
            btnCreateTransaction.TabIndex = 8;
            btnCreateTransaction.Text = "Tạo ";
            btnCreateTransaction.UseVisualStyleBackColor = true;
            btnCreateTransaction.Click += btnCreateTransaction_Click;
            // 
            // numAmount
            // 
            numAmount.Location = new Point(290, 47);
            numAmount.Name = "numAmount";
            numAmount.Size = new Size(104, 29);
            numAmount.TabIndex = 7;
            // 
            // dtpDate
            // 
            dtpDate.Location = new Point(6, 110);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(265, 29);
            dtpDate.TabIndex = 6;
            // 
            // txtReceiver
            // 
            txtReceiver.Location = new Point(146, 47);
            txtReceiver.Name = "txtReceiver";
            txtReceiver.Size = new Size(125, 29);
            txtReceiver.TabIndex = 5;
            // 
            // txtSender
            // 
            txtSender.Location = new Point(6, 49);
            txtSender.Name = "txtSender";
            txtSender.Size = new Size(125, 29);
            txtSender.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(290, 25);
            label4.Name = "label4";
            label4.Size = new Size(58, 21);
            label4.TabIndex = 3;
            label4.Text = "Số tiền";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 86);
            label3.Name = "label3";
            label3.Size = new Size(73, 21);
            label3.TabIndex = 2;
            label3.Text = "Ngày gửi";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(146, 25);
            label2.Name = "label2";
            label2.Size = new Size(93, 21);
            label2.TabIndex = 1;
            label2.Text = "Người nhận";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 25);
            label1.Name = "label1";
            label1.Size = new Size(80, 21);
            label1.TabIndex = 0;
            label1.Text = "Người gửi";
            // 
            // grpProcessing
            // 
            grpProcessing.Controls.Add(rtbMerkleTree);
            grpProcessing.Controls.Add(label5);
            grpProcessing.Controls.Add(btnVerify);
            grpProcessing.Controls.Add(btnSign);
            grpProcessing.Controls.Add(btnEncrypt);
            grpProcessing.Controls.Add(btnHash);
            grpProcessing.Controls.Add(rtbProcessing);
            grpProcessing.Location = new Point(12, 170);
            grpProcessing.Name = "grpProcessing";
            grpProcessing.Size = new Size(400, 474);
            grpProcessing.TabIndex = 1;
            grpProcessing.TabStop = false;
            grpProcessing.Text = "Xử lý Transaction";
            // 
            // rtbMerkleTree
            // 
            rtbMerkleTree.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 163);
            rtbMerkleTree.Location = new Point(6, 277);
            rtbMerkleTree.Name = "rtbMerkleTree";
            rtbMerkleTree.ReadOnly = true;
            rtbMerkleTree.Size = new Size(388, 191);
            rtbMerkleTree.TabIndex = 5;
            rtbMerkleTree.Text = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 253);
            label5.Name = "label5";
            label5.Size = new Size(88, 21);
            label5.TabIndex = 0;
            label5.Text = "Cây Merkle";
            // 
            // btnVerify
            // 
            btnVerify.Enabled = false;
            btnVerify.Location = new Point(299, 189);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new Size(85, 29);
            btnVerify.TabIndex = 4;
            btnVerify.Text = "Xác minh";
            btnVerify.UseVisualStyleBackColor = true;
            btnVerify.Click += btnVerify_Click;
            // 
            // btnSign
            // 
            btnSign.Enabled = false;
            btnSign.Location = new Point(300, 140);
            btnSign.Name = "btnSign";
            btnSign.Size = new Size(84, 29);
            btnSign.TabIndex = 3;
            btnSign.Text = "Ký";
            btnSign.UseVisualStyleBackColor = true;
            btnSign.Click += btnSign_Click;
            // 
            // btnEncrypt
            // 
            btnEncrypt.Enabled = false;
            btnEncrypt.Location = new Point(299, 93);
            btnEncrypt.Name = "btnEncrypt";
            btnEncrypt.Size = new Size(85, 29);
            btnEncrypt.TabIndex = 2;
            btnEncrypt.Text = "Mã hóa";
            btnEncrypt.UseVisualStyleBackColor = true;
            btnEncrypt.Click += btnEncrypt_Click;
            // 
            // btnHash
            // 
            btnHash.Enabled = false;
            btnHash.Location = new Point(299, 39);
            btnHash.Name = "btnHash";
            btnHash.Size = new Size(85, 29);
            btnHash.TabIndex = 1;
            btnHash.Text = "Băm";
            btnHash.UseVisualStyleBackColor = true;
            btnHash.Click += btnHash_Click;
            // 
            // rtbProcessing
            // 
            rtbProcessing.Location = new Point(6, 28);
            rtbProcessing.Name = "rtbProcessing";
            rtbProcessing.Size = new Size(287, 203);
            rtbProcessing.TabIndex = 0;
            rtbProcessing.Text = "";
            // 
            // grpBlockchain
            // 
            grpBlockchain.Controls.Add(btnSaveChain);
            grpBlockchain.Controls.Add(rtbBlockDetails);
            grpBlockchain.Controls.Add(lstBlocks);
            grpBlockchain.Location = new Point(597, 12);
            grpBlockchain.Name = "grpBlockchain";
            grpBlockchain.Size = new Size(383, 400);
            grpBlockchain.TabIndex = 2;
            grpBlockchain.TabStop = false;
            grpBlockchain.Text = "Blockchain";
            // 
            // btnSaveChain
            // 
            btnSaveChain.Location = new Point(8, 360);
            btnSaveChain.Name = "btnSaveChain";
            btnSaveChain.Size = new Size(125, 29);
            btnSaveChain.TabIndex = 3;
            btnSaveChain.Text = "Lưu chuỗi";
            btnSaveChain.UseVisualStyleBackColor = true;
            btnSaveChain.Click += btnSaveChain_Click;
            // 
            // rtbBlockDetails
            // 
            rtbBlockDetails.Location = new Point(139, 28);
            rtbBlockDetails.Name = "rtbBlockDetails";
            rtbBlockDetails.Size = new Size(236, 361);
            rtbBlockDetails.TabIndex = 1;
            rtbBlockDetails.Text = "";
            // 
            // lstBlocks
            // 
            lstBlocks.FormattingEnabled = true;
            lstBlocks.ItemHeight = 21;
            lstBlocks.Location = new Point(8, 28);
            lstBlocks.Name = "lstBlocks";
            lstBlocks.Size = new Size(125, 319);
            lstBlocks.TabIndex = 0;
            lstBlocks.SelectedIndexChanged += lstBlocks_SelectedIndexChanged;
            // 
            // grpFileOperations
            // 
            grpFileOperations.Controls.Add(btnVerifyLoadedChain);
            grpFileOperations.Controls.Add(btnLoadChain);
            grpFileOperations.Controls.Add(rtbLoadedChain);
            grpFileOperations.Location = new Point(430, 423);
            grpFileOperations.Name = "grpFileOperations";
            grpFileOperations.Size = new Size(550, 221);
            grpFileOperations.TabIndex = 3;
            grpFileOperations.TabStop = false;
            grpFileOperations.Text = "Mở File Blockchain";
            // 
            // btnVerifyLoadedChain
            // 
            btnVerifyLoadedChain.Location = new Point(414, 149);
            btnVerifyLoadedChain.Name = "btnVerifyLoadedChain";
            btnVerifyLoadedChain.Size = new Size(126, 29);
            btnVerifyLoadedChain.TabIndex = 2;
            btnVerifyLoadedChain.Text = "Xác minh File";
            btnVerifyLoadedChain.UseVisualStyleBackColor = true;
            btnVerifyLoadedChain.Click += btnVerifyLoadedChain_Click;
            // 
            // btnLoadChain
            // 
            btnLoadChain.Location = new Point(414, 65);
            btnLoadChain.Name = "btnLoadChain";
            btnLoadChain.Size = new Size(126, 29);
            btnLoadChain.TabIndex = 1;
            btnLoadChain.Text = "Tải File";
            btnLoadChain.UseVisualStyleBackColor = true;
            btnLoadChain.Click += btnLoadChain_Click;
            // 
            // rtbLoadedChain
            // 
            rtbLoadedChain.Location = new Point(6, 28);
            rtbLoadedChain.Name = "rtbLoadedChain";
            rtbLoadedChain.Size = new Size(390, 187);
            rtbLoadedChain.TabIndex = 0;
            rtbLoadedChain.Text = "";
            // 
            // btnSelectImage
            // 
            btnSelectImage.Location = new Point(439, 110);
            btnSelectImage.Name = "btnSelectImage";
            btnSelectImage.Size = new Size(94, 29);
            btnSelectImage.TabIndex = 9;
            btnSelectImage.Text = "Chọn ảnh";
            btnSelectImage.UseVisualStyleBackColor = true;
            btnSelectImage.Click += btnSelectImage_Click;
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.Location = new Point(421, 180);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new Size(159, 232);
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.TabIndex = 4;
            pictureBoxImage.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 653);
            Controls.Add(pictureBoxImage);
            Controls.Add(grpFileOperations);
            Controls.Add(grpBlockchain);
            Controls.Add(grpProcessing);
            Controls.Add(grpTransaction);
            Name = "Form1";
            Text = "Blockchain";
            grpTransaction.ResumeLayout(false);
            grpTransaction.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAmount).EndInit();
            grpProcessing.ResumeLayout(false);
            grpProcessing.PerformLayout();
            grpBlockchain.ResumeLayout(false);
            grpFileOperations.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpTransaction;
        private NumericUpDown numAmount;
        private DateTimePicker dtpDate;
        private TextBox txtReceiver;
        private TextBox txtSender;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button btnCreateTransaction;
        private GroupBox grpProcessing;
        private Button btnVerify;
        private Button btnSign;
        private Button btnEncrypt;
        private Button btnHash;
        private RichTextBox rtbProcessing;
        private GroupBox grpBlockchain;
        private Button btnSaveChain;
        private Button btnVerifyChain;
        private RichTextBox rtbBlockDetails;
        private ListBox lstBlocks;
        private GroupBox grpFileOperations;
        private Button btnVerifyLoadedChain;
        private Button btnLoadChain;
        private RichTextBox rtbLoadedChain;
        private Label label5;
        private RichTextBox rtbMerkleTree;
        private Button btnSelectImage;
        private PictureBox pictureBoxImage;
    }
}
