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

                                // Verifica se a resposta não é nula e possui mensagens
                if (messages?.Value != null)
                {
                    Console.WriteLine("Últimos 5 e-mails na caixa de entrada do usuário:");
                    foreach (var msg in messages.Value)
                    {
                        Console.WriteLine($"Assunto: {msg.Subject} - De: {msg.From?.EmailAddress?.Address}");
                    }
                }
                else
                {
                    Console.WriteLine("Nenhuma mensagem foi retornada.");
                }
            }
            catch (ServiceException ex)
            {
            Console.WriteLine($"Erro ao acessar o Microsoft Graph: {ex.Message}");

                // Tenta acessar o status code através da exceção interna, se for HttpRequestException
                if (ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
                {
                    var statusCode = httpEx.StatusCode.Value;
                    if (statusCode == HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Token de autenticação inválido ou expirado.");
                    }
                    else if (statusCode == HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine("Permissões insuficientes para acessar este recurso.");
                    }
                    else
                    {
                        Console.WriteLine($"Status Code: {statusCode}");
                    }
                }
                else
                {
                    Console.WriteLine("Não foi possível determinar o código de status HTTP.");
                }
            }
        }
    }
}
