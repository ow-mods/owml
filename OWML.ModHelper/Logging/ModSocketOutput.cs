﻿using System;
using System.Collections.Generic;
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
        public override void WriteLine(string line)
        {
            MessageType type;
            if (line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
            {
                type = MessageType.Error;
            }
            else if (line.ToLower().Contains("warning") || line.ToLower().Contains("disabled"))
            {
                type = MessageType.Warning;
            }
            else if (line.ToLower().Contains("success"))
            {
                type = MessageType.Success;
            }
            else
            {
                type = MessageType.Message;
            }
            var senderName = Manifest.Name;
            var senderFile = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().DeclaringType.Name;
            WriteLine(senderName, senderFile, type, line);
        }

        [Obsolete("Use ModSocketOutput.Writeline(MessageType type, params object[] objects) instead")]
        public override void WriteLine(params object[] objects)
        {
            if (CheckForParamsError(objects))
            {
                var line = string.Join(" ", objects.Select(o => o.ToString()).ToArray());
                MessageType type;
                if (line.ToLower().Contains("error") || line.ToLower().Contains("exception"))
                {
                    type = MessageType.Error;
                }
                else if (line.ToLower().Contains("warning") || line.ToLower().Contains("disabled"))
                {
                    type = MessageType.Warning;
                }
                else if (line.ToLower().Contains("success"))
                {
                    type = MessageType.Success;
                }
                else
                {
                    type = MessageType.Message;
                }
                var senderName = Manifest.Name;
                var senderFile = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().DeclaringType.Name;
                WriteLine(senderName, senderFile, type, line);
            }
        }

        public override void WriteLine(MessageType type, string line)
        {
            Logger?.Log(line);
            CallWriteCallback(Manifest, line);

            var message = new SocketMessage
            {
                SenderName = Manifest.Name,
                SenderType = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().DeclaringType.Name,
                Type = type,
                Message = line
            };
            var json = JsonConvert.SerializeObject(message);

            WriteToSocket(json);
        }

        private void WriteLine(string senderName, string senderFile, MessageType type, string line)
        {
            Logger?.Log(line);
            CallWriteCallback(Manifest, line);

            var message = new SocketMessage
            {
                SenderName = senderName,
                SenderType = senderFile,
                Type = type,
                Message = line
            };
            var json = JsonConvert.SerializeObject(message);

            WriteToSocket(json);
        }

        public override void WriteLine(MessageType type, params object[] objects)
        {
            var senderName = Manifest.Name;
            var senderFile = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().DeclaringType.Name;
            WriteLine(senderName, senderFile, type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
        }

        private bool CheckForParamsError(object[] objects)
        {
            if (objects[0].GetType() == typeof(MessageType))
            {
                var type = (MessageType)objects[0];
                var list = new List<object>(objects);
                list.Remove(0);
                objects = list.ToArray();
                WriteLine(type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
                return false;
            }
            else if (objects[0].GetType() == typeof(string) 
                && objects[1].GetType() == typeof(string) 
                && objects[2].GetType() == typeof(MessageType))
            {
                var senderName = (string)objects[0];
                var senderFile = (string)objects[1];
                var type = (MessageType)objects[2];
                var list = new List<object>(objects);
                list.RemoveRange(0, 3);
                objects = list.ToArray();
                WriteLine(senderName, senderFile, type, string.Join(" ", objects.Select(o => o.ToString()).ToArray()));
                return false;
            }
            return true;
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
            var bytes = Encoding.UTF8.GetBytes(message);
            _socket?.Send(bytes);
        }
    }
}