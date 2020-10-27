using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace TCPIPConnection {
    class Program {
        static void ClientSide(string ip_host, int port_num) {
            TcpClient client = new TcpClient(ip_host, port_num);
            Console.Write("Digite a mensagem que você deseja enviar: ");
            string message = Console.ReadLine();
            
            // armazenando o tamanho da mensagem e enviando o texto ao vetor "send"
            int byteCount = Encoding.ASCII.GetByteCount(message + 1);
            byte[] send = new byte[byteCount];
            send = Encoding.ASCII.GetBytes(message);

            // enviando o vetor "send"
            NetworkStream stream = client.GetStream();
            stream.Write(send, 0, send.Length);

            // recebendo uma mensagem de volta (sw.flush)
            StreamReader sr = new StreamReader(stream);
            string response = sr.ReadLine();
            Console.WriteLine(response);

            byte[] buffer = new byte[1024];
            Console.WriteLine("Aguarde a resposta do servidor...");
            stream.Read(buffer, 0, buffer.Length);
            int recv = 0;

            foreach (byte b in buffer) {
                if (b != 0) {
                    recv++;
                }
            }

            // recebendo uma mensagem do servidor
            StreamWriter sw = new StreamWriter(client.GetStream());
            string request = Encoding.UTF8.GetString(buffer, 0, recv);
            Console.WriteLine("Mensagem recebida: " + request);

            sw.WriteLine("Request received.");
            sw.Flush();

            stream.Close();
            client.Close();
        }
        static void ServerSide(int port_num) {
            // "System.Net.IPAddress.Any" perrmite fazer uma conexão local (LAN) por qualquer IP
            TcpListener server = new TcpListener(System.Net.IPAddress.Any, port_num);
            server.Start();

            Console.WriteLine("Esperando uma conexão na porta " + port_num + "...");
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client accepted.");


            NetworkStream stream = client.GetStream();
            StreamReader sr = new StreamReader(client.GetStream());
            StreamWriter sw = new StreamWriter(client.GetStream());

            byte[] buffer = new byte[1024];
            stream.Read(buffer, 0, buffer.Length);
            int recv = 0;

            foreach (byte b in buffer) {
                if (b != 0) {
                    recv++;
                }
            }

            // recebendp e convertendo o texto recebido do stream - client 
            string request = Encoding.UTF8.GetString(buffer, 0, recv);
            Console.WriteLine("Mensagem recebida: " + request);

            // enviando uma mensagem de volta
            sw.WriteLine("Request received.");
            sw.Flush();

            // enviando uma mensagem ao client
            Console.Write("Digite a mensagem que você deseja enviar: ");
            string message = Console.ReadLine();

            int byteCount = Encoding.ASCII.GetByteCount(message + 1);
            byte[] send = new byte[byteCount];
            send = Encoding.ASCII.GetBytes(message);
            stream.Write(send, 0, send.Length);

            string response = sr.ReadLine();
            Console.WriteLine(response);
        }

        static void Main(string[] args) {
            Console.Write("Deseja ser o host [S/N]? ");
            string host = Console.ReadLine();

            try {
                Console.Write("Digite o número da porta/protocolo: ");
                int port_num = int.Parse(Console.ReadLine());

                if (host.ToUpper() == "SIM" || host.ToUpper() == "S")
                    ServerSide(port_num);
                else {
                    Console.Write("Digite o IP do host: ");
                    string ip_host = Console.ReadLine();

                    ClientSide(ip_host, port_num);
                }
            } catch(Exception e) {
                Console.WriteLine("Falha na conexão...");
            }

            Console.Write("Deseja continuar [S/N]? ");
            string mensagem = Console.ReadLine();
            mensagem.ToLower();
            if (mensagem == "sim" || mensagem == "s")
                Main(null);
        }
    }
}
