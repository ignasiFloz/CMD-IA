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
    static string greeting = "Welcome! You can ask any questions to the AI related with commands. If you want to leave just type 'exit'.";
    static string bye = "Remember to just type dotnet run inside the right folder if you want to run the ia again. Goodbye!";

    static async Task<int> Main(string[] args)
    {
        EnvManager envManager = new EnvManager();
        OpenAiManager openAiManager = new OpenAiManager();
        CommandHelper commandHelper = new CommandHelper();

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

            var response = await openAiManager.AskOpenAi(question, apiKey);

            
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("There is nothing to show here.");
            }
            else
            {
                Console.WriteLine($"AI response: {response}");
            }

            
            Console.WriteLine("Do you want to run, edit, or exit this command? (run/edit/exit)");
            string executeCommand = Console.ReadLine();

            if (executeCommand?.ToLower() == "run")
            {
                commandHelper.ExecuteCommand(response);
            }
            else if (executeCommand?.ToLower() == "edit")
            {
                commandHelper.EditCommand(ref response); 
                Console.WriteLine($"Edited command: {response}");

                
                Console.WriteLine("Do you want to run the edited command? (yes/no)");
                string runEditedCommand = Console.ReadLine();

                if (runEditedCommand?.ToLower() == "yes")
                {
                    commandHelper.ExecuteCommand(response);
                }
            }
            else if (executeCommand?.ToLower() == "exit")
            {
                Console.WriteLine(bye);
                break;
            }
        }

        return 0;
    }
}
