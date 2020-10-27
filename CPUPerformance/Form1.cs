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
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;
using System.Threading;

namespace CPUPerformance
{
    public partial class Form1 : Form
    {
        PerformanceCounter perfCounter = new PerformanceCounter("System", "System Up Time");
        OpenHardwareMonitor.Hardware.Computer computerHardware = new OpenHardwareMonitor.Hardware.Computer();
        ComputerDiagnostic ComputerData = new ComputerDiagnostic();
        bool backgroundFinished = false;
        string dateVariable;
        int kelvinTemp;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text = "System Up Time";
            computerHardware.MainboardEnabled = true;
            computerHardware.FanControllerEnabled = true;
            computerHardware.CPUEnabled = true;
            computerHardware.GPUEnabled = true;
            computerHardware.RAMEnabled = true;
            computerHardware.HDDEnabled = true;
            computerHardware.Open();
            label7.Text = "Date: " + DateTime.Now.ToString();
            timer1.Start();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float cpu = CPU.NextValue();
            progressBar1.Value = (int)cpu;

            label2.Text = "CPU  " + string.Format("{0:0.00}%", cpu);
            chart1.Series["CPU"].Points.AddY(cpu);

            float ram = RAM1.NextValue();
            progressBar2.Value = (int)ram;

            label3.Text = "RAM  " + string.Format("{0:0.00}%", ram);
            chart2.Series["RAM"].Points.AddY(cpu);

            label4.Text = "System Up Time " + perfCounter.NextValue() + " Seconds "
                + perfCounter.NextValue() / 60 + " Minutes " + perfCounter.NextValue() / 60 / 60 + " Hours";

           
            
            computerHardware.Hardware[4].Update();
            computerHardware.Hardware[0].Update();
            computerHardware.Hardware[1].Update();
            string value = computerHardware.Hardware[1].Sensors[11].Value.ToString();
            label5.Text = value + " °C";
            backgroundWorker1.RunWorkerAsync();
            timer1.Stop();
            //computerHardware.Close();

            Thread tempKelvin = new Thread(new ThreadStart(() =>
            {
                while (!backgroundFinished)
                {
                    kelvinTemp = (int)(ComputerData.Temp + 273.15);
                    Thread.Sleep(1000);
                }
            }));
            tempKelvin.Start();

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //thread with lambda experssion
            Thread DateThread = new Thread(new ThreadStart(() =>
            {
                while (!backgroundFinished)
                {
                    dateVariable = DateTime.Now.ToString();
                    Thread.Sleep(1000);
                }
            }));
            DateThread.Start();

            while (!backgroundFinished) { 
                ComputerData.UpdatePerformanceData(CPU,RAM1,computerHardware,perfCounter);
                backgroundWorker1.ReportProgress((int)20);
                Thread.Sleep(1000);
            }
            DateThread.Join();
            
            // Join obliges to wait from one thread to finish.

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label2.Text = "CPU  " + string.Format("{0:0.00}%", ComputerData.CPUPercentage);
            label3.Text = "RAM  " + string.Format("{0:0.00}%", ComputerData.RAMUsed);
            chart1.Series["CPU"].Points.AddY(ComputerData.CPUPercentage);
            chart1.Series["Temp"].Points.AddY(ComputerData.Temp);
            chart2.Series["RAM"].Points.AddY(ComputerData.RAMUsed);
            chart2.Series["Temp"].Points.AddY(kelvinTemp);
            progressBar1.Value = (int)ComputerData.CPUPercentage;
            progressBar2.Value = (int)ComputerData.RAMUsed;
            label5.Text = ComputerData.Temp + " °C";
            label4.Text = "System Up Time " + ComputerData.SystemUpSec + " Seconds "
                + ComputerData.SystemUpMin + " Minutes " + ComputerData.SystemUpHour + " Hours";
            label7.Text = "Date: " + dateVariable;

        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundFinished = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundFinished = true;
            DialogResult comment = MessageBox.Show("Do you want to close the program", "Exit", MessageBoxButtons.YesNo);
            if (comment == DialogResult.Yes)
            {
                computerHardware.Close();

            }
            else if (comment == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        
    }
}
