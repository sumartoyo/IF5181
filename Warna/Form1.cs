using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warna
{
    public partial class Form1 : Form
    {
        private ColorData ColorData;

        public Form1()
        {
            InitializeComponent();

            // set axis X label
            chart1.ChartAreas[0].AxisX.CustomLabels.Add(0.0, 127.0, "64");
            chart1.ChartAreas[0].AxisX.CustomLabels.Add(0.0, 255.0, "128");
            chart1.ChartAreas[0].AxisX.CustomLabels.Add(128.0, 255.0, "192");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private async void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //var input = Image.FromFile(openFileDialog1.FileName, true);
            //pictureBox1.Image = Processor.ResizeImageTo500(input);

            // clear
            label2.Text = "";
            chart1.Series["R"].Points.Clear();
            chart1.Series["G"].Points.Clear();
            chart1.Series["B"].Points.Clear();
            chart1.Series["Gray"].Points.Clear();

            // calculate
            ColorData = await Task.Run(() => Processor.Count(openFileDialog1.FileName));

            // show
            label2.Text = ColorData.CountUnique + "";
            for (int i = 0; i < 256; i++) {
                chart1.Series["R"].Points.AddXY(i, ColorData.CountR[i]);
                chart1.Series["G"].Points.AddXY(i, ColorData.CountG[i]);
                chart1.Series["B"].Points.AddXY(i, ColorData.CountB[i]);
                chart1.Series["Gray"].Points.AddXY(i, ColorData.CountGrayScale[i]);
            }
        }

        private void histogram_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series["R"].Enabled = checkBox1.Checked;
            chart1.Series["G"].Enabled = checkBox2.Checked;
            chart1.Series["B"].Enabled = checkBox3.Checked;
            chart1.Series["Gray"].Enabled = checkBox4.Checked;
        }
    }
}
