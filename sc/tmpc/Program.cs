using System;
using SuperSimpleTcp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Desenvolvedor > Rafael Rodrigo | e-mail: rafael@itprati.ao");
        Console.WriteLine("Iniciando o cliente TCP simples...");
        Console.WriteLine("Pressione ENTER para continuar ou CTRL+C para sair.");

        Console.Title = "Cliente de Chat Simples";
        Console.Write("Informe seu nome ou e-mail: ");
        string? identificacao = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(identificacao))
        {
            Console.WriteLine("Identificação obrigatória. Encerrando aplicação.");
            return;
        }
        Console.WriteLine($"Bem-vindo, {identificacao}! Digite mensagens para enviar ao servidor. Pressione ENTER para sair.");
        using (SimpleTcpClient client = new SimpleTcpClient("127.0.0.1:9000"))
        {
            client.Events.Connected += (sender, e) =>
            {
                Console.WriteLine("Conectado ao servidor.");
            };

            client.Events.Disconnected += (sender, e) =>
            {
                Console.WriteLine("Desconectado do servidor.");
            };

            client.Events.DataReceived += (sender, e) =>
            {
                string msg = System.Text.Encoding.UTF8.GetString(e.Data);
                Console.WriteLine($"Mensagem recebida do servidor: {msg}");
            };

            client.Connect();

            // Envia identificação ao servidor
            client.Send($"[IDENTIFICACAO]{identificacao}");

            string? line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                client.Send(line);
            }
        }
    }
}
