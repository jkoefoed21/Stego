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
    public partial class ImplantForm : Form
    {
        public ImplantForm() //If you want to make settings for various encodings--do this HERE
        {
            InitializeComponent();
        }

        private void pictureInBox_TextChanged(object sender, EventArgs e)
        {
            //this doesn't have to use delegates--basically calculate size and shit
        }

        private void messageInBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void picOutBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
