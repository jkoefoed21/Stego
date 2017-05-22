using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Stego_Stuff
{
    public partial class ExtractForm : Form
    {
        public ExtractForm()
        {
            InitializeComponent();
        }

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
                    picInStatusLabel.Text = "";
                    b.Dispose();
                }
            }
        }

        private void messageOutBox_TextChanged(object sender, EventArgs e)
        {
            String filePath = this.messageOutBox.Text;
            FileInfo f = new FileInfo(filePath);
            if (!(f.Directory.Exists))
            {
                msgOutStatusLabel.Text = "Error: Directory does not exist";
            }
            else if (f.Exists && f.IsReadOnly)
            {
                msgOutStatusLabel.Text = "Error: File is read only";
            }
            else if (f.Exists)
            {
                msgOutStatusLabel.Text = "Warning: File will be overwritten";
            }
            else
            {
                msgOutStatusLabel.Text = "";
            }
        }

        private void passBox_TextChanged(object sender, EventArgs e)
        {
            if (passBox.Text.Length < 8)
            {
                passwordStatusLabel.Text = "Error--password must be at least 8 characters";
            }
            else
            {
                passwordStatusLabel.Text = "";
            }
        }

        private void pictureInSelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "PNG (*.png)|*.png|All Files (*.*)|*.*";
            od.ShowDialog();
            pictureInBox.Text = od.FileName;
        }

        private void messageOutSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sf.DefaultExt = ".txt";
            sf.ShowDialog();
            messageOutBox.Text = sf.FileName;
        }

        private void runButton_Click(object sender, EventArgs e) //as much error checking as possible here
        {
            Thread t = new Thread(extractClick);
            t.IsBackground = true;
            t.Start();
        }

        private void extractClick() //error checking needed here
        {
            string imgPath = pictureInBox.Text;
            string msgPath = messageOutBox.Text;
            string password = passBox.Text;
            Bitmap b = new Bitmap(imgPath);
            byte[] msg=StegoHandler.extractMain(password, b);
            File.WriteAllBytes(msgPath, msg);
        }
    }
}