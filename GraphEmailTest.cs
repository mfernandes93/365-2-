using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace GraphEmailTest
{
    class Program
    {
        // Substitua pelos valores do seu registro de aplicativo no Azure AD
        private const string clientId = "YOUR_CLIENT_ID";
        private const string tenantId = "YOUR_TENANT_ID";
        private const string clientSecret = "YOUR_CLIENT_SECRET";

        // Para client credentials, o escopo é https://graph.microsoft.com/.default, que usa as permissões configuradas no portal
        private static string[] scopes = new[] { "https://graph.microsoft.com/.default" };

        static async Task Main(string[] args)
        {
            // Cria o aplicativo confidencial (client credentials flow)
            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithTenantId(tenantId)
                .Build();

            // Adquire o token de acesso
            AuthenticationResult authResult = await confidentialClient.AcquireTokenForClient(scopes).ExecuteAsync();

            // Configura o GraphServiceClient com o token obtido
            GraphServiceClient graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                (requestMessage) =>
                {
                    requestMessage.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                    return Task.CompletedTask;
                }));

            // Informe o e-mail (ou ID) do usuário que deseja consultar
            string userEmail = "user@example.com";

            try
            {
                // Consulta os 5 e-mails mais recentes da caixa de entrada do usuário especificado
                var messages = await graphClient.Users[userEmail].Messages.Request().Top(5).GetAsync();

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
