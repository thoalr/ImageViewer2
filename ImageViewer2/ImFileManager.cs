using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer2
{
    class ImFileManager
    {
        // list or array of images from current directory
        FileInfo[] file_list;

        // current working list as array of indeces into directory list
        // to allow for searching
        int[] working_list;

        // current index into working list
        int current_file_index = 0;

        // current directory
       // DirectoryInfo currentDir = new DirectoryInfo(@"C:\");

        // current image
        //FileInfo imagefilepath;

        public ImManager image;

        // is current image gif
        //bool IsGif = false;
        

        // allowed exentsions
        // BMP, GIF, EXIF, JPG, PNG and TIFF.
        //TODO: Add webp support https://archive.codeplex.com/?p=webp
        /*
         image_codecs: Built-in BMP Codec, mime: image/bmp, extension: *.BMP;*.DIB;*.RLE
         image_codecs: Built-in JPEG Codec, mime: image/jpeg, extension: *.JPG;*.JPEG;*.JPE;*.JFIF
         image_codecs: Built-in GIF Codec, mime: image/gif, extension: *.GIF
         image_codecs: Built-in EMF Codec, mime: image/x-emf, extension: *.EMF
         image_codecs: Built-in WMF Codec, mime: image/x-wmf, extension: *.WMF
         image_codecs: Built-in TIFF Codec, mime: image/tiff, extension: *.TIF;*.TIFF
         image_codecs: Built-in PNG Codec, mime: image/png, extension: *.PNG
         image_codecs: Built-in ICO Codec, mime: image/x-icon, extension: *.ICO
        */
        static readonly string[] extensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };


        //FileSystemWatcher dir_watch;

        public ImFileManager(PictureBox pictureBox)
        {
            image = new ImManager(pictureBox);
            OpenNewFile();
        }

        public ImFileManager(FileInfo image_file, PictureBox pictureBox) 
        {
            //this.imagefilepath = image_file;

            GetFileList(image_file);

            image = new ImManager(pictureBox, image_file);

            //this.currentDir = this.imagefilepath.Directory;
            //OpenFile()
        }



        // valid fileextension
        private bool ValidExtension(String ext)
        {
            if (extensions.Contains(ext.ToLower())) return true;
            return false;
        }


        // open image
        public void OpenImage(FileInfo file) 
        {
            if(file.Exists && ValidExtension(file.Extension) )
            {
                image.Change_Image(file);
            }
            else
            {
                GetFileList(file);
            }
        
        }

        private FileInfo GetCurrentFile()
        {
            return file_list[working_list[current_file_index]];
        }

        private FileInfo GetFile(int i)
        {
            return file_list[working_list[i]];
        }

        // open file dialog
        public void OpenNewFile()
        {
            
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                DirectoryInfo last_dir = file_list == null ? new DirectoryInfo(@"C:\") : file_list[0].Directory;
                FileInfo last_file = null;

                if (file_list != null)
                {
                    last_file = this.GetCurrentFile();
                    openFileDialog1.FileName = last_file.Name;
                    
                }

                openFileDialog1.InitialDirectory = last_dir.FullName;

                //openFileDialog1.Filter = "image files|*.jpg;*.JPG;*.jpeg;*.png;*.gif|All files|*.*";
                openFileDialog1.Filter = "All files|*.*";

                //openFileDialog1.FilterIndex = 0;
                openFileDialog1.RestoreDirectory = true;

                // RefreshSection ?

                String file_ext = ".fake";
                do
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        
                        FileInfo new_file = new FileInfo(openFileDialog1.FileName);

                        DirectoryInfo new_dir = new_file.Directory;

                        GetFileList(new_file);

                        image.Change_Image(GetCurrentFile());
                        file_ext = GetCurrentFile().Extension;
                    }
                    else
                    {
                        return;
                    }
                } while (!ValidExtension(file_ext));
            }
        }


        private async Task SendKey(string FileName)
        {
            await Task.Delay(250); // Wait for the Dialog shown at the screen
            SendKeys.SendWait("+{TAB}"); // First Shift + Tab moves to Header of DataGridView of OpenFileDialog
            SendKeys.SendWait("+{TAB}"); // Second Shift + Tab moves to first item of list
            SendKeys.SendWait(FileName); // after sending filename will directly moves it to the file that we are looking for
        }


        /*
        private void SetDirWatcher(DirectoryInfo dir)
        {
            dir_watch = new FileSystemWatcher(dir.FullName, "*.*");
            dir_watch.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
            dir_watch.Changed += 
        }

        private void 
            */

        private void GetFileList(FileInfo file)
        {
            if (!file.Directory.Exists) throw new System.Exception("Directory does not exist");

            // Get files and order them
            var tmp = file.Directory.GetFiles("*.*", SearchOption.TopDirectoryOnly).Where(s => extensions.Contains(s.Extension.ToLower()));
            /*
            if (nameToolStripMenuItem.Checked)
            {
                if (ascendingToolStripMenuItem.Checked)
                    tmp = tmp.OrderBy(p => p.Name);
                else
                    tmp = tmp.OrderByDescending(p => p.Name);
            }
            else
            {
                if (ascendingToolStripMenuItem.Checked)
                    tmp = tmp.OrderBy(p => p.CreationTimeUtc);
                else
                    tmp = tmp.OrderByDescending(p => p.CreationTimeUtc);
            }
            */
            file_list = tmp.ToArray();

            working_list = Enumerable.Range(0, file_list.Length).ToArray();

            // Find index of current image
            get_working_index(file);
            //imagefilepath = file_list[working_list[list_index]];
        }

        // return index of file in working_list
        private int get_working_index(FileInfo file)
        {
            current_file_index = 0;
            for (int i = 0; i < working_list.Length; i++)
            {
                if (this.GetFile(i).FullName.Equals(file.FullName))
                {
                    // set list_index
                    current_file_index = i;
                    break;
                }
            }
            return current_file_index;
        }

        // next image
        public void ShowNextImage()
        {
            if (file_list == null || working_list == null) return;
            current_file_index = (current_file_index + 1) % (working_list.Length);
            image.Change_Image(GetCurrentFile());
        }


        // prev image
        public void ShowPreviousImage()
        {
            if (file_list == null || working_list == null) return;
            current_file_index = (current_file_index - 1 + working_list.Length) % (working_list.Length);
            image.Change_Image(GetCurrentFile());
        }

        // Refresh file list
        public void RefreshFileList()
        {
            if (file_list == null || working_list == null) return;
            FileInfo tmp = GetCurrentFile();
            GetFileList(tmp);
            //get_working_index(tmp); called by GetFileList(file)
        }


        // Rename current image
        public void RenameImage()
        {
            if (file_list == null || working_list == null) return;
            FileInfo tmp = GetCurrentFile();

            using(RenameForm form = new RenameForm())
            {
                form.FileName = tmp.Name;
                
                if (form.ShowDialog() == DialogResult.OK)
                {

                    FileInfo newfile = new FileInfo(tmp.DirectoryName + @"\" + form.FileName);
                    
                    if (newfile.Exists)
                    {
                        MessageBox.Show("New file name already exists");
                        return;
                    }

                    image.DisposeImage();

                    try
                    {
                        tmp.MoveTo(newfile.FullName);

                        GetFileList(newfile);

                        image.Change_Image(GetCurrentFile());
                    }
                    catch (Exception e)
                    {
                        image.Change_Image(tmp);
                    }


                    //GetFileList(newfile); // refresh

                }
            }


            }



        // search current directory list and update current working list
        // if current image not in new list show first image in list



        // clear search



        // text to display in titlebar of from
        // [i/n] - image path - Zoom: zoom_value
        public String get_form_text()
        {
            if (working_list == null || file_list == null || image == null) return "";
            else return "[" + (current_file_index + 1) + "/" + working_list.Length + "] " + file_list[working_list[current_file_index]].FullName + " - Zoom: " + image.get_zoom();
        }


    }
}
