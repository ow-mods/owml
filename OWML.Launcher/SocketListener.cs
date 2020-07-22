using Newtonsoft.Json;
using OWML.Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Launcher
{
    public class SocketListener
    {
        private static int _port;

        public SocketListener(int port)
        {
            _port = port;
            new Task(SetupSocketListener).Start();
        }

        private void SetupSocketListener()
        {
            TcpListener server = null;
            try
            {
                ListenToSocket(ref server);
            }
            catch (SocketException ex)
            {
                ConsoleUtils.WriteByType(MessageType.Error, $"Error in socket listener: {ex}");
            }
            finally
            {
                server?.Stop();
            }
        }

        private void ListenToSocket(ref TcpListener server)
        {
            var localAddress = IPAddress.Parse(Constants.LocalAddress);

            server = new TcpListener(localAddress, _port);
            server.Start();

            var bytes = new byte[1024];

            while (true)
            {
                var client = server.AcceptTcpClient();

                ConsoleUtils.WriteByType(MessageType.Success, "Console connected to socket!");

                var stream = client.GetStream();

                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    PrintOutput(bytes, i);   
                }

                client.Close();
            }
        }

        private void PrintOutput(byte[] bytes, int count)
        {
            var json = Encoding.ASCII.GetString(bytes, 0, count);

            var data = JsonConvert.DeserializeObject<SocketMessage>(json);

            ConsoleUtils.WriteByType(data.Type,
                $"[{data.SenderName}-{data.SenderType}] : {data.Message}");
        }
    }
}
