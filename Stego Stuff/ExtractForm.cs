using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stego_Stuff
{
    public partial class ExtractForm : Form
    {
        public ExtractForm()
        {
            InitializeComponent();
        }

        private int possibleSize = 0;

        delegate void invokeSetPSLabelText(string text);

        /// <summary>
        /// To be able to adjust the PSL from other threads
        /// </summary>
        /// <param name="text"> text to change to </param>
        private void SetPrimaryStatusLabelText(string text)
        {
            if (primaryStatusLabel.InvokeRequired)
            {
                invokeSetPSLabelText d = new invokeSetPSLabelText(SetPrimaryStatusLabelText);
                Invoke(d, new object[] { text });
            }
            else
            {
                primaryStatusLabel.Text = text;
                primaryStatusLabel.Refresh();
            }
        }

        private void pictureInBox_TextChanged(object sender, EventArgs e)
        {
            String filePath = this.pictureInBox.Text;
            if (!File.Exists(filePath))
            {
                picInStatusLabel.Text = "Error: File does not exist";
            }
            else
            {
                if (!Path.GetExtension(filePath).Equals(".png"))
                {
                    picInStatusLabel.Text = "Error: File must be a PNG";
                }
                else
                {
                    Bitmap b = new Bitmap(filePath);
                    int size = b.Height * b.Width;
                    calculatePossibleSize(size);
                    b.Dispose();
                    picInStatusLabel.Text = "Size: " + size + " px. " + possibleSize + " bytes available.";
                }
            }
        }

        private void calculatePossibleSize(int imgSize)
        {
            this.possibleSize = (((imgSize - 2 * StegoHandler.START_LENGTH) / 512) - 8);
        }

        private void messageInBox_TextChanged(object sender, EventArgs e)
        {
            pictureInBox_TextChanged(null, null);
            String filePath = this.messageInBox.Text;
            if (!File.Exists(filePath))
            {
                picInStatusLabel.Text = "Error: File does not exist";
            }
            else
            {
                FileInfo f = new FileInfo(filePath);
                if (f.Length > this.possibleSize)
                {
                    msgInStatusLabel.Text = "Error: File size exceeds that available in image";
                }
                else
                {
                    msgInStatusLabel.Text = "File Size=" + f.Length + " bytes";
                }
            }
        }

        private void picOutBox_TextChanged(object sender, EventArgs e)
        {
            String filePath = this.picOutBox.Text;
            FileInfo f = new FileInfo(filePath);
            if (!(f.Directory.Exists))
            {
                picOutStatusLabel.Text = "Error: Directory does not exist";
            }
            else if (!Path.GetExtension(filePath).Equals(".png"))
            {
                picOutStatusLabel.Text = "Error: File must be a PNG";
            }
            else if (f.Exists && f.IsReadOnly)
            {
                picOutStatusLabel.Text = "Error: File is read only";
            }
            else if (f.Exists)
            {
                picOutStatusLabel.Text = "Warning: File will be overwritten";
            }
        }

        private void pictureInSelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg; *.jpg)|*.jpeg; *.jpg|All Files (*.*)|*.*";
            od.ShowDialog();
            pictureInBox.Text = od.FileName;
        }

        private void messageInSelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.ShowDialog();
            messageInBox.Text = od.FileName;
        }

        private void picOutSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PNG (*.png)|*.png|All Files (*.*)|*.*";
            sf.DefaultExt = "png";
            sf.ShowDialog();
            picOutBox.Text = sf.FileName;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(implantClick);
            t.IsBackground = true;
            t.Start();
        }

        private void implantClick()
        {
            string imgPath = pictureInBox.Text;
            string msgPath = messageInBox.Text;
            string outPath = picOutBox.Text;
            string password = pass1Box.Text;
            Bitmap b = new Bitmap(imgPath);
            byte[] msg = File.ReadAllBytes(msgPath);
            StegoHandler.implantMain(password, b, msg);
            b.Save(outPath);
        }

        private void pass1Box_TextChanged(object sender, EventArgs e)
        {
            if (pass1Box.Text.Length < 8)
            {
                primaryStatusLabel.Text = "Error--password must be at least 8 characters";
            }
        }

        private void pass2Box_TextChanged(object sender, EventArgs e)
        {
            if (!pass2Box.Text.Equals(pass1Box.Text))
            {
                primaryStatusLabel.Text = "Error--passwords do not match";
            }
        }

    }
}