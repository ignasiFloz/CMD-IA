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

            
            var response = await AskOpenAi(question, apiKey);
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

    static async Task<string> AskOpenAi(string prompt, string apiKey)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string apiUrl = "https://api.openai.com/v1/chat/completions";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "Only provide the command as answer." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 100,
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OpenAiResponse>(responseBody);
                return responseObject?.choices[0]?.message?.content ?? "No content returned.";
            }
            catch (HttpRequestException e)
            {
                return $"Error al llamar a la API: {e.Message}";
            }
            catch (JsonException e)
            {
                return $"Error de deserialización: {e.Message}";
            }
        }
    }

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
