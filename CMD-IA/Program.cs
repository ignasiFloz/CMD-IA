using System.CommandLine;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.IO; 
using CMD_IA.Models;
using System.Diagnostics;
using CMD_IA.Helpers;

namespace scl;

class Program
{
    static string apiKey = "";
    static string greeting = "Welcome! You can ask any questions to the AI related with commands. If you want to leave just type 'exit'.";
    static string bye = "Remember to just type dotnet run inside the right folder if you want to run the ia again. Goodbye!";
    static async Task<int> Main(string[] args)
    {
        EnvManager envManager = new EnvManager();


        await envManager.ValidateEnvFile(); 
        string apiKey = envManager.ApiKey; 
        Console.WriteLine(greeting);


        while (true)
        {
            Console.WriteLine("\nPlease enter your question:");
            string question = Console.ReadLine();

            
            if (question?.ToLower() == "exit")
            {
                Console.WriteLine(bye);
                break;
            }

            OpenAiManager openAiManager = new OpenAiManager();
            var response = await openAiManager.AskOpenAi(question, apiKey);
            Console.WriteLine($"AI response: {response}");

            
            Console.WriteLine("Do you want to run this command? (yes/no)");
            string executeCommand = Console.ReadLine();

            if (executeCommand?.ToLower() == "yes")
            {
                ExecuteCommand(response);
            }
        }

        return 0; 
    }

    //FUNCTIONS

   

    static void ExecuteCommand(string command)
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
