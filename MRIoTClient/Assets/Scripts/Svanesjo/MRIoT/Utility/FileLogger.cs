#nullable enable

using System;
using System.IO;
using System.Text;

namespace Svanesjo.MRIoT.Utility
{
    public class FileLogger : ILogger
    {
        private readonly Type _type;
        private readonly string _directory;
        private readonly string _filePath;
        private Stream? _stream;
        private StreamWriter? _writer;
        private const string Extension = ".csv";

        public FileLogger(Type type, string directory)
        {
            _type = type;
            _directory = directory;
            _filePath = Path.Combine(directory, type.Name + Extension);
        }

        public FileLogger CreateSibling(Type type)
        {
            return new FileLogger(type, _directory);
        }

        private void EnsureOpenWriter()
        {
            // If necessary, create directory
            if (!Directory.Exists(_directory))
                Directory.CreateDirectory(_directory);

            if (_writer != null) return;
            _stream ??= new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Write);
            _writer = new StreamWriter(_stream, Encoding.UTF8);
        }

        private void LogStr(string message)
        {
            EnsureOpenWriter();
            if (_writer == null)
                throw new Exception("FileLogger: _writer is null");

            _writer.WriteLineAsync($"{DateTime.Now}; {message}");
        }

        public void Log(string message)
        {
            LogStr($"DEBUG; {_type.Name}; {message}");
        }

        public void LogWarning(string message)
        {
            LogStr($"WARNING; {_type.Name}; {message}");
        }

        public void LogError(string message)
        {
            LogStr($"ERROR; {_type.Name}; {message}");
        }

        public void Flush()
        {
            _writer?.FlushAsync();
        }

        public void OnDestroy()
        {
            _writer?.Close();
            _stream?.Close();
        }

        public static string NextAvailableDirectory(string parentDirectory, string defaultDirectoryName)
        {
            var dirName = defaultDirectoryName;
            var dirPath = Path.Combine(parentDirectory, dirName);
            while (Directory.Exists(dirPath) || File.Exists(dirPath))
            {
                var start = dirName.IndexOf('0');
                var end = dirName.IndexOf('.');
                if (end == -1) end = dirName.Length;
                if (start < 0 || end > dirName.Length || start >= end)
                    throw new IndexOutOfRangeException();
                var intString = dirName.Substring(start, end - start);

                var parsed = int.TryParse(intString, out var val);
                if (!parsed)
                    throw new FormatException(
                        "Could not parse directory name, should be '[A-Za-z]*[0-9]+[\\.[A-Za-z0-9]+]{0,1}'");

                var newString = $"{(val + 1).ToString($"D{intString.Length}")}";
                if (newString.Length > intString.Length)
                    throw new IndexOutOfRangeException("Incrementing would change the length of the directory name!");

                dirName = $"{dirName[..start]}{newString}{dirName[end..]}";
                dirPath = Path.Combine(parentDirectory, dirName);
            }

            return dirPath;
        }
    }
}
