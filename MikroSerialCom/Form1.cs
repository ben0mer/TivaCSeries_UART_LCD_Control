using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; // SerialPort sınıfını kullanmak için gerekli kütüphane


namespace MikroSerialCom
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxComPort.Text; // Port ismini comboBox1'den al
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text); // BaudRate'i comboBox2'den al
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text); // DataBits'i comboBox2'den al
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text); // StopBits'i comboBox2'den al
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text); // Parity'i comboBox2'den al
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived); // Veri geldiğinde sp_DataReceived fonksiyonunu çağır
                serialPort1.Open(); // Portu aç
                durumLabel.ForeColor = Color.Green; // Durum label'ının yazı rengini yeşil yap
                durumLabel.Text = "Port Açık"; // Durum label'ını güncelle
                
                logger("Port Açıldı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); // Hata mesajı göster
                logger("Port Açılamadı.");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames(); // Bilgisayara bağlı portları diziye aktar
            cBoxComPort.Items.AddRange(ports); // Portları comboBox1'e ekle
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen) // Port açıksa
            {
                serialPort1.Close(); // Portu kapat
                durumLabel.ForeColor = Color.Red; // Durum label'ının yazı rengini kırmızı yap
                durumLabel.Text = "Port Kapalı"; // Durum label'ını güncelle
                logger("Port Kapatıldı.");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.Write("/1"+textBoxVeri.Text+"\n"+textBoxVeri2.Text + '\0'); // textBoxVeri'deki veriyi gönder
            //serialPort1.Write(textBoxVeri2.Text + '\0'); // textBoxVeri'deki veriyi gönder
            logger(textBoxVeri.Text+" ve "+ textBoxVeri2.Text +" Gönderildi.");
        }
        void logger(string data)
        {
            DateTime now = DateTime.Now;
            string time = "[ "+now.ToString("HH:mm:ss")+" ]";
            dataOut.Text += Environment.NewLine + time+" --> " + data; // Alt satıra geçmek için Environment.NewLine sabitini kullan
        }


        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.Write("/2"+ '\0'); // sayaç başlat
            logger("Sayaç Başlatıldı.");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            serialPort1.Write("/3" + '\0'); // sayaç durdur
            logger("Sayaç Durduruldu.");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            serialPort1.Write("/4" + '\0'); // sayaç sıfırla
            logger("Sayaç Sıfırlandı.");
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string time = now.ToString("HH:mm:ss");
            serialPort1.Write("/5" + time +'\0'); // saat gönder
            saatLabel.Text = time;
            logger("Saat Gönderildi.");
        }

        // Veri alındığında çalışacak olay işleyicisi
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Verileri okuyun
            string data = serialPort1.ReadExisting();

            // Veriyi parçalayın
            // Örneğin, "/1ABC" şeklinde bir veri geldiğini varsayalım
            // Bu veriyi başındaki "/" karakterine göre ikiye ayırırız
            // Böylece, parts[0] = "" ve parts[1] = "1ABC" olur
            string[] parts = data.Split('/');

            // Parts dizisinin uzunluğunu kontrol edin
            // Eğer parts dizisi iki elemandan az ise, hata mesajı verin veya veriyi atlayın
            if (parts.Length < 2)
            {
                // Hata mesajı verin
                MessageBox.Show("Geçersiz veri formatı: " + data);

                // Veriyi atlayın ve bir sonraki veriyi bekleyin
                return;
            }

            // Parçalanan verinin ikinci elemanını alın
            // Bu, başındaki karakteri ve kalan veriyi içerir
            // Örneğin, part = "1ABC"
            string part = parts[1];

            // Başındaki karakteri alın
            // Bu, hangi textbox'a yazılacağını belirler
            // Örneğin, ch = '1'
            char ch = part[0];

            // Kalan veriyi alın
            // Bu, textbox'a yazılacak olan veridir
            // Örneğin, value = "ABC"
            string value = part.Substring(1);

            // Başındaki karaktere göre bir koşul kullanın
            // Eğer '1' ise, textBox1'e yazın
            // Eğer '2' ise, textBox2'ye yazın
            // Başka bir karakter ise, hata mesajı verin
            if (ch == '1')
            {
                // Verileri textBox1'e yazdırmak için Invoke metodunu kullanın
                this.Invoke(new Action(() =>
                {
                    // Verileri textBox1'e ekleyin
                    textBox1.AppendText(value);
                }));
            }
            else if (ch == '2')
            {
                // Verileri textBox2'ye yazdırmak için Invoke metodunu kullanın
                this.Invoke(new Action(() =>
                {
                    // Verileri textBox2'ye ekleyin
                    textBox2.AppendText(value);
                }));
            }
            else
            {
                // Hata mesajı verin
                MessageBox.Show("Geçersiz veri formatı: " + data);
            }
        }

    }
}
