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
    public partial class ImplantForm : Form
    {
        public ImplantForm() //If you want to make settings for various encodings--do this in control--pass parameter
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
            picInStatusLabel.Text = checkPictureInBox();
        }

        private string checkPictureInBox()
        {
            String filePath = this.pictureInBox.Text;
            if (!File.Exists(filePath))
            {
                return "Error: File does not exist";
            }
            else
            {
                if (!(Path.GetExtension(filePath).ToLower().Equals(".png") || Path.GetExtension(filePath).ToLower().Equals(".jpg")))
                {
                    return "Error: File must be a PNG or a JPG";
                }
                else
                {
                    Bitmap b = new Bitmap(filePath);
                    int size = b.Height * b.Width;
                    calculatePossibleSize(size);
                    b.Dispose();
                    return "Size: " + size + " px. " + possibleSize + " bytes available.";
                }
            }
        }

        private void calculatePossibleSize(int imgSize)
        {
            this.possibleSize = (((imgSize - 2 * StegoHandler.START_LENGTH) / 512) - 8);
        }

        private void messageInBox_TextChanged(object sender, EventArgs e)
        {
            msgInStatusLabel.Text=checkMessageInBox();
        }

        private string checkMessageInBox()
        {
            pictureInBox_TextChanged(null, null);
            String filePath = this.messageInBox.Text;
            if (!File.Exists(filePath))
            {
                return "Error: File does not exist";
            }
            else
            {
                FileInfo f = new FileInfo(filePath);
                if (f.Length > this.possibleSize)
                {
                    return "Error: File size exceeds that available in image";
                }
                else
                {
                    return "File Size=" + f.Length + " bytes";
                }
            }
        }

        private void picOutBox_TextChanged(object sender, EventArgs e)
        {
            picOutStatusLabel.Text = checkPictureOutBox();
        }

        private string checkPictureOutBox()
        {
            String filePath = this.picOutBox.Text;
            if (filePath == null || filePath == "")
            {
                return "Error: No file path specified";
            }
            if (filePath.Equals(messageInBox.Text) || filePath.Equals(pictureInBox.Text))
            {
                return "Error: Cannot use same file";
            }
            else
            {
                FileInfo f = new FileInfo(filePath);
                if (!(f.Directory.Exists))
                {
                    return "Error: Directory does not exist";
                }
                else if (!Path.GetExtension(filePath).ToLower().Equals(".png"))
                {
                    return "Error: File must be a PNG";
                }
                else if (f.Exists && f.IsReadOnly)
                {
                    return "Error: File is read only";
                }
                else if (f.Exists)
                {
                    return "Warning: File will be overwritten";
                }
                else
                {
                    return "";
                }
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


        private void pass1Box_TextChanged(object sender, EventArgs e) //this is whack--should honestly add two more labels for passwords
        {
            pass1StatusLabel.Text = checkPass1Box();
        }

        private string checkPass1Box()
        {
            if (pass1Box.Text.Length < 8)
            {
                return "Error--password must be at least 8 characters";
            }
            else
            {
                return "";
            }
        }

        private void pass2Box_TextChanged(object sender, EventArgs e)
        {
            pass2StatusLabel.Text = checkPass2Box();
        }

        private string checkPass2Box()
        {
            if (!pass2Box.Text.Equals(pass1Box.Text))
            {
                return "Error--passwords do not match";
            }
            else
            {
                return "";
            }
        }

        private void runButton_Click(object sender, EventArgs e) //error checking needed here
        {
            if (checkPictureInBox().StartsWith("Error") ||
            checkMessageInBox().StartsWith("Error") ||
            checkPictureOutBox().StartsWith("Error") ||
            checkPass1Box().StartsWith("Error") ||
            checkPass2Box().StartsWith("Error"))
            {
                refreshAllLabels();
                primaryStatusLabel.Text = "Error--See messages above for details";
            }
            else
            {
                Thread t = new Thread(implantClick);
                t.IsBackground = true;
                t.Start();
            }
        }

        private void implantClick() //error checking needed here
        {
            SetPrimaryStatusLabelText("Implantation Running");
            string imgPath = pictureInBox.Text;
            string msgPath = messageInBox.Text;
            string outPath = picOutBox.Text;
            string password = pass1Box.Text;
            Bitmap b = new Bitmap(imgPath);
            byte[] msg = File.ReadAllBytes(msgPath);
            StegoHandler.implantMain(password, b, msg);
            b.Save(outPath);
            SetPrimaryStatusLabelText("Implantation Complete");
        }

        private void refreshAllLabels()
        {
            pictureInBox_TextChanged(null, null);
            messageInBox_TextChanged(null, null);
            picOutBox_TextChanged(null, null);
            pass1Box_TextChanged(null, null);
            pass2Box_TextChanged(null, null);
        }
    }
}
