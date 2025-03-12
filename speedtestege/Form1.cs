using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using Vlc.DotNet.Forms;


namespace speedtestege
{
    public partial class Form1 : Form
    {
        
    private VlcControl vlcControl;

        public Form1()
        {
            InitializeComponent();

            
        }

       

        private async void btnStartTest_Click(object sender, EventArgs e)
        {
            
            progressBar1.Value = 0;
            label3.Text = "Ping testi yapılıyor";
            label1.Text = "İndirme testi yapılıyor";
            label2.Text = "Yükleme testi yapılıyor";

            try
            {
                
                progressBar1.Value = 20;
                long pingResult = await TestPingAsync("8.8.8.8");
                textBox3.Text = $"Ping: {pingResult} ms";

                
                progressBar1.Value = 50;
                double downloadSpeed = await TestDownloadSpeedAsync("https://fsn1-speed.hetzner.com/100MB.bin");
                textBox1.Text = $"İndirme Hızı: {downloadSpeed:F2} Mbps";

                
                progressBar1.Value = 80;
                double uploadSpeed = await TestUploadSpeedAsync("https://httpbin.org/post");
                textBox2.Text = $"Yükleme Hızı: {uploadSpeed:F2} Mbps";

                
                progressBar1.Value = 100;
                MessageBox.Show("Hız testi tamamlandı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task<long> TestPingAsync(string host)
        {
            using (Ping ping = new Ping())
            {
                PingReply reply = await ping.SendPingAsync(host, 1000);
                return reply.RoundtripTime;
            }
        }

        
        private async Task<double> TestDownloadSpeedAsync(string downloadUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                byte[] data = await client.GetByteArrayAsync(downloadUrl);
                stopwatch.Stop();

                double seconds = stopwatch.Elapsed.TotalSeconds;
                double fileSizeInMB = data.Length / (1024.0 * 1024.0); 
                return (fileSizeInMB / seconds) * 8; 
            }
        }

        
        private async Task<double> TestUploadSpeedAsync(string uploadUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                byte[] testData = new byte[5 * 1024 * 1024]; 
                HttpContent content = new ByteArrayContent(testData);

                HttpResponseMessage response = await client.PostAsync(uploadUrl, content);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Yükleme testi başarısız!");

                double seconds = stopwatch.Elapsed.TotalSeconds;
                double fileSizeInMB = testData.Length / (1024.0 * 1024.0); 
                return (fileSizeInMB / seconds) * 8; 
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show("Label tıklandı!");
        }

    }
}

