using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.ModHelper
{
    public class ModSocket : IModSocket
    {
        private readonly int _port;
        private Socket _socket;

        public ModSocket(int port)
        {
            _port = port;
        }

        public void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(Constants.LocalAddress);
            var endPoint = new IPEndPoint(ipAddress, _port);
            _socket.Connect(endPoint);
        }

        public void WriteToSocket(ISocketMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(json + Environment.NewLine);
            try
            {
                _socket?.Send(bytes);
            }
            catch (SocketException) { }
        }
    }
}
