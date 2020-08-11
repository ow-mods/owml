using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using OWML.Common;

namespace OWML.Logging
{
    public class ModSocket : IModSocket
    {
        private readonly Socket _socket;
        private const int _closeWaitSeconds = 1;

        public ModSocket(int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse(Constants.LocalAddress);
            var endPoint = new IPEndPoint(ipAddress, port);
            _socket.Connect(endPoint);
        }

        public void WriteToSocket(IModSocketMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(json + Environment.NewLine);
            try
            {
                _socket?.Send(bytes);
            }
            catch (SocketException) { }
        }

        public void Close()
        {
            Thread.Sleep(TimeSpan.FromSeconds(_closeWaitSeconds));
            _socket.Close();
        }
    }
}
