using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
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
                ConnectToSocket();
            }
        }

        [Obsolete("Use ModSocketOutput.Writeline(MessageType type, string s) instead")]
        public override void WriteLine(string s)
        {
            MessageType type;
            if (s.ToLower().Contains("error") || s.ToLower().Contains("exception"))
            {
                type = MessageType.Error;
            }
            else if (s.ToLower().Contains("warning") || s.ToLower().Contains("disabled"))
            {
                type = MessageType.Warning;
            }
            else if (s.ToLower().Contains("success"))
            {
                type = MessageType.Success;
            }
            else
            {
                type = MessageType.Message;
            }
            WriteLine(type, s);
        }

        [Obsolete("Use ModSocketOutput.Writeline(MessageType type, params object[] objects) instead")]
        public override void WriteLine(params object[] objects)
        {
            WriteLine(string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        public override void WriteLine(string sender, MessageType type, string data)
        {
            Logger?.Log(data);
            CallWriteCallback(Manifest, data);

            var message = new SocketMessage
            {
                Sender = sender,
                Type = type,
                Message = data
            };
            var json = JsonConvert.SerializeObject(message);

            WriteToSocket(json);
        }

        public override void WriteLine(MessageType type, string data)
        {
            Logger?.Log(data);
            CallWriteCallback(Manifest, data);

            var message = new SocketMessage
            {
                Sender = Manifest.Name,
                Type = type,
                Message = data
            };
            var json = JsonConvert.SerializeObject(message);

            WriteToSocket(json);
        }

        public override void WriteLine(string sender, MessageType type, params object[] objects)
        {
            WriteLine(sender, type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        public override void WriteLine(MessageType type, params object[] objects)
        {
            WriteLine(type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private void ConnectToSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(LocalHost);
            var endPoint = new IPEndPoint(ipAddress, _port);
            _socket.Connect(endPoint);
        }

        private void WriteToSocket(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            _socket?.Send(bytes);
        }
    }
}