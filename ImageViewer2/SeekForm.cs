using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer2
{
    public partial class SeekForm : Form
    {


        public decimal Index
        {
            get { return numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }

        public decimal MaxIndex
        {
            get { return numericUpDown1.Maximum; }
            set { numericUpDown1.Maximum = value; }
        }





        public SeekForm()
        {
            InitializeComponent();

            listView1.Columns[0].Width = listView1.Width;
        }



        public void SetFileList(FileInfo[] filelist)
        {
            int i = 1;
            foreach (FileInfo file in filelist)
            {
                listView1.Items.Add(file.Name, i++);
            }

            listView1.Items[(int)numericUpDown1.Value - 1].Focused = true;
            listView1.Items[(int)numericUpDown1.Value - 1].Selected = true;


            listView1.EnsureVisible((int)numericUpDown1.Value - 1);

        }


        public void SetFileList(List<FileInfo> filelist)
        {
            int i = 1;
            foreach (FileInfo file in filelist)
            {
                listView1.Items.Add(file.Name, i++);
            }

            listView1.Items[(int)numericUpDown1.Value - 1].Focused = true;
            listView1.Items[(int)numericUpDown1.Value - 1].Selected = true;


            listView1.EnsureVisible((int)numericUpDown1.Value - 1);

        }



        private void SeekForm_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedIndices.Count > 0)
                numericUpDown1.Value = listView1.SelectedIndices[0]+1;
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

            if (keyData == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();

            }


            return base.ProcessCmdKey(ref msg, keyData); // Forward the event to the default handler
        }

    }
}
