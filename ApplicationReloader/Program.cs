using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ApplicationReloader
{
    
    internal class Program
    {
        private const int TimeOut = 30000;

        private static bool isRunning = true;

        //Args: processID, filePath
        public static void Main(string[] args)
        {
            if (args.Length < 2)
                return;
            var processIdString = args[0];
            var filePath  = args[1].Replace("\"\"", " ");
            if (int.TryParse(processIdString, out var processId) && File.Exists(filePath))
            {
                var timer = new Timer { Interval = TimeOut };
                timer.Elapsed += TimerOnElapsed;
                timer.Start();
                var hookProcess = Process.GetProcessById(processId);
                while(!hookProcess.HasExited && isRunning)
                    Thread.Sleep(10);
                if(isRunning)
                    Process.Start(filePath);
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            isRunning = false;
        }
    }
}