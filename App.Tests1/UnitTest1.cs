using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace App.Tests1
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _outputHelper;

        public UnitTest1(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

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
            
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run -p BackgroundApp1/BackgroundApp1.csproj",
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = dir.FullName,
            });
            _outputHelper.WriteLine("Started PID = " + process.Id);
        }
    }
}