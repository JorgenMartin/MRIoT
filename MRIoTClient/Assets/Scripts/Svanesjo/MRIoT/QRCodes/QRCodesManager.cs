#nullable enable

#if UNITY_WSA

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.Utility;
using UnityEngine;

namespace Svanesjo.MRIoT.QRCodes
{
    public static class QRCodeEventArgs
    {
        public static QRCodeEventArgs<T> Create<T>(T data)
        {
            return new QRCodeEventArgs<T>(data);
        }
    }

    [Serializable]
    public class QRCodeEventArgs<T> : EventArgs
    {
        public T Data { get; private set; }

        public QRCodeEventArgs(T data)
        {
            Data = data;
        }
    }

    [RequireComponent(typeof(AudioSource))]
    public class QRCodesManager : Singleton<QRCodesManager>
    {
        private readonly DateTime _initTime = DateTime.Now;
        public bool autoStartQRTracking = true;
        public bool IsTrackerRunning { get; private set; }
        public bool IsSupported { get; private set; }
        public bool runningEvaluation = false;

        public string? FilePath { get; private set; } = null;
        [SerializeField] private string fileName = "QRTracking0001.log";

        public event EventHandler<bool>? QRCodesTrackingStateChanged;
        public event EventHandler<QRCodeEventArgs<QRCode>>? QRCodeAdded;
        public event EventHandler<QRCodeEventArgs<QRCode>>? QRCodeUpdated;
        public event EventHandler<QRCodeEventArgs<QRCode>>? QRCodeRemoved;
        private readonly SortedDictionary<Guid, QRCode> _qrCodesList = new();

        private QRCodeWatcher? _qrTracker;
        private bool _capabilityInitialized = false;
        private QRCodeWatcherAccessStatus _accessStatus;
        private Task<QRCodeWatcherAccessStatus>? _capabilityTask;
        private Stream? _stream;
        private StreamWriter? _writer;
        private bool _firstQRLog = true;

        private AudioSource _audioSource = null!;
        private bool _popped = false;
        private Vector3? _lastCameraPosition = null;
        private Quaternion? _lastCameraRotation = null;

        private void DebugLog(string message)
        {
            Debug.Log(message);
            LogStr($"[DEBUG] {message}");
        }

        private void EnsureOpenWriter()
        {
            if (_writer != null) return;
            if (_stream == null)
            {
                if (FilePath == null)
                    throw new Exception("FilePath is null!");

                _stream = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.Write);
            }
            _writer = new StreamWriter(_stream, Encoding.UTF8);
        }

        private void LogStr(string message)
        {
            if (!runningEvaluation)
                return;

            EnsureOpenWriter();
            if (_writer == null)
                throw new Exception("QRCodesManager: _writer still null");

            _writer.WriteLineAsync($"{DateTime.Now}; {message}");
        }

        private void LogQR(QRCode code, bool pop = true)
        {
            if (!runningEvaluation)
                return;

            // Run Pop-sound on next frame
            _popped = pop;

            var node = SpatialGraphNode.FromStaticNodeId(code.SpatialGraphNodeId);
            if (node == null || !node.TryLocate(FrameTime.OnUpdate, out Pose pose)) return;
            if (_firstQRLog)
            {
                LogStr("Id; SpatialGraphNodeId; Position; Rotation; CameraPosition; CameraRotation; DistanceFromCamera; PositionFromCamera; RotationFromCamera; Version; PhysicalSideLength; RawDataSize; Data; LastDetectedTime");
                _firstQRLog = false;
            }

            string distanceStr = "-", diffPosStr = "-", diffRotStr = "-";
            string camPosStr = "-", camRotStr = "-";
            if (_lastCameraPosition is {} camPos && _lastCameraRotation is {} camRot)
            {
                distanceStr = pose.position.DistanceFrom(camPos).ToString("F7");
                diffPosStr = pose.position.DifferanceFrom(camPos).ToString("F7");
                diffRotStr = pose.rotation.DifferenceFrom(camRot).ToString("F7");
                camPosStr = camPos.ToString("F7");
                camRotStr = camRot.ToString("F7");
            }

            LogStr($"{code.Id}; {code.SpatialGraphNodeId}; {pose.position.ToString("F7")}; {pose.rotation.ToString("F7")}; {camPosStr}; {camRotStr}; {distanceStr}; {diffPosStr}; {diffRotStr}; {code.Version}; {code.PhysicalSideLength}; {code.RawDataSize}; {code.Data}, {code.LastDetectedTime}");
        }

        public Guid GetIdForQRCode(string qrCodeData)
        {
            lock (_qrCodesList)
            {
                foreach (var ite in _qrCodesList)
                {
                    if (ite.Value.Data == qrCodeData)
                    {
                        return ite.Key;
                    }
                }
            }
            return new Guid();
        }

        public IList<QRCode> GetList()
        {
            lock (_qrCodesList)
            {
                return new List<QRCode>(_qrCodesList.Values);
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                throw new MissingComponentException(nameof(_audioSource));
        }

        private async void Start()
        {
            IsSupported = QRCodeWatcher.IsSupported();
            _capabilityTask = QRCodeWatcher.RequestAccessAsync();
            _accessStatus = await _capabilityTask;
            _capabilityInitialized = true;

            fileName = "QRCodeManager0001.log";
            FilePath = Path.Combine(Application.persistentDataPath, fileName);
            while (File.Exists(FilePath))
            {
                var start = fileName.IndexOf('0');
                var end = fileName.IndexOf('.');
                if (start < 0 || end > fileName.Length || start >= end)
                    throw new IndexOutOfRangeException();
                var intString = fileName.Substring(start, end - start);

                var parsed = int.TryParse(intString, out var val);
                if (!parsed)
                    throw new FormatException("Could not parse file name, should be '[A-Za-z]*[0-9]+\\.[A-Za-z0-9]+'");

                var newString = $"{(val+1).ToString($"D{intString.Length}")}";
                if (newString.Length > intString.Length)
                    throw new IndexOutOfRangeException("Incrementing would extend the file name!");

                fileName = $"{fileName[..start]}{newString}{fileName[end..]}";
                FilePath = Path.Combine(Application.persistentDataPath, fileName);
            }
            DebugLog($"Using new log file '{FilePath}'");
            EnsureOpenWriter();
        }

        private void SetupQRTracking()
        {
            try
            {
                _qrTracker = new QRCodeWatcher();
                IsTrackerRunning = false;
                _qrTracker.Added += QRCodeWatcher_Added;
                _qrTracker.Updated += QRCodeWatcher_Updated;
                _qrTracker.Removed += QRCodeWatcher_Removed;
                _qrTracker.EnumerationCompleted += QRCodeWatcher_EnumerationCompleted;
            }
            catch (Exception ex)
            {
                DebugLog("QRCodesManager : exception starting the tracker " + ex);
            }

            if (autoStartQRTracking)
            {
                StartQRTracking();
            }
        }

        public void StartQRTracking()
        {
            if (_qrTracker != null && !IsTrackerRunning)
            {
                DebugLog("QRCodesManager starting QRCodeWatcher");
                try
                {
                    _qrTracker.Start();
                    IsTrackerRunning = true;
                    QRCodesTrackingStateChanged?.Invoke(this, true);
                }
                catch (Exception ex)
                {
                    DebugLog("QRCodesManager starting QRCodeWatcher Exception: " + ex);
                }
            }
        }

        public void StopQRTracking()
        {
            if (IsTrackerRunning)
            {
                IsTrackerRunning = false;
                if (_qrTracker != null)
                {
                    _qrTracker.Stop();
                    lock (_qrCodesList)
                    {
                        _qrCodesList.Clear();
                    }
                }

                QRCodesTrackingStateChanged?.Invoke(this, false);
            }
        }

        private void QRCodeWatcher_Removed(object sender, QRCodeRemovedEventArgs args)
        {
            DebugLog("QRCodesManager QRCodeWatcher_Removed : " + args.Code.Data);
            LogQR(args.Code, false);

            bool found = false;
            lock (_qrCodesList)
            {
                if (_qrCodesList.ContainsKey(args.Code.Id))
                {
                    _qrCodesList.Remove(args.Code.Id);
                    found = true;
                }
            }

            if (found)
            {
                QRCodeRemoved?.Invoke(this, QRCodeEventArgs.Create(args.Code));
            }
        }

        private void QRCodeWatcher_Updated(object sender, QRCodeUpdatedEventArgs args)
        {
            DebugLog("QRCodesManager QRCodeWatcher_Updated : " + args.Code.Data);
            LogQR(args.Code);

            bool found = false;
            lock (_qrCodesList)
            {
                if (_qrCodesList.ContainsKey(args.Code.Id))
                {
                    found = true;
                    _qrCodesList[args.Code.Id] = args.Code;
                }
            }
            if (found)
            {
                QRCodeUpdated?.Invoke(this, QRCodeEventArgs.Create(args.Code));
            }
            else
            {
                AddQRCode(args.Code);
            }
        }

        private void QRCodeWatcher_Added(object sender, QRCodeAddedEventArgs args)
        {
            DebugLog("QRCodesManager QRCodeWatcher_Added : " + args.Code.Data);
            LogQR(args.Code);

            AddQRCode(args.Code);
        }

        private void AddQRCode(QRCode code)
        {
            if (code.LastDetectedTime < _initTime)
            {
                DebugLog($"QRCodesManager Code '{code.Data}' last detected prior to application start, aborting add");
                return;
            }

            lock (_qrCodesList)
            {
                _qrCodesList[code.Id] = code;
            }
            QRCodeAdded?.Invoke(this, QRCodeEventArgs.Create(code));
        }

        private void QRCodeWatcher_EnumerationCompleted(object sender, object e)
        {
            DebugLog("QRCodesManager QRCodeWatcher_EnumerationCompleted");
        }

        private void Update()
        {
            var cam = Camera.main;
            if (cam != null)
            {
                var camTransform = cam.transform;
                _lastCameraPosition = camTransform.position;
                _lastCameraRotation = camTransform.rotation;
            }

            if (_qrTracker == null && _capabilityInitialized && IsSupported)
                if (_accessStatus == QRCodeWatcherAccessStatus.Allowed)
                    SetupQRTracking();
                else
                    DebugLog("Capability access status : " + _accessStatus);

            // If instructed to pop since last frame (QR event occurred)
            if (_popped)
            {
                _popped = false;
                _audioSource.PlayOneShot(_audioSource.clip);
            }

            _writer?.FlushAsync();
        }

        private void OnDestroy()
        {
            _writer?.Close();
            _stream?.Close();
        }
    }
}

#endif
