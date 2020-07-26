using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using OWML.Common;
using UnityEngine;

namespace OWML.ModHelper
{
    public class ModSocketOutput : ModConsole
    {
        private readonly int _port;
        private static Socket _socket;

        public ModSocketOutput(IOwmlConfig config, IModLogger logger, IModManifest manifest, bool listenToUnity) : base(config, logger, manifest)
        {
            if (_socket == null)
            {
                _port = config.SocketPort;
                ConnectToSocket();
            }

            if (config.Verbose && listenToUnity)
            {
                WriteLine("Verbose mode is enabled", MessageType.Info);
                Application.logMessageReceived += OnLogMessageReceived;
            }
        }

        private void OnLogMessageReceived(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                var socketMessage = new SocketMessage
                {
                    SenderName = "Unity",
                    SenderType = type.ToString(),
                    Type = MessageType.Error,
                    Message = $"Unity log message: {message}. Stack trace: {stackTrace?.Trim()}"
                };
                WriteToSocket(JsonConvert.SerializeObject(socketMessage));
            }
        }

        [Obsolete("Use WriteLine(string) or WriteLine(string, MessageType) instead.")]
        public override void WriteLine(params object[] objects)
        {
            var line = string.Join(" ", objects.Select(o => o.ToString()).ToArray());
            var type = MessageType.Message;
            WriteLine(type, line, GetCallingType(new StackTrace()));
        }

        public override void WriteLine(string line)
        {
            WriteLine(MessageType.Message, line, GetCallingType(new StackTrace()));
        }

        public override void WriteLine(string line, MessageType type)
        {
            WriteLine(type, line, GetCallingType(new StackTrace()));
        }

        private void WriteLine(MessageType type, string line, string senderType)
        {
            Logger?.Log(line);
            CallWriteCallback(Manifest, line);

            var message = new SocketMessage
            {
                SenderName = Manifest.Name,
                SenderType = senderType,
                Type = type,
                Message = line
            };
            var json = JsonConvert.SerializeObject(message);

            WriteToSocket(json);
        }

        private string GetCallingType(StackTrace frame)
        {
            try
            {
                return frame.GetFrame(1).GetMethod().DeclaringType.Name;
            }
            catch (Exception ex)
            {
                var message = new SocketMessage
                {
                    SenderName = "OWML",
                    SenderType = "ModSocketOutput",
                    Type = MessageType.Error,
                    Message = $"Error while getting calling type : {ex.Message}"
                };
                WriteToSocket(JsonConvert.SerializeObject(message));

                return "";
            }
        }

        private void ConnectToSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(Constants.LocalAddress);
            var endPoint = new IPEndPoint(ipAddress, _port);
            _socket.Connect(endPoint);
        }

        private void WriteToSocket(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _socket?.Send(bytes);
        }
    }
}
