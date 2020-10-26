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

namespace CPUPerformance
{
    public partial class Form1 : Form
    {
        PerformanceCounter perfCounter = new PerformanceCounter("System","System Up Time");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text ="System Up Time";
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //CPU.CategoryName = "Processor";
            float cpu = CPU.NextValue();
            progressBar1.Value = (int)cpu;

            label2.Text = "CPU  " + string.Format("{0:0.00}%",cpu) ;
            chart1.Series["CPU"].Points.AddY(cpu);

            float ram = RAM1.NextValue();
            progressBar2.Value = (int)ram;

            label3.Text = "RAM  " + string.Format("{0:0.00}%", ram);
            chart2.Series["RAM"].Points.AddY(cpu);

            label4.Text = "System Up Time " + perfCounter.NextValue() + " Seconds " 
                +perfCounter.NextValue()/60 + " Minutes" + perfCounter.NextValue() / 60/60 + " Hours";
        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }
    }
}
