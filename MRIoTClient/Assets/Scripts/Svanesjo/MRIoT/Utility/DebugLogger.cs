#nullable enable

using System;
using UnityEngine;

namespace Svanesjo.MRIoT.Utility
{
    public class DebugLogger : ILogger
    {
        private Type _type;

        public DebugLogger(Type type)
        {
            _type = type;
        }

        public void Log(string message)
        {
            Debug.Log($"[{_type.Name}] {message}");
        }

        public void LogWarning(string message)
        {
            Debug.LogError($"[{_type.Name}] {message}");
        }

        public void LogError(string message)
        {
            Debug.LogError($"[{_type.Name}] {message}");
        }

        public void Flush()
        {
        }

        public void OnDestroy()
        {
        }
    }
}
