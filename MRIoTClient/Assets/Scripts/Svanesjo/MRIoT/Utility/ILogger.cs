#nullable enable

using System;

namespace Svanesjo.MRIoT.Utility
{
    public interface ILogger
    {
        public void Log(string message);
        public void LogWarning(string message);
        public void LogError(string message);
        public void Flush();
        public void OnDestroy();
    }
}
