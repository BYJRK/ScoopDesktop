using System.Diagnostics;

namespace ScoopDesktop.Utils
{
    public class PwshHelper
    {
        private static Process GetPwshProcess(string command)
        {
            return new Process
            {
                StartInfo = new("powershell.exe", command)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
        }

        public static async Task<string> RunCommandAsync(string command)
        {
            using var p = GetPwshProcess(command);
            p.Start();
            var output = await p.StandardOutput.ReadToEndAsync();
            return output.TrimEnd();
        }

        public static async Task RunCommandAsync(string command, DataReceivedEventHandler callback)
        {
            using var process = GetPwshProcess(command);
            process.OutputDataReceived += callback;
            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
        }
    }
}
