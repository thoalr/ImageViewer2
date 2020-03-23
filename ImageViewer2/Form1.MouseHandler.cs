/*
 * This file contains all methods for handling events caused by mouse interaction
 * 
 * 
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageViewer2
{
    public partial class Form1 : Form
    {
        // Abstract class used by method to get tilt wheel event from mouse
        abstract class Win32Messages
        {
            public const int WM_MOUSEHWHEEL = 0x020E;//discovered via Spy++ 
        }

        // Abstract class used by method to get tilt wheel event from mouse
        abstract class Utils
        {
            internal static Int32 HIWORD(IntPtr ptr)
            {
                Int32 val32 = ptr.ToInt32();
                return ((val32 >> 16) & 0xFFFF);
            }
            internal static Int32 LOWORD(IntPtr ptr)
            {
                Int32 val32 = ptr.ToInt32();
                return (val32 & 0xFFFF);
            }

        }

        // Horizontal Scroll START
        // Horizontal Scrolling from
        // http://www.philosophicalgeek.com/2007/07/27/mouse-tilt-wheel-horizontal-scrolling-in-c/
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (this.IsDisposed || m.HWnd != this.Handle)
            {
                return;
            }
            switch (m.Msg)
            {
                case Win32Messages.WM_MOUSEHWHEEL:
                    FireMouseHWheel(m.WParam, m.LParam);
                    m.Result = (IntPtr)1;
                    break;
                default:
                    break;
            }
        }

        public event EventHandler<MouseEventArgs> MouseHWheel;
        protected void FireMouseHWheel(IntPtr wParam, IntPtr lParam)
        {
            Int32 tilt = (Int16)Utils.HIWORD(wParam);
            Int32 keys = Utils.LOWORD(wParam);
            Int32 x = Utils.LOWORD(lParam);
            Int32 y = Utils.HIWORD(lParam);

            FireMouseHWheel(MouseButtons.None, 0, x, y, tilt);
        }

        protected void FireMouseHWheel(MouseButtons buttons, int clicks, int x, int y, int delta)
        {
            MouseEventArgs args = new MouseEventArgs(buttons,
                                         clicks, x, y, delta);
            OnMouseHWheel(args);
            //let everybody else have a crack at it
            MouseHWheel?.Invoke(this, args);
        }
        protected virtual void OnMouseHWheel(MouseEventArgs e)
        {
            /*
            if (e.Delta <= -10) // Select previous image in directory
            {
                if (isGIF) timer1.Stop();
                ChangeImage(-1);
                setGIF();
            }

            if (e.Delta >= 10) // Select next image in directory
            {
                if (isGIF) timer1.Stop();
                ChangeImage(1);
                setGIF();
            }
            */
        }

        private void panel2_Layout(object sender, LayoutEventArgs e)
        {

        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {


        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {



        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 0) return;
            manager.image.zoom_image(e);
            pictureBox1.Invalidate();
            update_form_text();
        }

        private void pictureBox1_Layout(object sender, LayoutEventArgs e)
        {
            if (manager == null) return;
            manager.image.rezise_reset();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle))
                pictureBox1.Capture = true;
            mouseLastPos = e.Location;
            
            if (e.Button == MouseButtons.XButton1)
            {
                manager.ShowPreviousImage();
                pictureBox1.Invalidate();
                update_form_text();
                return;
            }
            if (e.Button == MouseButtons.XButton2)
            {
                manager.ShowNextImage();
                pictureBox1.Invalidate();
                update_form_text();
                return;
            }
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pictureBox1.Capture || !(e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)) return;
            manager.image.move(e.X - mouseLastPos.X, e.Y - mouseLastPos.Y);
            mouseLastPos = e.Location;
            //manager.image.center_image();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Capture = false;
        }


        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            
        }
        // Go fullscreen or exit fullscreen on double click
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.WindowState = FormWindowState.Maximized;
                panel1.Height = 30;
            }
            else
            {
                panel1.Height = 0; // hide panel1 causes panel2 and therefore picturebox to take up entire screen
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.Activate();
            }
        }
    }
}
