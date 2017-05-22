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
            this.implantButton = new System.Windows.Forms.Button();
            this.extractButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // implantButton
            // 
            this.implantButton.Location = new System.Drawing.Point(147, 82);
            this.implantButton.Name = "implantButton";
            this.implantButton.Size = new System.Drawing.Size(150, 50);
            this.implantButton.TabIndex = 5;
            this.implantButton.Text = "Implant";
            this.implantButton.UseVisualStyleBackColor = true;
            this.implantButton.Click += new System.EventHandler(this.implantButton_Click);
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(672, 64);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(150, 50);
            this.extractButton.TabIndex = 6;
            this.extractButton.Text = "Extract";
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 669);
            this.Controls.Add(this.extractButton);
            this.Controls.Add(this.implantButton);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ControlForm";
            this.Text = "Rijndael";
            this.ResumeLayout(false);

        }

        #endregion
        private Button implantButton;
        private Button extractButton;
    }
}

