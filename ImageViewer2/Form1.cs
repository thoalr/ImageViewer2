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
    public partial class Form1 : Form
    {
        ImFileManager manager;
        Point mouseLastPos = new Point(0, 0); // Last mouse position used for panning

        //ImManager image;

        // Default contructor causes file dialog to open when imageviewer called without file as argument
        public Form1()
        {
            InitializeComponent();
            ImageViewerInit(null);
        }

        // contructor which is called when imageviewer is opened with command line argument
        public Form1(String arg)
        {
            InitializeComponent();
            //Form1_Construct();
            ImageViewerInit(arg);
        }

        // ImageViewer initialze
        private void ImageViewerInit(String file)
        {
            this.WindowState = FormWindowState.Maximized;
            // Bring this form to the front of the screen
            this.BringToFront();
            //DoubleBuffered = true;
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            if (file == null)
            {
                manager = new ImFileManager(pictureBox1);
            }
            else
            {
                FileInfo temp = new FileInfo(file);
                manager = new ImFileManager(temp, pictureBox1);
                
            }


            update_form_text();

        }

        private void update_form_text()
        {
            this.Text = "ImageViewer - " + manager.get_form_text();
        }


        // Prev image
        private void button1_Click(object sender, EventArgs e)
        {
            manager.ShowPreviousImage();
            pictureBox1.Invalidate();
            update_form_text();
        }

        // next image
        private void button2_Click(object sender, EventArgs e)
        {
            manager.ShowNextImage();
            pictureBox1.Invalidate();
            update_form_text();
        }

        // Rename current image
        private void button3_Click(object sender, EventArgs e)
        {
            // open dialog box
        }

        // delete current image
        private void button4_Click(object sender, EventArgs e)
        {
            // open dialog box to ask for conformation
        }

        // gif previous frame
        private void button5_Click(object sender, EventArgs e)
        {

        }

        // gif next frame
        private void button6_Click(object sender, EventArgs e)
        {

        }

        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        // open new image by selecting from openfiledialog
        private void button7_Click(object sender, EventArgs e)
        {
            manager.OpenNewFile();
            pictureBox1.Invalidate();
            update_form_text();
        }

        // start pause gif
        private void button8_Click(object sender, EventArgs e)
        {
            if (manager.image.IsPlaying())
            {
                manager.image.StopGif();
                button8.Text = "Play";
            }
            else
            {
                manager.image.StartGif();
                button8.Text = "Pause";
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            manager.image.SetGifSpeed((int)numericUpDown1.Value);
        }
    }
}
