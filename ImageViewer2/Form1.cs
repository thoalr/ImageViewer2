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
            Text = "ImageViewer - " + manager.get_form_text();
        }

        // Previous image
        private void prev_image()
        {
            manager.ShowPreviousImage();
            pictureBox1.Invalidate();
            update_form_text();
        }
        // Next image
        private void next_image()
        {
            manager.ShowNextImage();
            pictureBox1.Invalidate();
            update_form_text();
        }

        // Open new image from filedialog
        private void open_new_image()
        {
            manager.OpenNewFile();
            pictureBox1.Invalidate();
            update_form_text();
        }



        // GIF control
        // Start stop
        private void gif_play_pause()
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

        // Next frame
        private void gif_prev_frame() { manager.image.GifNextFrame(); }

        // Previous frame
        private void gif_next_frame() { manager.image.GifPreviousFrame(); }

        // Set delay between frames to value
        private void gif_set_delay(int delay)
        {
            manager.image.SetGifSpeed(delay);
        }


        // Prev image
        private void button1_Click(object sender, EventArgs e)
        {
            prev_image();
        }

        // next image
        private void button2_Click(object sender, EventArgs e)
        {
            next_image();
        }

        // Rename current image
        private void button3_Click(object sender, EventArgs e)
        {
            manager.RenameImage();
            update_form_text();
        }

        // delete current image
        private void button4_Click(object sender, EventArgs e)
        {
            manager.DeleteImage();
            pictureBox1.Invalidate();
            update_form_text();
        }

        // gif previous frame
        private void button5_Click(object sender, EventArgs e)
        {
            manager.image.GifPreviousFrame();
        }

        // gif next frame
        private void button6_Click(object sender, EventArgs e)
        {

            manager.image.GifNextFrame();
        }

        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        // open new image by selecting from openfiledialog
        private void button7_Click(object sender, EventArgs e)
        {
            open_new_image();
        }

        // start pause gif
        private void button8_Click(object sender, EventArgs e)
        {
            gif_play_pause();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            gif_set_delay((int)numericUpDown1.Value);
        }

        // Refresh button
        private void button9_Click(object sender, EventArgs e)
        {
            manager.RefreshFileList();
            update_form_text();
        }

        // Seek to image
        private void button10_Click(object sender, EventArgs e)
        {
            manager.SeekToImage();
            pictureBox1.Invalidate();
            update_form_text();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
