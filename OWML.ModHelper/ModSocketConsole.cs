using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModSocketConsole : IModConsole
    {
        [Obsolete("Use ModHelper.Console instead")]
        public static ModSocketConsole Instance { get; private set; }

        public static event Action<IModManifest, string> OnConsole;

        private static Socket _socket;

        private readonly IModLogger _logger;
        private readonly IModManifest _manifest;

        public ModSocketConsole(IOwmlConfig config, IModLogger logger, IModManifest manifest)
        {
            if (manifest.Name == "OWML")
            {
                Instance = this;
            }
            _logger = logger;
            _manifest = manifest;
            if (_socket == null)
            {
                int port;
                try
                {
                    port = int.Parse(GetArgument("owmmPort"));
                }
                catch
                {
                    _logger?.Log("Error: Missing mod manager console port argument");
                    return;
                }

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var localHost = "127.0.0.1";
                IPAddress ipAdd = IPAddress.Parse(localHost);
                IPEndPoint remoteEP = new IPEndPoint(ipAdd, port);
                _socket.Connect(remoteEP);

                ModConsole.OnConsole += OnConsole;
            }
        }

        private string GetArgument(string name)
        {
            var arguments = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < arguments.Length; i++)
            {
                var argument = arguments[i];
                if (argument == $"-{name}" && arguments.Length > i)
                {
                    return arguments[i + 1];
                }
            }
            return null;
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
            _socket.Send(bytes);
        }

    }
}
