using System.Diagnostics;

namespace ScoopDesktop.Utils
{
    public class PwshHelper
    {
        public static async Task<string> RunPowerShellCommandAsync(string command)
        {
            return await Task.Run(() =>
            {
                using var p = new Process();
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "powershell.exe";
                p.StartInfo.Arguments = command;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                return p.StandardOutput.ReadToEnd();
            });
        }
        
        public static async Task RunPowerShellCommandAsync(string command, DataReceivedEventHandler callback)
        {
            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = command,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.OutputDataReceived += callback;
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            });
        }
    }
}
