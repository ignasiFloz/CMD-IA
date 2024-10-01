using CMD_IA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMD_IA.Helpers
{
    internal class OpenAiManager
    {
        public async Task<string> AskOpenAi(string prompt, string apiKey)
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
                    return $"API call error: {e.Message}";
                }
                catch (JsonException e)
                {
                    return $"Deserialization error: {e.Message}";
                }
            }
        }

    }
}
