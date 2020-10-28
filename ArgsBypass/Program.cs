using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArgsBypass
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];

            if (args.Length == 0)
                return;

            File.AppendAllText(logFilePath, $"ArgsBypass was called with args:" + Environment.NewLine);
            File.AppendAllText(logFilePath, string.Join(" ", args) + Environment.NewLine);

            string[] bypassedArgs = args.Select(arg => arg.Replace("^\\", "/")).ToArray();

            File.AppendAllText(logFilePath, $"Replacing slashes:" + Environment.NewLine);
            File.AppendAllText(logFilePath, string.Join(" ", bypassedArgs) + Environment.NewLine);

            try
            {
                string proicessFilePath = args[0];
                string processArgs = string.Join(" ", bypassedArgs.Skip(1));

                File.AppendAllText(logFilePath, $"Starting {proicessFilePath} with args {processArgs}..." + Environment.NewLine);

                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = proicessFilePath;
                process.StartInfo.Arguments = processArgs;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                File.AppendAllText(logFilePath, $"{proicessFilePath} output:" + Environment.NewLine);
                File.AppendAllText(logFilePath, output + Environment.NewLine);
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFilePath, ex.Message + Environment.NewLine);
            }
            finally
            {
                File.AppendAllText(logFilePath, Environment.NewLine);
            }
        }
    }
}
