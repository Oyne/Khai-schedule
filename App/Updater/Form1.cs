using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;

namespace Updater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            WebClient webClient = new WebClient();
            var client = new WebClient();

            try
            {
                System.Threading.Thread.Sleep(5000);
                string[] files = Directory.GetFiles(@".\");
                foreach (string file in files)
                {
                    var name = new FileInfo(file).Name;
                    if (name != "Updater.exe")
                    {
                        File.Delete(file);
                    }
                }
                client.DownloadFile("https://github.com/Oyne/Khai-schedule/raw/Updater/KhaiSchedule.zip", @"KhaiScedule.zip");
                string zipPath = @".\KhaiSchedule.zip";
                string extarctPath = @".\";
                ZipFile.ExtractToDirectory(zipPath, extarctPath);
                File.Delete(@".\KhaiSchedule.zip");
                Process.Start(@".\Updater.exe");
                this.Close();
            }
            catch
            {
                Process.Start(@".\Updater.exe");
                this.Close();
            }

        }
    }
}
