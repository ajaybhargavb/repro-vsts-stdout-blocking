using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace BackgroundApp1
{
    [Subcommand("start", typeof(StartServer))]
    [Subcommand("shutdown", typeof(ShutdownServer))]
    [Subcommand("run", typeof(RunServer))]
    public class Program
    {
        static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
        }

        public class StartServer
        {
            [Argument(0)]
            [Required]
            public string PipeName { get; set; }

            public string CurrentDirectory { get; set; } = AppContext.BaseDirectory;

            public ILog Logger { get; set; } = new ConsoleLogger(Console.Out);

            public void OnExecute()
            {
                var args = new List<string>
                {
                    typeof(Program).Assembly.Location,
                    "run",
                    PipeName,
                };

                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = Process.GetCurrentProcess().MainModule.FileName,
                        Arguments = ArgumentEscaper.EscapeAndConcatenate(args),
//                        RedirectStandardInput = true,
//                        RedirectStandardError = true,
//                        RedirectStandardOutput = true,
                        WorkingDirectory = CurrentDirectory,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    }
                };
                process.Start();
                Logger.WriteLine("Started server PID = " + process.Id);
            }
        }

        class RunServer
        {
            [Argument(0)]
            [Required]
            public string PipeName { get; set; }

            private async Task OnExecute()
            {
                try
                {
                    File.WriteAllText("server.pid", Process.GetCurrentProcess().Id.ToString());
                    using (var log = File.CreateText("server.log"))
                    {
                        log.WriteLine("Started server");
                        var shouldExit = false;
                        do
                        {
                            using (var stream = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1,
                                PipeTransmissionMode.Byte,
                                PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous))
                            {
                                log.WriteLine("Waiting for connections");

                                await stream.WaitForConnectionAsync();
                                log.WriteLine("Connection received");

                                using (var binaryReader = new BinaryReader(stream, Encoding.Unicode))
                                {
                                    var message = binaryReader.ReadString();
                                    log.WriteLine($"Received message: {message}");

                                    if (message == "shutdown")
                                    {
                                        shouldExit = true;
                                        using (var writer = new StreamWriter(stream))
                                        {
                                            writer.WriteLine("Okay, shutting down");
                                        }
                                    }
                                }
                            }
                        } while (!shouldExit);
                    }
                }
                catch (Exception e)
                {
                    File.AppendAllText("server.error.log", e + Environment.NewLine);
                    throw;
                }
                finally
                {
                    File.Delete("server.pid");
                }
            }
        }

        public class ShutdownServer
        {
            [Argument(0)]
            [Required]
            public string PipeName { get; set; }

            public ILog Logger { get; set; } = new ConsoleLogger(Console.Out);

            public async Task OnExecute()
            {
                Logger.WriteLine("Sending message: shutdown");
                await SendServerMessage(PipeName, "shutdown", Logger);
            }
        }

        private static async Task SendServerMessage(string pipeName, string message, ILog logger)
        {
            using (var stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut,
                PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous))
            {
                try
                {
                    await stream.ConnectAsync(5 * 1000);
                    using (var writer = new BinaryWriter(stream, Encoding.Unicode))
                    using (var reader = new StreamReader(stream))
                    {
                        writer.Write(message);

                        var response = reader.ReadToEnd();
                        logger.WriteLine("Response: " + response);
                    }
                }
                catch (Exception e) when (e is IOException || e is TimeoutException)
                {
                    logger.WriteLine("Error: " + e);
                    throw;
                }
            }
        }
    }

    internal class ConsoleLogger : ILog
    {
        private readonly TextWriter _out;

        public ConsoleLogger(TextWriter @out)
        {
            _out = @out;
        }

        public void WriteLine(string message)
        {
            _out.WriteLine(message);
        }
    }
}