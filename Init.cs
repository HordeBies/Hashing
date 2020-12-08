using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hashing
{
    public partial class Init : Form
    {
        public Init()
        {
            InitializeComponent();

        }

        private void Init_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text;
            string txt2 = textBox2.Text;
            string txt3 = textBox3.Text;
            int ikey;
            double ilf;
            int ibdl;

            if (!int.TryParse(txt, out ikey))
                ikey = 2;

            if(!double.TryParse(txt2,out ilf))
                ilf = 1;
            
            if (!int.TryParse(txt3, out ibdl))
                ibdl = 5;

            HashGUI gui = new HashGUI(ikey,ilf,ibdl);
            gui.Location = this.Location;
            gui.Show();
            this.Hide();
            
        }

        private void Init_LocationChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "2";
            textBox2.Text = "1";
            textBox3.Text = "5";
        }
    }
}
