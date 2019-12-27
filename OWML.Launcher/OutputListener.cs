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
            using (var reader = File.Open(_outputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                while (true)
                {
                    var bytes = new byte[128];
                    reader.Read(bytes, 0, 128);
                    var s = Encoding.UTF8.GetString(bytes).Replace("\0", "");
                    if (!string.IsNullOrEmpty(s))
                    {
                        OnOutput?.Invoke(s);
                    }
                    await Task.Delay(100);
                }
            }
        }
    }
}
