using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Xunit;

namespace App.Tests1
{
    public class UnitTest1
    {
        [Fact]
        public void StartBackgroundProcess()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "BackgroundApp1")))
                {
                    break;
                }

                dir = dir.Parent;
            }
            
            var dotnetPath = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
            var startInfo = new ProcessStartInfo
            {
                FileName = dotnetPath,
                Arguments = "run -p BackgroundApp1/BackgroundApp1.csproj",
                UseShellExecute = false,
                // RedirectStandardInput = true,
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = dir.FullName,
            };
            var process = Process.Start(startInfo);
            
            Console.WriteLine("Started PID = " + process.Id);
            //Thread.Sleep(TimeSpan.FromSeconds(2)); // wait a little for dotnet-run to at least start the background process

            if (process.WaitForExit(60000))
            {
                Console.WriteLine($"Outer process {process.Id} exited cleanly.");
            }
            else
            {
                Console.WriteLine($"Outer process {process.Id} is still running.");
            }
        }
    }
}