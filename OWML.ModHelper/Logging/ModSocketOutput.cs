using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OWML.Common;

namespace OWML.ModHelper.Logging
{
    public class ModSocketOutput : ModConsole
    {
        private const string LocalHost = "127.0.0.1";
        private const string NameMessageSeparator = ";;";

        private int _port;
        private static Socket _socket;

        public ModSocketOutput(IOwmlConfig config, IModLogger logger, IModManifest manifest) : base(config, logger, manifest)
        {
            if (_socket == null)
            {
                _port = config.SocketPort;
                config.SocketPort = -1;
                JsonHelper.SaveJsonObject(Constants.OwmlConfigFileName, config);
                CreateSocket();
            }
        }

        public override void WriteLine(string s)
        {
            Logger?.Log(s); Console.WriteLine(s);
            CallWriteCallback(Manifest, s);
            var message = $"{Manifest.Name}{NameMessageSeparator}{s}";
            InternalWriteLine(message);
        }

        public override void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private void CreateSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(LocalHost);
            var endPoint = new IPEndPoint(ipAddress, _port);
            _socket.Connect(endPoint);
        }

        private void InternalWriteLine(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            _socket?.Send(bytes);
        }
    }
}