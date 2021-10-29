﻿using System;
using System.Diagnostics;
using OWML.Common;

namespace OWML.Logging
{
	public class ModSocketOutput : ModConsole
	{
		private readonly IModSocket _socket;
		private readonly IProcessHelper _processHelper;

		public ModSocketOutput(IOwmlConfig config, IModManifest manifest, IModSocket socket, IProcessHelper processHelper)
			: base(config, manifest)
		{
			_socket = socket;
			_processHelper = processHelper;
		}

		public override void WriteLine(string line) =>
			WriteLine(line, MessageType.Message, GetCallingType(new StackTrace()));

		public override void WriteLine(string line, MessageType type) =>
			WriteLine(line, type, GetCallingType(new StackTrace()));

		public override void WriteLine(string line, MessageType type, string senderType)
		{
			if (type != MessageType.Debug || OwmlConfig.DebugMode)
			{
				_socket.WriteToSocket(new ModSocketMessage
				{
					SenderName = Manifest.Name,
					SenderType = senderType,
					Type = type,
					Message = line
				});
			}

			if (type == MessageType.Fatal)
			{
				KillProcess();
			}
		}
		private void WriteLine(MessageType type, string line, string senderType) =>
			WriteLine(line, type, senderType);

		private void KillProcess()
		{
			_socket.Close();
			_processHelper.KillCurrentProcess();
		}

		private string GetCallingType(StackTrace frame)
		{
			try
			{
				return frame.GetFrame(1).GetMethod().DeclaringType.Name;
			}
			catch (Exception ex)
			{
				var message = new ModSocketMessage
				{
					SenderName = Constants.OwmlTitle,
					SenderType = nameof(ModSocketOutput),
					Type = MessageType.Error,
					Message = $"Error while getting calling type : {ex.Message}"
				};
				_socket.WriteToSocket(message);
				return string.Empty;
			}
		}
	}
}
