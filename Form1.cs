using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.Entity;
using Syncfusion.Windows.Forms.Chart;
using System.Globalization;

namespace DataVisualization.UI
{
    public partial class Form1 : Form
    {        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {     
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ddlPeriod.Text = "Today";           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            chartReceipts.Series.Clear();
            chartReleases.Series.Clear();

            if (ddlPeriod.Text == "Today")
            {
                DisplayToday();
            }
            else if (ddlPeriod.Text == "This Week")
                DisplayThisWeek();
            else if (ddlPeriod.Text == "This Month")
                DisplayThisMonth();
            else if (ddlPeriod.Text == "This Year")
                DisplayThisYear();

            lblLast1.Visible = ddlPeriod.Text == "Today";
            lblLast2.Visible = ddlPeriod.Text == "Today";
        }

        private List<spMonthlyActivity_Result> GetReceiptData()
        {
            using (var context = new WMS5Entities())
            {
                var day = new DateTime(DateTime.Today.Year, 1, 1);
                return context.spMonthlyActivity(day, "RECEIPT").ToList();
            }
        }
      
        private void LoadReceipts(List<spMonthlyActivity_Result> receiptData)
        {
            chartReceipts.PrimaryYAxis.Range = new MinMaxInfo(0, 10000, 2000);
           
            var day = new DateTime(DateTime.Today.Year, 1, 1);                

            ChartSeries series = new ChartSeries("Receipts", ChartSeriesType.Column);
            series.SortPoints = false;

            foreach (var data in receiptData)
            {
                series.Points.Add(((DateTime)data.BeginDate).ToString("MMM yy"), (double)data.Pallets);
            }

            chartReceipts.Series.Add(series);
            chartReceipts.Series[0].Style.DisplayText = true;
            chartReceipts.PrimaryXAxis.LabelRotate = false;               
            gaugeReceipts.MaximumValue = 120000;
            gaugeReceipts.MajorDifference = 20000;
            gaugeReceipts.MinorDifference = 10000;
            gaugeReceipts.Ranges.Clear();
            gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 40000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
            gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 40000, 80000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
            gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 80000, 120000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
            gaugeReceipts.GaugeLabel = "Received This Year";
            gaugeReceipts.Value = (float)receiptData.Sum(o => o.Pallets);     
        }

        private float GetOldReceiptData(List<spMonthlyActivity_Result> receiptData)
        {
            using (var context = new WMS5Entities())
            {
                var day = new DateTime(DateTime.Today.Year, 1, 1);
                var beginDate = day.AddYears(-1);
                var endDate = beginDate.AddDays(DateTime.Today.DayOfYear);
                var oldReceipts = (float)context.spTransSummary("RECEIPT", beginDate, endDate).First().Quantity;
                return ((float)(receiptData.Sum(o => o.Pallets) - oldReceipts) / oldReceipts);
            }
        }

        private void LoadOldReceipts(float value)
        {            
            lblReceipts.Text = value.ToString("p1");

            if (value < 0)
            {
                lblReceipts.ImageIndex = 0;
                lblReceiptChange.Text = "Receiving is DOWN";
            }
            else
            {
                lblReceipts.ImageIndex = 1;
                lblReceiptChange.Text = "Receiving is UP";
            }

            lblReceiptPeriod.Text = "Last Year";
        }

        private List<spMonthlyActivity_Result> GetReleaseData()
        {
            using (var context = new WMS5Entities())
            {
                var day = new DateTime(DateTime.Today.Year, 1, 1);
                return context.spMonthlyActivity(day, "RELEASE").ToList();
            }
        }

        private void LoadOldReleases(float value)
        {
            lblReleases.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReleases.ImageIndex = 0;
                    lblReleaseChange.Text = "Shipping is DOWN";
                }
                else
                {
                    lblReleases.ImageIndex = 1;
                    lblReleaseChange.Text = "Shipping is UP";
                }

                lblReleasePeriod.Text = "Last Year";
        }

        private float GetOldReleaseData (List<spMonthlyActivity_Result> releaseData)
        {
            using(var context = new WMS5Entities()){
                var day = new DateTime(DateTime.Today.Year, 1, 1); 
                var beginDate = day.AddYears(-1);
                var endDate = beginDate.AddDays(DateTime.Today.DayOfYear);
                var oldReleases = (float)context.spTransSummary("RELEASE", beginDate, endDate).First().Quantity;
                return ((float)(releaseData.Sum(o => o.Pallets) - oldReleases) / (oldReleases == 0 ? 1 : oldReleases));
            }
        }

        private void LoadReleases(List<spMonthlyActivity_Result> releaseData)
        {
            chartReleases.PrimaryYAxis.Range = new MinMaxInfo(0, 10000, 2000);            
            ChartSeries s2 = new ChartSeries("Releases", ChartSeriesType.Column);
            s2.SortPoints = false;

            foreach (var data in releaseData)
            {
                s2.Points.Add(((DateTime)data.BeginDate).ToString("MMM yy"), (double)data.Pallets);
            }

            chartReleases.Series.Add(s2);
            chartReleases.Series[0].Style.DisplayText = true;
            chartReleases.Series[0].Style.TextOffset = 10;                               
            gaugeReleases.MaximumValue = 120000;
            gaugeReleases.MajorDifference = 20000;
            gaugeReleases.MinorDifference = 10000;
            gaugeReleases.Ranges.Clear();
            gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 40000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
            gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 40000, 80000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
            gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 80000, 120000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
            gaugeReleases.GaugeLabel = "Released This Year";
            gaugeReleases.Value = (float)releaseData.Sum(o => o.Pallets);
            
        }

        private async void DisplayThisYear()
        {
            lblReceiptChange.Text = "";
            lblReleaseChange.Text = "";
            lblReceiptPeriod.Text = "";
            lblReleasePeriod.Text = "";
            label2.Text = "";
            label3.Text = "";
            gaugeReceipts.Value = 0;
            gaugeReleases.Value = 0;
            gaugeReceipts.GaugeLabel = "";
            gaugeReleases.GaugeLabel = "";
            lblReleases.ImageIndex = -1;
            lblReceipts.ImageIndex = -1;
            lblReceipts.Text = "";
            lblReleases.Text = "";
            var receiptData = await Task.Run(() => GetReceiptData());
            LoadReceipts(receiptData);
            var releaseData = await Task.Run(() => GetReleaseData());
            LoadReleases(releaseData);
            chartReceipts.Series[0].Style.DisplayText = true;
            chartReceipts.Series[0].Style.TextOrientation = ChartTextOrientation.Up;
            LoadOldReceipts(await Task.Run(() => GetOldReceiptData(receiptData)));
            label2.Text = "from this time";
            chartReceipts.PrimaryXAxis.LabelAlignment = StringAlignment.Center;
           
            chartReleases.Series[0].Style.DisplayText = true;
            chartReleases.Series[0].Style.TextOrientation = ChartTextOrientation.Up;
            chartReleases.PrimaryXAxis.LabelAlignment = StringAlignment.Center;          
            LoadOldReleases(await Task.Run(() => GetOldReleaseData(releaseData)));
            label3.Text = "from this time";
        }

        private void DisplayThisMonth()
        {
            chartReceipts.PrimaryYAxis.Range = new MinMaxInfo(0, 600, 100);
            chartReceipts.PrimaryXAxis.LabelAlignment = StringAlignment.Center;
            chartReleases.PrimaryYAxis.Range = new MinMaxInfo(0, 600, 100);
            chartReleases.PrimaryXAxis.LabelAlignment = StringAlignment.Center;

            using (var context = new WMS5Entities())
            {
                var day = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var receiptData = context.spDailyActivity(day, "RECEIPT").ToList();
                var releaseData = context.spDailyActivity(day, "RELEASE").ToList();

                ChartSeries series = new ChartSeries("Receipts", ChartSeriesType.Column);

                foreach (var data in receiptData)
                {
                    series.Points.Add(((DateTime)data.BeginDate).ToString("MMM dd"), (double)data.Pallets);
                }

                ChartSeries s2 = new ChartSeries("Releases", ChartSeriesType.Column);

                foreach (var data in releaseData)
                {
                    s2.Points.Add(((DateTime)data.BeginDate).ToString("MMM dd"), (double)data.Pallets);
                }

                chartReceipts.Series.Add(series);
                chartReleases.Series.Add(s2);               
                chartReceipts.PrimaryXAxis.LabelRotate = false;             
                chartReleases.Series[0].Style.TextOffset = 10;

                var beginDate = day.AddMonths(-1);
                var endDate = beginDate.AddDays(DateTime.Today.Day - 1);
                endDate = endDate.Add(DateTime.Now.TimeOfDay);

                var oldReceipts = (float)context.spTransSummary("RECEIPT", beginDate, endDate).First().Quantity;   

                float value = ((float)(receiptData.Sum(o => o.Pallets) - oldReceipts) / oldReceipts);

                lblReceipts.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReceipts.ImageIndex = 0;
                    lblReceiptChange.Text = "Receiving is DOWN";
                }
                else
                {
                    lblReceipts.ImageIndex = 1;
                    lblReceiptChange.Text = "Receiving is UP";
                }

                var oldReleases = (float)context.spTransSummary("RELEASE", beginDate, endDate).First().Quantity;

                value = ((float)(releaseData.Sum(o => o.Pallets) - oldReleases) / (oldReleases == 0 ? 1 : oldReleases));

                lblReleases.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReleases.ImageIndex = 0;
                    lblReleaseChange.Text = "Shipping is DOWN";
                }
                else
                {
                    lblReleases.ImageIndex = 1;
                    lblReleaseChange.Text = "Shipping is UP";
                }


                lblReceiptPeriod.Text = "Last Month";
                lblReleasePeriod.Text = "Last Month";

                gaugeReceipts.MaximumValue = 10000;
                gaugeReceipts.MajorDifference = 1000;
                gaugeReceipts.MinorDifference = 500;
                gaugeReceipts.Ranges.Clear();
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 3500, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 3500, 6500, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 6500, 10000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.GaugeLabel = "Received This Month";
                gaugeReceipts.Value = (float)receiptData.Sum(o => o.Pallets);

                gaugeReleases.MaximumValue = 10000;
                gaugeReleases.MajorDifference = 1000;
                gaugeReleases.MinorDifference = 500;
                gaugeReleases.Ranges.Clear();
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 3500, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 3500, 6500, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 6500, 10000, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.GaugeLabel = "Released This Month";
                gaugeReleases.Value = (float)releaseData.Sum(o => o.Pallets);

                chartReceipts.Series[0].Style.DisplayText = true;
                chartReceipts.Series[0].Style.TextOrientation = ChartTextOrientation.Up;

                chartReleases.Series[0].Style.DisplayText = true;
                chartReleases.Series[0].Style.TextOrientation = ChartTextOrientation.Up;
            }
        }

        private void DisplayThisWeek()
        {
            chartReceipts.PrimaryYAxis.Range = new MinMaxInfo(0, 600, 100);
            chartReceipts.PrimaryXAxis.LabelAlignment = StringAlignment.Center;
            chartReleases.PrimaryYAxis.Range = new MinMaxInfo(0, 600, 100);
            chartReleases.PrimaryXAxis.LabelAlignment = StringAlignment.Center;

            using (var context = new WMS5Entities())
            {
                var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
                var receiptData = context.spDailyActivity(monday, "RECEIPT").ToList();
                var releaseData = context.spDailyActivity(monday, "RELEASE").ToList();

                ChartSeries series = new ChartSeries("Receipts", ChartSeriesType.Column);
                series.SortPoints = false;
                foreach (var data in receiptData)
                {
                    series.Points.Add(((DateTime)data.BeginDate).ToString("ddd"), (double)data.Pallets);
                }
                                         
                ChartSeries s2 = new ChartSeries("Releases", ChartSeriesType.Column);
                s2.SortPoints = false;
                foreach (var data in releaseData)
                {
                    s2.Points.Add(((DateTime)data.BeginDate).ToString("ddd"), (double)data.Pallets);
                }

                chartReceipts.Series.Add(series);   
                chartReleases.Series.Add(s2);              
                chartReceipts.Series[0].Style.Callout.DisplayTextAndFormat = "{2}";
                
                chartReceipts.PrimaryXAxis.LabelRotate = false;             
                var beginDate = monday.AddDays(-7);
                var endDate = beginDate.AddDays((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday);
                endDate = endDate.Add(DateTime.Now.TimeOfDay);

                var oldReceipts = (float)context.spTransSummary("RECEIPT", beginDate, endDate).First().Quantity;   

                float value = ((float)(receiptData.Sum(o => o.Pallets) - oldReceipts) / oldReceipts);

                lblReceipts.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReceipts.ImageIndex = 0;
                    lblReceiptChange.Text = "Receiving is DOWN";
                }
                else
                {
                    lblReceipts.ImageIndex = 1;
                    lblReceiptChange.Text = "Receiving is UP";
                }

                var oldReleases = (float)context.spTransSummary("RELEASE", beginDate, endDate).First().Quantity;

                value = ((float)(releaseData.Sum(o => o.Pallets) - oldReleases) / (oldReleases == 0 ? 1 : oldReleases));

                lblReleases.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReleases.ImageIndex = 0;
                    lblReleaseChange.Text = "Shipping is DOWN";
                }
                else
                {
                    lblReleases.ImageIndex = 1;
                    lblReleaseChange.Text = "Shipping is UP";
                }


                lblReceiptPeriod.Text = "Last Week";
                lblReleasePeriod.Text = "Last Week";

                gaugeReceipts.MaximumValue = 2500;
                gaugeReceipts.MajorDifference = 500;
                gaugeReceipts.MinorDifference = 250;
                gaugeReceipts.Ranges.Clear();
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 750, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 750, 1750, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 1750, 2500, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.GaugeLabel = "Received This Week";
                gaugeReceipts.Value = (float)receiptData.Sum(o => o.Pallets);

                gaugeReleases.MaximumValue = 2500;
                gaugeReleases.MajorDifference = 500;
                gaugeReleases.MinorDifference = 250;
                gaugeReleases.Ranges.Clear();
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 750, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 750, 1750, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 1750, 2500, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.GaugeLabel = "Released This Week";
                gaugeReleases.Value = (float)releaseData.Sum(o => o.Pallets);

                chartReceipts.Series[0].Style.DisplayText = true;
                chartReceipts.Series[0].Style.TextOrientation = ChartTextOrientation.Up;

                chartReleases.Series[0].Style.DisplayText = true;
                chartReleases.Series[0].Style.TextOrientation = ChartTextOrientation.Up;
            }
        }

        private void DisplayToday()
        {           
            using (var context = new WMS5Entities())
            {
                var receiptData = context.spHourlyActivity(DateTime.Today.AddHours(6), "RECEIPT").ToList();
                var releaseData = context.spHourlyActivity(DateTime.Today.AddHours(6), "RELEASE").ToList();

                var max = ((int)(Math.Ceiling((double)receiptData.Max(o => o.Pallets) / 10))) * 10;
                var interval = max / 5;

                ChartSeries series = new ChartSeries("Receipts", ChartSeriesType.Column);
                series.SortPoints = false;
                chartReceipts.PrimaryYAxis.Range = new MinMaxInfo(0, max, interval);
                chartReleases.PrimaryYAxis.Range = new MinMaxInfo(0, max, interval);
                
                foreach (var data in receiptData)
                {
                    series.Points.Add(DateTime.Today.Add((TimeSpan)data.BeginTime).ToString("h tt"), (double)data.Pallets); //+ "-" + DateTime.Today.Add((TimeSpan)data.EndTime).ToString("hh tt"), (double)data.Pallets);
                }

                chartReceipts.Series.Add(series);              
                gaugeReceipts.Value = (float)receiptData.Sum(o => o.Pallets);

                ChartSeries s2 = new ChartSeries("Releases", ChartSeriesType.Column);
                s2.SortPoints = false;

                foreach (var data in releaseData)
                {
                    s2.Points.Add(DateTime.Today.Add((TimeSpan)data.BeginTime).ToString("h tt"), (double)data.Pallets);// + "-" + DateTime.Today.Add((TimeSpan)data.EndTime).ToString("hh"), (double)data.Pallets);
                }

                chartReleases.Series.Add(s2);               
                gaugeReleases.Value = (float)releaseData.Sum(o => o.Pallets);

                var beginDate = DateTime.Today.AddDays(-1);
                var endDate = DateTime.Now.AddDays(-1);

                var oldReceipts = (float)context.spTransSummary("RECEIPT", beginDate, endDate).First().Quantity;                

                float value = ((float)(receiptData.Sum(o => o.Pallets) - oldReceipts) / oldReceipts);

                lblReceipts.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReceipts.ImageIndex = 0;
                    lblReceiptChange.Text = "Receiving is DOWN";
                }
                else
                {
                    lblReceipts.ImageIndex = 1;
                    lblReceiptChange.Text = "Receiving is UP";
                }

                var oldReleases = (float)context.spTransSummary("RELEASE", beginDate, endDate).First().Quantity;

                value = ((float)(releaseData.Sum(o => o.Pallets) - oldReleases) / (oldReleases == 0 ? 1 : oldReleases));

                lblReleases.Text = value.ToString("p1");

                if (value < 0)
                {
                    lblReleases.ImageIndex = 0;
                    lblReleaseChange.Text = "Shipping is DOWN";
                }
                else
                {
                    lblReleases.ImageIndex = 1;
                    lblReleaseChange.Text = "Shipping is UP";
                }

                lblReceiptPeriod.Text = "Yesterday";
                lblReleasePeriod.Text = "Yesterday";

                gaugeReceipts.MaximumValue = 600;
                gaugeReceipts.MajorDifference = 60;
                gaugeReceipts.MinorDifference = 30;
                gaugeReceipts.Ranges.Clear();
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 180, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 180, 420, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 420, 600, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReceipts.GaugeLabel = "Received Today";
                gaugeReceipts.Value = (float)receiptData.Sum(o => o.Pallets);

                gaugeReleases.MaximumValue = 600;
                gaugeReleases.MajorDifference = 60;
                gaugeReleases.MinorDifference = 30;
                gaugeReleases.Ranges.Clear();
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Red, 0, 180, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Yellow, 180, 420, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.Ranges.Add(new Syncfusion.Windows.Forms.Gauge.Range(Color.Lime, 420, 600, 5, Syncfusion.Windows.Forms.Gauge.TickPlacement.Inside));
                gaugeReleases.GaugeLabel = "Released Today";
                gaugeReleases.Value = (float)releaseData.Sum(o => o.Pallets);

                chartReceipts.Series[0].Style.DisplayText = true;
                chartReceipts.Series[0].Style.TextOrientation = ChartTextOrientation.Up;

                chartReleases.Series[0].Style.DisplayText = true;
                chartReleases.Series[0].Style.TextOrientation = ChartTextOrientation.Up;
            }
        }
    }
}
