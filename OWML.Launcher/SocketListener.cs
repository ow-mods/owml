using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OWML.Logging;

namespace OWML.Launcher
{
    public class SocketListener
    {
        private const string Separator = "\n--------------------------------";
        private const int BufferSize = 262144;
        private static int _port;
        private static TcpListener _server;
        private static IOwmlConfig _config;

        public SocketListener(IOwmlConfig config)
        {
            _config = config;
        }

        public void Init()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            _port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            _config.SocketPort = _port;
            JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, _config);

            new Task(SetupSocketListener).Start();
        }

        private void SetupSocketListener()
        {
            try
            {
                ListenToSocket();
            }
            catch (SocketException ex)
            {
                ConsoleUtils.WriteByType(MessageType.Error, $"Error in socket listener: {ex}");
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteByType(MessageType.Error, $"Error while listening: {ex}");
            }
            finally
            {
                _server?.Stop();
            }
        }

        private void ListenToSocket()
        {
            var localAddress = IPAddress.Parse(Constants.LocalAddress);

            _server = new TcpListener(localAddress, _port);
            _server.Start();

            var bytes = new byte[BufferSize];

            while (true)
            {
                var client = _server.AcceptTcpClient();

                ConsoleUtils.WriteByType(MessageType.Success, "Console connected to socket!");

                var stream = client.GetStream();

                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    ProcessMessage(bytes, i);   
                }

                ConsoleUtils.WriteByType(MessageType.Success, "Closing client!");
                client.Close();
            }
        }

        private void ProcessMessage(byte[] bytes, int count)
        {
            var message = Encoding.UTF8.GetString(bytes, 0, count);
            var jsons = message.Split('\n');
            foreach (var json in jsons)
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    continue;
                }
                SocketMessage data;
                try
                {
                    data = JsonConvert.DeserializeObject<SocketMessage>(json);
                }
                catch (Exception ex)
                {
                    ConsoleUtils.WriteByType(MessageType.Warning, $"Failed to process following message:{Separator}\n{json}{Separator}");
                    ConsoleUtils.WriteByType(MessageType.Warning, $"Reason: {ex.Message}");
                    continue;
                }

                if (data.Type == MessageType.Quit)
                {
                    Environment.Exit(0);
                }
                ConsoleUtils.WriteByType(data.Type,
                    $"[{data.SenderName}.{data.SenderType}] : {data.Message}");
            }
        }
    }
}
