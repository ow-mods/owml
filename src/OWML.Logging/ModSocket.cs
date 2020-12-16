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
		private const int CloseWaitSeconds = 1;

		private readonly Socket _socket;
		private readonly IOwmlConfig _config;

		public ModSocket(IOwmlConfig config)
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_config = config;
		}

		public void WriteToSocket(IModSocketMessage message)
		{
			if (!_socket.Connected)
			{
				Connect();
			}

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
			Thread.Sleep(TimeSpan.FromSeconds(CloseWaitSeconds));
			_socket.Close();
		}

		private void Connect()
		{
			var ipAddress = IPAddress.Parse(Constants.LocalAddress);
			var endPoint = new IPEndPoint(ipAddress, _config.SocketPort);
			_socket.Connect(endPoint);
		}
	}
}
