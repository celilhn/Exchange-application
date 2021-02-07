using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kur_Bilgileri
{
    public partial class Giriş : Form
    {
        public Giriş()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "kahrolsunpkk")
            {
                Form1 frm = new Form1();
                this.Hide();
                frm.Show();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textBox1.PasswordChar = '*';
        }
    }
}
