using System;
using System.Diagnostics;
using System.Linq;
using OWML.Common;
using OWML.Common.Enums;
using OWML.Common.Interfaces;

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
            WriteLine(MessageType.Message, line, GetCallingType(new StackTrace()));
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
            Logger?.Log($"{type}: {line}");

            var message = new ModSocketMessage
            {
                SenderName = Manifest.Name,
                SenderType = senderType,
                Type = type,
                Message = line
            };
            _socket.WriteToSocket(message);

            if (type == MessageType.Fatal)
            {
                KillProcess();
            }
        }

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
