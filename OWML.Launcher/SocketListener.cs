using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OWML.Launcher
{
    public class SocketListener
    {
        private const int BufferSize = 1024;
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

            var bytes = new byte[BufferSize];

            while (true)
            {
                var client = server.AcceptTcpClient();

                ConsoleUtils.WriteByType(MessageType.Success, "Console connected to socket!");

                var stream = client.GetStream();

                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    ProcessMessage(bytes, i);   
                }

                client.Close();
            }
        }

        private void ProcessMessage(byte[] bytes, int count)
        {
            var json = Encoding.UTF8.GetString(bytes, 0, count);

            var data = JsonConvert.DeserializeObject<SocketMessage>(json);

            if (data.Type == MessageType.Quit)
            {
                Environment.Exit(0);
            }

            ConsoleUtils.WriteByType(data.Type,
                $"[{data.SenderName}.{data.SenderType}] : {data.Message}");
        }
    }
}
