using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chart_Replicator
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            //textBox1.Location.X = (this.Width - textBox1.Width) / 2;
            //textBox1.SelectionLength = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
