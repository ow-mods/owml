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

        private static Socket _socket;

        public ModSocketOutput(IOwmlConfig config, IModLogger logger, IModManifest manifest) : base(config, logger, manifest)
        {
            if (_socket == null)
            {
                CreateSocket();
            }
        }

        public override void WriteLine(string s)
        {
            _logger.Log(s);
            CallWriteCallback(_manifest, s);
            var message = $"{_manifest.Name};;{s}";
            InternalWriteLine(message);
        }

        public override void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private void CreateSocket()
        {
            var consolePortArgument = CommandLineArguments.GetArgument(Constants.ConsolePortArgument);
            if (!int.TryParse(consolePortArgument, out var port))
            {
                _logger.Log("Error: Missing or incorrectly formatted console port argument");
                return;
            }

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(LocalHost);
            var endPoint = new IPEndPoint(ipAddress, port);
            _socket.Connect(endPoint);
        }

        private void InternalWriteLine(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _socket?.Send(bytes);
        }
    }
}
