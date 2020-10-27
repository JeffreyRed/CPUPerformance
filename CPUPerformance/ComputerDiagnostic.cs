using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Collections;
using OpenHardwareMonitor.Hardware;

namespace CPUPerformance
{
    class ComputerDiagnostic
    {
        float cpuPercentage;
        float ramUsed;
        int temp;
        int systemUpSec;
        int systemUpMin;
        int systemUpHour;
        PerformanceCounter perfCounter = new PerformanceCounter("System", "System Up Time");

        public float CPUPercentage { get => cpuPercentage; set => cpuPercentage = value; }
        public int Temp { get => temp; set => temp = value; }
        public float RAMUsed { get => ramUsed; set => ramUsed = value; }
        public int SystemUpSec { get => systemUpSec; set => systemUpSec = value; }
        public int SystemUpMin { get => systemUpMin; set => systemUpMin = value; }
        public int SystemUpHour { get => systemUpHour; set => systemUpHour = value; }

        public ComputerDiagnostic(){
            CPUPercentage = 0;
            RAMUsed = 0;
            temp = 0;
            SystemUpSec =0;
            SystemUpMin =0;
            SystemUpHour =0;
        }

        public float[] GetSystemUpData(float timeInSec) {
            float[] SystemUpTime = { timeInSec, timeInSec/60, timeInSec/3600 };
            SystemUpSec = (int)timeInSec;
            SystemUpMin = (int)timeInSec /60;
            SystemUpHour = (int)timeInSec /3600;
            return SystemUpTime;
        }

        public void UpdatePerformanceData(PerformanceCounter CPU, PerformanceCounter RAM, Computer computerHardware, PerformanceCounter SystemUp) {
            cpuPercentage = CPU.NextValue();
            ramUsed = RAM.NextValue();
            GetSystemUpData(SystemUp.NextValue());
            computerHardware.Hardware[1].Update();
            temp = (int)computerHardware.Hardware[1].Sensors[11].Value;

        }


    }
}
