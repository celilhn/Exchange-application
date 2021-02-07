using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Kur_Bilgileri
{
    public partial class Form1 : Form
    {
        int timer = 0;
        double totalAmount = 0;
        double totalGram = 0;
        XDocument tcmbdoviz = XDocument.Load("http://www.tcmb.gov.tr/kurlar/today.xml");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            ExchangeRate();
            CreateChart();

            timer1.Interval = 1000;
            timer1.Enabled = true;
        }

        private void ExchangeRate()
        {
            totalAmount = 0;
            totalGram = 0;

            DollarEuroRates("EURO", lbl_Euro_buy, lbl_Euro_sale, 1500, lbl_V_Euro);
            DollarEuroRates("ABD DOLARI", lbl_Dollar_buy, lbl_Dollar_sale, 1000, lbl_V_Dollar);

            GoldRates("https://altin.in/fiyat/cumhuriyet-altini", lbl_rebuplic_gold_buy, lbl_republic_gold_sale, 6, lbl_V_Republic_gold);
            GoldRates("https://altin.in/fiyat/yarim-altin", lbl_half_gold_buy, lbl_half_gold_sale, 0, lbl_V_Half_Gold);
            GoldRates("https://altin.in/fiyat/gram-altin", lbl_Gram_gold_buy, lbl_Gram_gold_sale, 361.5, lbl_V_Gram_Gold);

            var total = string.Format("{0:C} ₺",
                    totalAmount);
            total = total.Substring(1);
            lbl_Toplam_varlık.Text = total.ToString();

            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            listView1.Columns.Add("TARİH", 70);
            listView1.Columns.Add("TÜR", 40);
            listView1.Columns.Add("FİYAT", 65);
            listView1.Columns.Add("MİKTAR", 65);
            listView1.Columns.Add("KUR", 70);
            listView1.Columns.Add("DEĞER", 70);

            AddListRow(40.40, lbl_Gram_gold_buy.Text, "30.09.2019", "Altın", "10.000 ₺", "6 Tam", "271.039 ₺");
            AddListRow(20, lbl_Gram_gold_buy.Text, "30.09.2019", "Altın", "5.000 ₺", "20 Gram", "272.000 ₺");
            AddListRow(63.086, lbl_Gram_gold_buy.Text, "23.03.2020", "Altın", "20.000 ₺", "63,08 gr", "317.026 ₺");
            AddListRow(200.51, lbl_Gram_gold_buy.Text, "07.08.2020", "Altın", "100.000 ₺", "200,51 gr", "487.567 ₺");
            AddListRow(21.58, lbl_Gram_gold_buy.Text, "05.01.2021", "Altın", "10.000 ₺", "21,58 gr", "463.392 ₺");
            AddListRow(56.33, lbl_Gram_gold_buy.Text, "20.01.2021", "Altın", "25.500 ₺", "56,33 gr", "443.786 ₺");
        }

        private void DollarEuroRates(string value, Label lbl_Buy, Label lbl_Sale, double piece, Label presence)
        {
            var kurbilgileri = from kurlar in tcmbdoviz.Descendants("Currency")
                               where kurlar.Element("Isim").Value == value
                               select new
                               {
                                   kuradiing = kurlar.Element("CurrencyName").Value,
                                   kuralis = kurlar.Element("ForexBuying").Value,
                                   kursatis = kurlar.Element("ForexSelling").Value
                               };
            foreach (var veriler in kurbilgileri)
            {
                lbl_Buy.Text = string.Format("{0:C} ₺",
                    veriler.kuralis);

                lbl_Sale.Text = string.Format("{0:C} ₺",
                    veriler.kursatis);

                totalAmount += StringToFloat(veriler.kuralis) * piece;
            }

            presence.Text = piece.ToString();
        }

        private void GoldRates(string URL, Label lbl_Buy, Label lbl_Sale, double piece, Label presence)
        {
            var url = new Uri(URL); // url oluştruduk
            var client = new WebClient(); // siteye erişim için client tanımladık
            var html = client.DownloadString(url); //sitenin html lini indirdik
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); //burada HtmlAgilityPack Kütüphanesini kullandık
            doc.LoadHtml(html); // indirdiğimiz sitenin html lini oluşturduğumuz dokumana dolduruyoruz
            var veri = doc.DocumentNode.SelectNodes("//*[@id='icerik']/div[1]/div[2]/div[4]/ul/li[2]")[0]; // siteden aldığımız xpath i buraya yazıp kaynak kısmını seçiyoruz
            var satis = doc.DocumentNode.SelectNodes("//*[@id='icerik']/div[1]/div[2]/div[4]/ul/li[3]")[0];
            if (veri != null)
            {
                lbl_Buy.Text = veri.InnerHtml + " ₺";
                totalAmount += StringToFloat(veri.InnerHtml) * piece;
            }
            if (satis != null)
            {
                lbl_Sale.Text = satis.InnerHtml + " ₺";
            }

            presence.Text = piece.ToString();
        }

        private void AddListRow(double piece, string rate, string date, string type, string fee, string quantity, string dateRate)
        {
            totalGram += piece;

            var value = string.Format("{0:C} ₺",
                   (piece) * StringToFloat(rate));
            value = value.Substring(1);
            string[] row = { date, type, fee, quantity, dateRate, value };
            var satir = new ListViewItem(row);
            listView1.Items.Add(satir);
        }

        private void CreateChart()
        {
            double[] assets = new double[50];
            string[] dates = new string[50];
            int counter = 0;
            string filePath = @"C:\Users\Celilhan\Desktop\Programlar\Kur_Ozet.csv";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string article = sw.ReadLine();
            while (article != null)
            {
                var _split = article.Split(',');
                dates[counter] = _split[0];
                assets[counter] = StringToFloat(_split[1]);
                counter++;
                article = sw.ReadLine();
            }

            sw.Close();
            fs.Close();

            //Point leri temizleme.
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            //Grafiğe değer ekleme
            for (int i = 0; i < counter; i++)
            {
                //Grafiğe değer ekleme
                chart1.Series["Grafik"].Points.Add(assets[i]);

                //x ekseninde öğrenci isimlerini belirleme
                chart1.Series["Grafik"].Points[i].AxisLabel = dates[i];

            }

            //chart1.Series[0].ChartType = line
            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -70; // İsimler Dikey
            chart1.BackColor = Color.White;//Arka Plan rengi

        }

        float StringToFloat(string word)
        {
            float multiplier = 1;
            float result = 0;
            int pointerIndex = -1;

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == '.' || word[i] == ',')
                {
                    pointerIndex = i;
                }
            }

            if (pointerIndex == -1)
                pointerIndex = word.Length;

            for (int k = pointerIndex - 1; k >= 0; k--)
            {
                int temp = word[k] - 48;
                result += multiplier * temp;
                multiplier *= 10;
            }
            float dividing = 10;

            for (int j = pointerIndex + 1; j < word.Length; j++)
            {
                int temp = word[j] - 48;
                result += temp / dividing;
                dividing *= 10;
            }

            return result;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            int counter = timer++;

            if ((counter % 3) == 0)
            {
                totalAmount = 0;
                listView1.Clear();
                ExchangeRate();
            }
        }

        private void txt_Guess_Exchange_Rate_TextChanged(object sender, EventArgs e)
        {
            if (txt_Guess_Exchange_Rate.Text != "Tahmini kur")
            {
                var value = string.Format("{0:C} ₺",
                   (totalGram) * StringToFloat(txt_Guess_Exchange_Rate.Text));
                value = value.Substring(1);

                lbl_Estimated_Result.Text = value;
            }
        }
    }
}
