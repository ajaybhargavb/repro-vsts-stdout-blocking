using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace BackgroundApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine($"Hello, I'm PID {Process.GetCurrentProcess().Id} and I'm going to run in the background for 30 minutes or so");
            //Thread.Sleep(TimeSpan.FromMinutes(30));
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory(),
            });

            if (process.WaitForExit(60000))
            {
                Console.WriteLine($"PID {process.Id} exited cleanly.");
            }
            else
            {
                Console.WriteLine($"PID {process.Id} is still running.");
            }
        }
    }
}
