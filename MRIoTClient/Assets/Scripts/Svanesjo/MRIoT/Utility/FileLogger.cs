#nullable enable

using System;
using System.IO;
using System.Text;

namespace Svanesjo.MRIoT.Utility
{
    public class FileLogger : ILogger
    {
        private readonly Type _type;
        private readonly string _filePath;
        private Stream? _stream;
        private StreamWriter? _writer;

        public FileLogger(Type type, string filePath)
        {
            _type = type;
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public FileLogger CreateSubLogger(Type type)
        {
            return new FileLogger(type, Path.Combine(_filePath, type.Name + ".log"));
        }

        private void EnsureOpenWriter()
        {
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
            LogStr($"DEBUG; {_type.Name}; {DateTime.Now}; {message}");
        }

        public void LogWarning(string message)
        {
            LogStr($"WARNING; {_type.Name}; {DateTime.Now}; {message}");
        }

        public void LogError(string message)
        {
            LogStr($"ERROR; {_type.Name}; {DateTime.Now}; {message}");
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

        public static string NextAvailableFilePath(string directory, string defaultFileName)
        {
            var fileName = defaultFileName;
            var filePath = Path.Combine(directory, fileName);
            while (File.Exists(filePath))
            {
                var start = fileName.IndexOf('0');
                var end = fileName.IndexOf('.');
                if (start < 0 || end > fileName.Length || start >= end)
                    throw new IndexOutOfRangeException();
                var intString = fileName.Substring(start, end - start);

                var parsed = int.TryParse(intString, out var val);
                if (!parsed)
                    throw new FormatException(
                        "Could not parse file name, should be '[A-Za-z]*[0-9]+\\.[A-Za-z0-9]+'");

                var newString = $"{(val + 1).ToString($"D{intString.Length}")}";
                if (newString.Length > intString.Length)
                    throw new IndexOutOfRangeException("Incrementing would extend the file name!");

                fileName = $"{fileName[..start]}{newString}{fileName[end..]}";
                filePath = Path.Combine(directory, fileName);
            }

            return filePath;
        }
    }
}
