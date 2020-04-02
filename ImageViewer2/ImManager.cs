using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer2
{
    class ImManager
    {

        // https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image?view=netframework-4.8

        // image
        Bitmap Image;
        // image rectangle for src in draw funciton
        Rectangle im_rec;

        // Rectangle used to draw to screen
        Rectangle disp_rec;

        // coord
        //private Point image_offset = new Point(0, 0);

        // current zoom (% of screen height and/or width)
        private double zoom = 100;        // The zoom of the image
        private readonly double max_zoom = 3000;      // The maximum allowed zoom
        private readonly double min_zoom = 5;         // The minimum allowed zoom
        private double zoom_step = 1.2;

        // is gif
        bool isGif = false;

        // speed
        private int gif_speed = 100;

        // gif timer
        private Timer gif_timer;

        private FrameDimension fdimension;
        private int gifFrameCount = 1;  // Number of frames in the GIF image
        private int currentFrame = 0;   // Current frame displayed of GIF image

        // canvas reference
        PictureBox canvas;


        public ImManager(PictureBox picturebox)
        {

            this.canvas = picturebox;
            disp_rec = new Rectangle(0, 0, 0, 0);

            gif_timer = new Timer();
            gif_timer.Tick += TimerTick;

            picturebox.Paint += picturebox_Paint;
        }


        public ImManager(PictureBox picturebox, FileInfo file)
        {
            this.canvas = picturebox;
            disp_rec = new Rectangle(0, 0, 0, 0);

            gif_timer = new Timer();
            gif_timer.Tick += TimerTick;

            picturebox.Paint += picturebox_Paint;

            Change_Image(file);
        }


        // draw function draw image to screen
        private void picturebox_Paint(object sender, PaintEventArgs e)
        {
            if (Image == null)
            {
                e.Graphics.Clear(Color.Gray);
                return;
            }

            /*
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            */

            e.Graphics.DrawImage(this.Image, disp_rec, im_rec, GraphicsUnit.Pixel);
        }


        public void Change_Image(FileInfo file)
        {
            gif_timer.Stop();
            try
            {
                if (Image != null) Image.Dispose();
                
                Image = (Bitmap)Bitmap.FromFile(file.FullName);
                im_rec = new Rectangle(0, 0, Image.Width, Image.Height);
                rezise_reset();

                if (file.Extension.ToLower().Equals(".gif"))
                {
                    isGif = true;
                    setGIF();
                }
                else
                {
                    isGif = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error when opening image", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Image = null;
            }
        }

        // dispose image freeing file
        public void DisposeImage()
        {
            if (Image == null) return;
            if (isGif) gif_timer.Stop();
            isGif = false;
            Image.Dispose();
        }



        // play gif
        // if gif
        void setGIF()
        {
            gif_timer.Stop();
            if (isGif)
            {
                currentFrame = 0;
                //fdimension = new FrameDimension(Image.FrameDimensionsList[0]);

                gifFrameCount = Image.GetFrameCount(FrameDimension.Time);

                Image.SelectActiveFrame(FrameDimension.Time, 0);
                
                //gifFrameCount = Image.GetFrameCount(fdimension);

                /*
                Image.SelectActiveFrame(fdimension, currentFrame);


                // gif_speed = Image.GetPropertyItem(0x5100).Value;
                // BitConverter.ToInt32(times, 4*i); // size of byte array is 4*framecount in Timedimension
                PropertyItem[] t = Image.PropertyItems;
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Id == 0x5100) // Timing data
                    {
                        interval = (t[i].Value[0] + t[i].Value[1] * 256);
                        isGIF = true;
                    }
                }
                if (!isGIF) return;
                // The file has a gif extension but no timing data so it will be displayed as a static image
                */

                //isGIF = true;
                //gIFPropertyToolStripMenuItem.Enabled = true;

                gif_timer.Interval = gif_speed;
                gif_timer.Start();
            }
        }

        // play/pause
        private void TimerTick(object sender, EventArgs e)
        {
            currentFrame = (currentFrame + 1) % gifFrameCount;
            Image.SelectActiveFrame(FrameDimension.Time, currentFrame);
            canvas.Invalidate();
        }


        public void StopGif()
        {
            if (isGif)
            {
                gif_timer.Stop();

            }
        }
        public void StartGif()
        {
            if (isGif) gif_timer.Start();
        }

        // change frame function
        public void GifNextFrame()
        {
            if (isGif && !gif_timer.Enabled)
            {
                currentFrame = (currentFrame + 1) % gifFrameCount;
                Image.SelectActiveFrame(FrameDimension.Time, currentFrame);
                canvas.Invalidate();
            }
        }

        public void GifPreviousFrame()
        {
            if (isGif && !gif_timer.Enabled)
            {
                currentFrame = (currentFrame - 1) % gifFrameCount;
                Image.SelectActiveFrame(FrameDimension.Time, currentFrame);
                canvas.Invalidate();
            }
        }


        // change speed
        public void SetGifSpeed(int val)
        {
            StopGif();

            gif_speed = val;
            gif_timer.Interval = val;

            StartGif();
        }

        public int GetGifSpeed() { return gif_speed; }

        public bool IsGIF() { return isGif; } 
        public bool IsPlaying() { return gif_timer.Enabled; }

        // zoom function
        // zoom and move image so that it zooms in on cursor location if image size is bigger than screen height or width
        public void zoom_image(MouseEventArgs e)
        {
            //if (isGIF) timer1.Stop();
            double oldzoom = zoom;
            zoom = e.Delta < 0 ? zoom / zoom_step : zoom * zoom_step;
            if (zoom > max_zoom || zoom < min_zoom)
            {
                zoom = oldzoom;
                return;
            }

            //double new_width = (e.Delta < 0 ? disp_rec.Width / zoom_step : disp_rec.Width * zoom_step);
            //double new_height = e.Delta < 0 ? disp_rec.Height / zoom_step : disp_rec.Height * zoom_step;
            double new_height = Image.Height * zoom / 100;
            double new_width = Image.Width * zoom / 100;

            double width_delta = disp_rec.Width - new_width; // - disp_rec.Width;
            double height_delta = disp_rec.Height - new_height; // - disp_rec.Height;

            int mouse_rx = e.X - disp_rec.X;
            int mouse_ry = e.Y - disp_rec.Y;

            /* https://stackoverflow.com/questions/16349798/reposition-rectangle-after-zooming
            Width delta is the new width minus the old width.
            Height delta is the new height minus the old height.
            Mouse x relative is the x-coordinate relative to the rect's left side
            Mouse y relative is the y-coordinate relative to the rect's top side 
             */

            int x_delta = (int)(width_delta * mouse_rx / disp_rec.Width);
            int y_delta = (int)(height_delta * mouse_ry / disp_rec.Height);

            disp_rec.Height = (int)new_height;
            disp_rec.Width = (int)new_width;
            move(x_delta, y_delta);
            //zoom = zoom_constrain((double)disp_rec.Height * 100.0 / (double)Image.Height);

            //center_image();


            //if (isGIF) timer1.Start();

        }



        // move function
        // image only moves in dimension were image size is bigger than screen
        public void move(int dx, int dy)
        {
            if (disp_rec.Width > canvas.Width) disp_rec.X += dx;
            else disp_rec.X = ((canvas.Width - disp_rec.Width) / 2);

            if (disp_rec.Height > canvas.Height) disp_rec.Y += dy;
            else disp_rec.Y = ((canvas.Height - disp_rec.Height) / 2);
            /*
            if (disp_rec.X + dx < 0 || disp_rec.Right + dx > canvas.Width)
            {
                if (dx < 0) disp_rec.X = 0;
                else disp_rec.X = canvas.Width - disp_rec.Width;
            }
            else
            {
                disp_rec.X += dx;
            }
            if(disp_rec.Y + dy < 0 || disp_rec.Bottom + dy > canvas.Height)
            {
                if (dy < 0) disp_rec.Y = 0;
                else disp_rec.Y = canvas.Height - disp_rec.Height;
            }
            else
            {
                disp_rec.Y += dy;
            }
            */
        }


        // rezise function resize and center
        public void rezise_reset()
        {
            if (Image == null) return;

            zoom_to_fit();
            center_image();
        }

        private void zoom_to_fit()
        {
            if (Image.Width > Image.Height)
            {
                zoom = zoom_constrain((double)canvas.Width * 100.0 / (double)Image.Width);
                disp_rec.Width = (int)(Image.Width * zoom / 100);
                disp_rec.Height = (int)(Image.Height * zoom / 100);
                if (disp_rec.Height > canvas.Height)
                {
                    zoom = zoom_constrain((double)canvas.Height * 100.0 / (double)Image.Height);
                    disp_rec.Width = (int)(Image.Width * zoom / 100);
                    disp_rec.Height = (int)(Image.Height * zoom / 100);
                }
            }
            else
            {
                zoom = zoom_constrain((double)canvas.Height * 100.0 / (double)Image.Height);
                disp_rec.Width = (int)(Image.Width * zoom / 100);
                disp_rec.Height = (int)(Image.Height * zoom / 100);
                if (disp_rec.Width > canvas.Width)
                {
                    zoom = zoom_constrain((double)canvas.Width * 100.0 / (double)Image.Width);
                    disp_rec.Width = (int)(Image.Width * zoom / 100);
                    disp_rec.Height = (int)(Image.Height * zoom / 100);
                }
            }
            //return zoom;
        }


        private double zoom_constrain(double val)
        {
            if (val < min_zoom) return min_zoom;
            if (val > max_zoom) return max_zoom;
            return val;
        }

        public double get_zoom() { return zoom; }


        public void center_image()
        {
            if (Image == null) return;
            disp_rec.X = ((canvas.Width - disp_rec.Width) / 2);
            disp_rec.Y = ((canvas.Height - disp_rec.Height) / 2);
        }

        private Size rezise_to(Size s)
        {
            return disp_rec.Size = s;
        }

        // returns size  of image to be drawn on screen in pixels
        public Size draw_size()
        {
            return disp_rec.Size;
        }

        //private void change_canvas_size(Size new_size)
        //{
        //    this.canvas_size = new_size;
        //}




    }
}
