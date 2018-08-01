using System.Diagnostics;
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
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cat",
                Arguments = "/dev/random",
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            _outputHelper.WriteLine("PID = " + process.Id);
        }
    }
}