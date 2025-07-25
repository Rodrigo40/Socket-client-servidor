using System;
using SuperSimpleTcp;

class Program
{
    static void Main(string[] args)
    {
        //Desenvolvedor > Rafael Rodrigo
        //rafael@itprati.ao
        // Exibe mensagens de status para o usuário
        Console.WriteLine("Desenvolvedor > Rafael Rodrigo | e-mail: rafael@itprati.ao");
        Console.WriteLine("Iniciando o servidor TCP simples...");
        Console.WriteLine("Certifique-se de que a porta 9000 está livre e não está sendo usada por outro serviço.");
        Console.WriteLine("Pressione ENTER para continuar ou CTRL+C para sair.");
        Console.WriteLine("Servidor iniciado. Aguardando conexões em 127.0.0.1:9000...");
        SimpleTcpServer server = new SimpleTcpServer("127.0.0.1:9000");
        var clientNames = new Dictionary<string, string>(); // IpPort -> Nome/Email


        server.Events.ClientConnected += (sender, e) =>
        {
            Console.WriteLine($"Cliente conectado: {e.IpPort}. Aguardando identificação...");
        };

        server.Events.ClientDisconnected += (sender, e) =>
        {
            string id = clientNames.ContainsKey(e.IpPort) ? clientNames[e.IpPort] : e.IpPort;
            Console.WriteLine($"Cliente desconectado: {id}");
            clientNames.Remove(e.IpPort);
        };

        server.Events.DataReceived += (sender, e) =>
        {
            string msg = System.Text.Encoding.UTF8.GetString(e.Data);
            if (!clientNames.ContainsKey(e.IpPort))
            {
                // Primeira mensagem do cliente é o nome/email
                clientNames[e.IpPort] = msg.Trim();
                Console.WriteLine($"Identificação recebida de {e.IpPort}: {msg.Trim()}");
            }
            else
            {
                string id = clientNames[e.IpPort];
                Console.WriteLine($"Mensagem de {id}: {msg}");
            }
        };

        server.Start();
        Console.WriteLine("Digite uma mensagem e pressione ENTER para enviar a todos os clientes. Pressione ENTER sem digitar nada para encerrar o servidor.");

        while (true)
        {
            string input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                break;
            }
            foreach (var client in server.GetClients())
            {
                string id = clientNames.ContainsKey(client) ? clientNames[client] : client;
                server.Send(client, input);
                Console.WriteLine($"Mensagem enviada para {id}");
            }
        }



        server.Stop();
    }
}
