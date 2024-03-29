﻿using System;
using System.Diagnostics;
using System.Linq;
using OWML.Common;

namespace OWML.Logging
{
	public class ModSocketOutput : ModConsole
	{
		private readonly IModSocket _socket;
		private readonly IProcessHelper _processHelper;

		public ModSocketOutput(IOwmlConfig config, IModManifest manifest, IModLogger logger, IModSocket socket, IProcessHelper processHelper)
			: base(config, logger, manifest)
		{
			_socket = socket;
			_processHelper = processHelper;
		}

		[Obsolete("Use WriteLine(string) or WriteLine(string, MessageType) instead.")]
		public override void WriteLine(params object[] objects)
		{
			var line = string.Join(" ", objects.Select(o => o.ToString()).ToArray());
			WriteLine(line, MessageType.Message, GetCallingType(new StackTrace()));
		}

		public override void WriteLine(string line) =>
			WriteLine(line, MessageType.Message, GetCallingType(new StackTrace()));

		public override void WriteLine(string line, MessageType type) =>
			WriteLine(line, type, GetCallingType(new StackTrace()));

		public override void WriteLine(string line, MessageType type, string senderType)
		{
			Logger?.Log($"{type}: {line}");

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
					Message = $"Error while getting calling type : {ex}"
				};
				_socket.WriteToSocket(message);
				return string.Empty;
			}
		}
	}
}
