using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer2
{
    public partial class RenameForm : Form
    {
        public string FileName
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }



        public RenameForm()
        {
            InitializeComponent();
        }

        private void RenameForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (textBox1.Focused)
            {
                if (textBox1.Text.Length > 0 && keyData == Keys.Enter)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();

                }
            }
           
            return base.ProcessCmdKey(ref msg, keyData); // Forward the event to the default handler
        }


    }
}
