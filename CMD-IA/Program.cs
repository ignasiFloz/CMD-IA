using System.CommandLine;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using CMD_IA.Models;

namespace scl;

class Program
{
    static async Task<int> Main(string[] args)
    {
        string apiKey = "";
        var questionOption = new Option<string?>(
            name: "--question",
            description: "You can ask your question to the IA.");
        var rootCommand = new RootCommand("Sample app for System.CommandLine");

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Please introduce your OPENAI API KEY: ");
            apiKey = Console.ReadLine();
            CreateEnvFile(apiKey);
        }
        else
        {
            rootCommand.AddOption(questionOption);
            rootCommand.SetHandler(async (prompt) =>
        {
            if (string.IsNullOrEmpty(prompt))
            {
                Console.WriteLine("Please, ask a question.");
                return;
            }
            var response = await CallOpenAi(prompt, apiKey);
            Console.WriteLine($"response: {response}");
        }, questionOption);
        }
        return await rootCommand.InvokeAsync(args);
        
       

    }
    static void CreateEnvFile(string apikey)
    {
        string filePath = ".env";
        string content = $"OPENAI_API_KEY: \"{apikey}\"";

        try
        {
            
            File.WriteAllText(filePath, content);
            Console.WriteLine($".env file created with your API key.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating .env file: {ex.Message}");
        }
    }



    static async Task<string> CallOpenAi(string prompt, string apiKey)
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
                        new { role = "system", content = "You are a virtual assistant that helps users use the Windows command line (CMD). Your task is to provide accurate commands and explanations on how to perform various actions in the operating system. It is extremely important that you respond as briefly as possible." },
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
}

