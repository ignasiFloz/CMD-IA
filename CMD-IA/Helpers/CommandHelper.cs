using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMD_IA.Helpers
{
    internal class CommandHelper
    {
        private string operativeSystem;
        public CommandHelper()
        {
            
            CheckOS(); 
            
        }
        public string OperativeSystem => operativeSystem;

        public void CheckOS()
        {
            try
            {
                ProcessStartInfo startCommand = new ProcessStartInfo("cmd.exe", "/c dir")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (Process process = Process.Start(startCommand))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0) 
                    {
                        operativeSystem = "Windows";
                    }
                    else
                    {
                        operativeSystem = "Linux";
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error checking operating system: {ex.Message}");
                operativeSystem = "Unknown";
            }

            
        }


        public void ExecuteCommand(string command)
        {
            try
            {

                ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                using (Process process = Process.Start(processInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine($"Output: \n{result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command: {ex.Message}");
            }
        }


    }
}
