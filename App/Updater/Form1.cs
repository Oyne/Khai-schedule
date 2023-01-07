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
                    if (name != "System.IO.Compression.dll" && name != "System.Net.Http.dll" && name != "Updater.exe.config" && name != "System.IO.Compression.FileSystem.dll" && name != "Updater.exe")
                    {
                        File.Delete(file);
                    }
                }
                client.DownloadFile("https://github.com/Oyne/Khai-schedule/raw/main/KhaiSchedule.zip", @"KhaiSchedule.zip");
                string zipPath = @".\KhaiSchedule.zip";
                string extarctPath = @".\";
                ZipFile.ExtractToDirectory(zipPath, extarctPath);
                File.Delete(@".\KhaiSchedule.zip");
                DialogResult result = MessageBox.Show("Приложение успешно обновлено", "Khai-schedule Update", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Process.Start(@".\App.exe");
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка:" + ex.ToString() + "\nСкорее всего Вы установили программу в защищённую папку, из-за чего не удаётся установить новую версию.\nВ данном случае нужно скачать и установить новую версию самостоятельно.");
                //Process.Start(@".\App.exe");
                this.Close();
            }

        }
    }
}
