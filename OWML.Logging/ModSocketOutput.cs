using System;
using System.Diagnostics;
using System.Linq;
using OWML.Common;

namespace OWML.Logging
{
    public class ModSocketOutput : ModConsole
    {
        private readonly IModSocket _socket;

        public ModSocketOutput(IOwmlConfig config, IModLogger logger, IModManifest manifest, IModSocket socket)
            : base(config, logger, manifest)
        {
            _socket = socket;
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
            Logger?.Log(line);

            var message = new ModSocketMessage
            {
                SenderName = Manifest.Name,
                SenderType = senderType,
                Type = type,
                Message = line
            };
            _socket.WriteToSocket(message);
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
                return "";
            }
        }

    }
}
