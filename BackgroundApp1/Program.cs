using System;
using System.Diagnostics;
using System.Threading;

namespace BackgroundApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hello, I'm PID {Process.GetCurrentProcess().Id} and I'm going to run in the background for 30 minutes or so");
            Thread.Sleep(TimeSpan.FromMinutes(30));
        }
    }
}
