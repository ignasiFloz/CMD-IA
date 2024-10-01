
namespace CMD_IA.Helpers
{
    internal class EnvManager
    {

        private string apiKey;

        public string ApiKey
        {
            get => apiKey;
            set => apiKey = value;
        }


        public async Task ValidateEnvFile()
        {
            if (File.Exists(".env"))
            {
                ApiKey = ReadApiKeyFromEnv();
            }
            if (string.IsNullOrEmpty(ApiKey))
            {
                Console.WriteLine("Please introduce your OPENAI API KEY: ");
                ApiKey = Console.ReadLine();
                CreateEnvFile(ApiKey);
            }
        }

        private string ReadApiKeyFromEnv() //hacer que coja la string literal no sseparando comillas 
        {
            try
            {
                string content = File.ReadAllText(".env");
                string[] lines = content.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("OPENAI_API_KEY:"))
                    {
                        return line.Split('"')[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading .env file: {ex.Message}");
            }

            return string.Empty;
        }


        private void CreateEnvFile(string apikey)
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

    }
}
