using System.Windows.Forms;
namespace Stego_Stuff
{
    partial class ControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.extractButton = new System.Windows.Forms.Button();
            this.filePathBox = new System.Windows.Forms.TextBox();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.runIndicatorLabel = new System.Windows.Forms.Label();
            this.fileBrowseButton = new System.Windows.Forms.Button();
            this.isRunningLabel = new System.Windows.Forms.Label();
            this.passwordCheckLabel = new System.Windows.Forms.Label();
            this.passwordCheckBox = new System.Windows.Forms.TextBox();
            this.implantButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(490, 593);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(150, 50);
            this.extractButton.TabIndex = 6;
            this.extractButton.Text = "Extract";
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // filePathBox
            // 
            this.filePathBox.Location = new System.Drawing.Point(40, 100);
            this.filePathBox.Name = "filePathBox";
            this.filePathBox.Size = new System.Drawing.Size(600, 26);
            this.filePathBox.TabIndex = 0;
            // 
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(40, 486);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.PasswordChar = '*';
            this.passwordBox.Size = new System.Drawing.Size(600, 26);
            this.passwordBox.TabIndex = 1;
            // 
            // filePathLabel
            // 
            this.filePathLabel.AutoSize = true;
            this.filePathLabel.Location = new System.Drawing.Point(36, 77);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(75, 20);
            this.filePathLabel.TabIndex = 7;
            this.filePathLabel.Text = "File Path:";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(36, 463);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(82, 20);
            this.passwordLabel.TabIndex = 8;
            this.passwordLabel.Text = "Password:";
            // 
            // runIndicatorLabel
            // 
            this.runIndicatorLabel.AutoSize = true;
            this.runIndicatorLabel.Location = new System.Drawing.Point(660, 220);
            this.runIndicatorLabel.Name = "runIndicatorLabel";
            this.runIndicatorLabel.Size = new System.Drawing.Size(0, 20);
            this.runIndicatorLabel.TabIndex = 11;
            this.runIndicatorLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // fileBrowseButton
            // 
            this.fileBrowseButton.Location = new System.Drawing.Point(660, 90);
            this.fileBrowseButton.Name = "fileBrowseButton";
            this.fileBrowseButton.Size = new System.Drawing.Size(150, 40);
            this.fileBrowseButton.TabIndex = 3;
            this.fileBrowseButton.Text = "Browse for File";
            this.fileBrowseButton.UseVisualStyleBackColor = true;
            this.fileBrowseButton.Click += new System.EventHandler(this.fileBrowseButton_Click);
            // 
            // isRunningLabel
            // 
            this.isRunningLabel.AutoSize = true;
            this.isRunningLabel.Location = new System.Drawing.Point(660, 160);
            this.isRunningLabel.Name = "isRunningLabel";
            this.isRunningLabel.Size = new System.Drawing.Size(0, 20);
            this.isRunningLabel.TabIndex = 10;
            // 
            // passwordCheckLabel
            // 
            this.passwordCheckLabel.AutoSize = true;
            this.passwordCheckLabel.Location = new System.Drawing.Point(36, 515);
            this.passwordCheckLabel.Name = "passwordCheckLabel";
            this.passwordCheckLabel.Size = new System.Drawing.Size(282, 20);
            this.passwordCheckLabel.TabIndex = 9;
            this.passwordCheckLabel.Text = "Reenter Password (for encryption only)";
            // 
            // passwordCheckBox
            // 
            this.passwordCheckBox.Location = new System.Drawing.Point(40, 538);
            this.passwordCheckBox.Name = "passwordCheckBox";
            this.passwordCheckBox.PasswordChar = '*';
            this.passwordCheckBox.Size = new System.Drawing.Size(600, 26);
            this.passwordCheckBox.TabIndex = 2;
            // 
            // implantButton
            // 
            this.implantButton.Location = new System.Drawing.Point(40, 593);
            this.implantButton.Name = "implantButton";
            this.implantButton.Size = new System.Drawing.Size(150, 50);
            this.implantButton.TabIndex = 5;
            this.implantButton.Text = "Implant";
            this.implantButton.UseVisualStyleBackColor = true;
            this.implantButton.Click += new System.EventHandler(this.implantButton_Click);
            // 
            // StegoWF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 669);
            this.Controls.Add(this.fileBrowseButton);
            this.Controls.Add(this.passwordCheckLabel);
            this.Controls.Add(this.isRunningLabel);
            this.Controls.Add(this.runIndicatorLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.filePathLabel);
            this.Controls.Add(this.passwordCheckBox);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.filePathBox);
            this.Controls.Add(this.extractButton);
            this.Controls.Add(this.implantButton);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "StegoWF";
            this.Text = "Rijndael";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button extractButton;
        private TextBox filePathBox;
        private TextBox passwordBox;
        private TextBox passwordCheckBox;
        private Label filePathLabel;
        private Label passwordLabel;
        private Label runIndicatorLabel;
        private Label isRunningLabel;
        private Label passwordCheckLabel;
        private Button fileBrowseButton;
        private Button implantButton;
    }
}

