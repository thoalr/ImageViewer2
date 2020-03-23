using System;
using System.Collections.Generic;
using System.Drawing;
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
        double zoom = 100;        // The zoom of the image
        double max_zoom = 3000;      // The maximum allowed zoom
        double min_zoom = 5;         // The minimum allowed zoom
        double zoom_step = 1.2;

        // is gif
        bool isGif = false;

        // speed
        int gif_speed = 16;

        // gif timer
        Timer gif_timer;


        // canvas reference
        PictureBox canvas;


        public ImManager(PictureBox picturebox)
        {

            this.canvas = picturebox;
            disp_rec = new Rectangle(0, 0, 0, 0);

            //this.canvas = picturebox;
            picturebox.Paint += picturebox_Paint;
        }


        public ImManager(PictureBox picturebox, FileInfo file)
        {
            this.canvas = picturebox;
            disp_rec = new Rectangle(0, 0, 0, 0);

            //this.canvas = picturebox;
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
        /*
        public void Change_Image(String file)
        {
            try
            {
                Image = (Bitmap)Bitmap.FromFile(file);
                if(Path.GetExtension(file).ToLower().Equals(".gif"))
                {
                    isGif = true;
                }
                else
                {
                    isGif = false;
                }
            }
            catch (Exception e)
            {
                Image = null;
            }
        }
        */
        public void Change_Image(FileInfo file)
        {
            try
            {
                if (isGif) gif_timer.Stop();

                Image = (Bitmap)Bitmap.FromFile(file.FullName);
                im_rec = new Rectangle(0, 0, Image.Width, Image.Height);
                rezise_reset();

                if (file.Extension.ToLower().Equals(".gif"))
                {
                    isGif = true;
                }
                else
                {
                    isGif = false;
                }
            } catch (Exception e)
            {
                MessageBox.Show("Error when opening image", e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Image = null;
            }
        }


        // play gif
        // if gif


        // play/pause


        // change frame function

        // change speed


        // zoom function
        // zoom and move image so that it zooms in on cursor location if image size is bigger than screen height or width
        public void zoom_image(MouseEventArgs e)
        {
            //if (isGIF) timer1.Stop();
            double oldzoom = zoom;
            zoom = e.Delta < 0 ? zoom / zoom_step : zoom * zoom_step;
            if (zoom > max_zoom || zoom < min_zoom )
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
            if(disp_rec.Width > canvas.Width) disp_rec.X += dx;
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
