﻿/*
 * This file conatins all functions for button events
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageViewer2
{
    public partial class Form1 : Form
    {
        

        //Point mouseLastPos = new Point(0, 0); // Last mouse position used for panning

        // Event handler for when a keyboard key is pressed
        private void Form1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            
            switch (e.KeyChar)
            {
                case '+': // zoom in
                    
                    break;
                case '-': // zoom out
                    
                    break;
                case 'o': // open new file from filedialog
                    manager.OpenNewFile();
                    pictureBox1.Invalidate();
                    break;
                case 's': // start stop timer for GIF
                    
                    break;
                case 'n': // next frame
                    
                    break;
                case 'b': // previous frame
                    
                    break;
                case 'x': // default speed as set by file
                    
                    break;
                case 'c': // speed increase so decrease interval
                    
                    break;
                case 'v': // speed decrease so increase interval
                    
                    break;
                case 'r':
                    
                    break;
            }
            
            update_form_text();

        }

        // event handler for when an arrow key is pressed
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            //TODO: On Keys.Enter check search bar for text and then search
            
            if (keyData == Keys.Left)
            {
                manager.ShowPreviousImage();
                pictureBox1.Invalidate();
            }
            if (keyData == Keys.Right)
            {
                manager.ShowNextImage();
                pictureBox1.Invalidate();
            }

            update_form_text();

            return base.ProcessCmdKey(ref msg, keyData); // Forward the event to the default handler
        }


    }
}
