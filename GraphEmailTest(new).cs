using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;

namespace GraphEmailTest
{
    class Program
    {
        // Substitua pelos valores do seu aplicativo registrado
        private const string clientId = "YOUR_CLIENT_ID";
        private const string tenantId = "YOUR_TENANT_ID";
        private const string clientSecret = "YOUR_CLIENT_SECRET";

        // O escopo padrão para client credentials é "https://graph.microsoft.com/.default"
        private static string[] scopes = new[] { "https://graph.microsoft.com/.default" };

        static async Task Main(string[] args)
        {
            // Cria o credential utilizando o client secret
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            // Instancia o GraphServiceClient utilizando o credential e os escopos
            var graphClient = new GraphServiceClient(credential, scopes);

            // Informe o e-mail (ou ID) do usuário que deseja consultar
            string userEmail = "user@example.com";

            try
            {
                // Consulta os 5 e-mails mais recentes da caixa de entrada do usuário especificado
                var messages = await graphClient.Users[userEmail].Messages.GetAsync(config =>
                {
                     config.QueryParameters.Top = 5;
                });

                Console.WriteLine("Últimos 5 e-mails na caixa de entrada do usuário:");
                foreach (var msg in messages)
                {
                    Console.WriteLine($"Assunto: {msg.Subject} - De: {msg.From?.EmailAddress?.Address}");
                }
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"Erro ao acessar o Microsoft Graph: {ex.Message}");
            }
        }
    }
}




dotnet add package Microsoft.Graph
dotnet add package Azure.Identity
