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
            var dotnetPath = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
            var startInfo = new ProcessStartInfo
            {
                FileName = dotnetPath,
                Arguments = "--info",
                // RedirectStandardInput = true,
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Directory.GetCurrentDirectory(),
            };
            var process = Process.Start(startInfo);

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
