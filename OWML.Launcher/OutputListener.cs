using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OWML.Common;

namespace OWML.Launcher
{
    public class OutputListener
    {
        private const int ReadLength = 128;

        public Action<string> OnOutput;

        private readonly IModConfig _config;
        private FileStream _reader;

        public OutputListener(IModConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            File.WriteAllText(_config.OutputFilePath, "");
            _reader = File.Open(_config.OutputFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Task.Run(Work);
        }

        private async Task Work()
        {
            while (true)
            {
                var bytes = new byte[ReadLength];
                _reader.Read(bytes, 0, ReadLength);
                var s = Encoding.UTF8.GetString(bytes)
                    .Replace("\0", "");
                HandleLine(s);
                await Task.Delay(100);
            }
        }

        private void HandleLine(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            var newLineIndex = s.LastIndexOf(Environment.NewLine);
            if (newLineIndex == -1)
            {
                _reader.Seek(-ReadLength, SeekOrigin.Current);
                return;
            }
            if (newLineIndex == s.Length - Environment.NewLine.Length)
            {
                OnOutput?.Invoke(s);
                return;
            }
            var seekBack = -ReadLength + newLineIndex + Environment.NewLine.Length;
            _reader.Seek(seekBack, SeekOrigin.Current);
            var untilNewLine = s.Substring(0, newLineIndex + Environment.NewLine.Length);
            OnOutput?.Invoke(untilNewLine);
        }

    }
}