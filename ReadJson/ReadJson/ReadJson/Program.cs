using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ReadJson
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var keys = await Task.Run(() => KeysFromFileRead());
            var keys = await Task.Run(() => KeysFromConfiguration());
            //var keys = await KeysFromKeyVault();

            Console.WriteLine(keys.PredictionKey);
            Console.WriteLine(keys.TrainingKey);

            Console.ReadLine();
        }

        public static Keys KeysFromFileRead()
        {
            /* Need the following NuGet packages
                Newtonsoft.Json
            */
            Keys keys = new Keys();

            using (var reader = new StreamReader("keys.json"))
            {
                var json = reader.ReadToEnd();

                keys = JsonConvert.DeserializeObject<Keys>(json);
            }

            return keys;
        }

        public static Keys KeysFromConfiguration()
        {
            /* Need the following NuGet packages 
                Microsoft.Extensions.Configuration
                Microsoft.Extensions.Configuration.Json
                Microsoft.Extensions.Configuration.Binder
            */
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("keys.json");

            var configuration = builder.Build();

            var keys = new Keys();
            configuration.Bind(keys);

            return keys;
        }

        private async static Task<Keys> KeysFromKeyVault()
        {
            /* Need the following NuGet packages 
                Microsoft.Azure.KeyVault
                Microsoft.Azure.Services.AppAuthentication
            */
            const string VAULT_URL = "YOUR KEY VAULT URL";
            var tokenProvider = new AzureServiceTokenProvider();

            var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback));

            var trainingKey = await client.GetSecretAsync(VAULT_URL, "trainingKey");
            var predictionKey = await client.GetSecretAsync(VAULT_URL, "predictionKey");

            return new Keys { PredictionKey = predictionKey.Value, TrainingKey = trainingKey.Value };
        }
    }
}
