using CliWrap;
using System.Text;
using System.Text.RegularExpressions;

namespace ScoopDesktop.Utils
{
    public class PwshHelper
    {
        public static async Task<string> RunCommandAsync(string command, string arguments)
        {
            var sb = new StringBuilder();
            await Cli
                .Wrap(command)
                .WithArguments(arguments)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                    sb.AppendLine(Regex.Replace(line, @"\x1B\[(?:\d+;)?\dm", ""))))
                .ExecuteAsync();
            return sb.ToString().TrimEnd();
        }

        public static async Task<string> RunCommandAsync(string command)
        {
            var split = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            return await RunCommandAsync(split[0], split[1]);
        }

        public static async Task RunCommandAsync(string command, string arguments, Action<string> callback)
        {
            await Cli
                .Wrap(command)
                .WithArguments(arguments)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line =>
                {
                    line = Regex.Replace(line, @"\x1B\[(?:\d+;)?\dm", "");
                    callback(line);
                }))
                .ExecuteAsync();
        }

        public static async Task RunCommandAsync(string command, Action<string> callback)
        {
            var split = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            await RunCommandAsync(split[0], split[1], callback);
        }
    }
}
