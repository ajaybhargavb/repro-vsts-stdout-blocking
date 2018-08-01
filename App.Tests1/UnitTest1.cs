using System;
using System.IO;
using System.Threading.Tasks;
using BackgroundApp1;
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
        public async Task BackgroundServerRuns()
        {
            var pidFile = Path.Combine(AppContext.BaseDirectory, "server.pid");
            var logFile = Path.Combine(AppContext.BaseDirectory, "server.log");
            var errorLogFile = Path.Combine(AppContext.BaseDirectory, "server.error.log");
            var logger = new TestLogger(_outputHelper);
            
            try
            {
                var server = new Program.StartServer
                {
                    PipeName = "testpipe",
                    CurrentDirectory = AppContext.BaseDirectory,
                    Logger = logger,
                };
                server.OnExecute();
                await Task.Delay(500);
                Assert.True(File.Exists(pidFile), "server pid should exist");

                var pid = File.ReadAllText(pidFile);
                _outputHelper.WriteLine("Server PID = " + pid);
            }
            finally
            {
                /*var shutdown = new Program.ShutdownServer
                {
                    PipeName = "testpipe",
                    Logger = logger,
                };

                await shutdown.OnExecute();
                await Task.Delay(1000);
                Assert.False(File.Exists(pidFile), "server pid should not exist");*/
                
                if (File.Exists(logFile))
                {
                    _outputHelper.WriteLine("=======Server log=====");
                    foreach (var line in File.ReadAllLines(logFile))
                    {
                        _outputHelper.WriteLine(line);
                    }
                }
                
                if (File.Exists(errorLogFile))
                {
                    _outputHelper.WriteLine("=======Server error  log=====");
                    foreach (var line in File.ReadAllLines(errorLogFile))
                    {
                        _outputHelper.WriteLine(line);
                    }
                }
            }
        }

        private class TestLogger : ILog
        {
            private readonly ITestOutputHelper _outputHelper;

            public TestLogger(ITestOutputHelper outputHelper)
            {
                _outputHelper = outputHelper;
            }

            public void WriteLine(string message)
            {
                _outputHelper.WriteLine(message);
            }
        }
    }
}