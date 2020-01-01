using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OWML.Common;

namespace OWML.Launcher
{
    public class OutputListener
    {
        public Action<string> OnOutput;

        private readonly string _outputPath;

        public OutputListener(IModConfig config)
        {
            _outputPath = config.OutputFilePath;
        }

        public void Start()
        {
            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }
            File.WriteAllText(_outputPath, "");
            Task.Run(Work);
        }

        private async Task Work()
        {
            const int readLength = 128;
            using (var reader = File.Open(_outputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                while (true)
                {
                    var bytes = new byte[readLength];
                    reader.Read(bytes, 0, readLength);
                    var s = Encoding.UTF8.GetString(bytes).Replace("\0", "");
                    if (!string.IsNullOrEmpty(s))
                    {
                        var newLineIndex = s.LastIndexOf(Environment.NewLine);
                        if (newLineIndex == -1)
                        {
                            reader.Seek(-readLength, SeekOrigin.Current);
                        }
                        else if (newLineIndex == s.Length - Environment.NewLine.Length)
                        {
                            OnOutput?.Invoke(s);
                        }
                        else
                        {
                            var seekBack = -readLength + newLineIndex + Environment.NewLine.Length;
                            reader.Seek(seekBack, SeekOrigin.Current);
                            var untilNewLine = s.Substring(0, newLineIndex + Environment.NewLine.Length);
                            OnOutput?.Invoke(untilNewLine);
                        }
                        
                    }
                    await Task.Delay(100);
                }
            }
        }
    }
}
