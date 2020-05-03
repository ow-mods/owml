using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OWML.Common;

namespace OWML.ModHelper.Logging
{
    public class ModSocketConsole : IModConsole
    {
        private const string LocalHost = "127.0.0.1";

        public static event Action<IModManifest, string> OnConsole;

        private static Socket _socket;

        private readonly IModLogger _logger;
        private readonly IModManifest _manifest;

        public ModSocketConsole(IModLogger logger, IModManifest manifest)
        {
            _logger = logger;
            _manifest = manifest;
            if (_socket == null)
            {
                int port;
                try
                {
                    port = int.Parse(CommandLineArguments.GetArgument(Constants.ConsolePortArgument));
                }
                catch
                {
                    _logger?.Log("Error: Missing or incorrectly formatted console port argument");
                    return;
                }

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(LocalHost);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
                _socket.Connect(endPoint);

                // TODO: ModConsole.OnConsole += OnConsole;
            }
        }

        public void WriteLine(string s)
        {
            _logger?.Log(s);
            OnConsole?.Invoke(_manifest, s);
            var message = $"{_manifest.Name};;{s}";
            InternalWriteLine(message);
        }

        public void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private static void InternalWriteLine(string message)
        {

            var bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _socket?.Send(bytes);
        }

    }
}
