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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace MikroSerialCom
{
    public partial class Form1 : Form
    {
        //tek cevrim boolu
        bool tekCevrim = true;

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
            DateTime now = DateTime.Now;
            string time = "[ " + now.ToString("HH:mm:ss") + " ]";
            // Verileri ekrana yazdırın
            this.Invoke(new Action(() => textBox3.Text += Environment.NewLine + time + " --> " + data)); // Alt satıra geçmek için Environment.NewLine sabitini kullan
            //textbox otomatik aşağı kaydırma
            this.Invoke(new Action(() => textBox3.SelectionStart = textBox3.Text.Length));
            this.Invoke(new Action(() => textBox3.ScrollToCaret()));


            // System.IndexOutOfRangeException: 'Dizin, dizi sınırlarının dışındaydı.' hatasını önlemek için
            if (data.Length < 2)
            {
                return;
            }

            char[] komut = new char[2];
            komut[0] = data[0];
            komut[1] = data[1];
            string komutString = new string(komut);

            //Kalan veriyi al
            data = data.Substring(2);

            //datanın içinde / varsa / işaretine kadar olan kısmı al
            if (data.Contains('/'))
            {
                data = data.Substring(0, data.IndexOf('/'));
            }

            // switch case yapısıyla gelen komutlara göre işlem yapılabilir
            switch (komutString)
            {
                case "/1":
                    this.Invoke(new Action(() =>
                    {
                        textBox1.Text += Environment.NewLine + time + " --> Buton 1 Basildi"; // Alt satıra geçmek için Environment.NewLine sabitini kullan
                                                                                              //textBox1.AppendText("Buton 1 Basildi. ");
                    }));
                    // otomatik aşağı kaydırma
                    this.Invoke(new Action(() => textBox1.SelectionStart = textBox1.Text.Length));
                    this.Invoke(new Action(() => textBox1.ScrollToCaret()));
                    break;
                case "/2":
                    this.Invoke(new Action(() =>
                    {
                        textBox2.Text += Environment.NewLine + time + " --> Buton 2 Basildi"; // Alt satıra geçmek için Environment.NewLine sabitini kullan
                                                                                              // Verileri textBox2'ye ekleyin
                                                                                              //textBox2.AppendText("Buton 2 Basildi. ");
                    }));
                    // otomatik aşağı kaydırma
                    this.Invoke(new Action(() => textBox2.SelectionStart = textBox2.Text.Length));
                    this.Invoke(new Action(() => textBox2.ScrollToCaret()));
                    break;
                case "/3":
                    //mesaj değişkenini label12 'ye yazdır
                    this.Invoke(new Action(() => label12.Text = data));
                    //mesaj değişkeni 12 bitlik bir ADC değeridir. Bunu 0-4095 aralığına çevir ve label13'e yazdır
                    int adc = Convert.ToInt32(data);
                    double volt = adc * 3.3 / 4095;
                    volt = Math.Round(volt, 4);
                    this.Invoke(new Action(() => label13.Text = volt.ToString() + "V"));
                    if (tekCevrim)
                    {
                        // seri porttan veri gönder
                        //this.Invoke(new Action(() => serialPort1.Write("/1" + volt.ToString() + "V" + "\n" + data + '\0')));
                        string mesaj1 = string.Format("/1{0}V\n{1}\0", volt, data);
                        // seri porttan veri gönder
                        this.Invoke(new Action(() => serialPort1.Write(mesaj1)));
                    }




                    break;
                default:
                    break;
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            tekCevrim = true;
            serialPort1.Write("/6" + '\0'); // 1 adc çevrimi
        }

        private void button9_Click(object sender, EventArgs e)
        {
            tekCevrim = false;
            serialPort1.Write("/7" + '\0'); // sürekli adc çevrimi
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tekCevrim = true;
            serialPort1.Write("/8" + '\0'); // adc durdur
        }
    }
}
