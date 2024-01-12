using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.Utility;
using UnityEngine;

namespace Svanesjo.MRIoT
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

    public class QRCodesManager : Singleton<QRCodesManager>
    {
        public bool AutoStartQRTracking = true;
        public bool IsTrackerRunning { get; private set; }
        public bool IsSupported { get; private set; }

        public event EventHandler<bool> QRCodesTrackingStateChanged;
        public event EventHandler<QRCodeEventArgs<QRCode>> QRCodeAdded;
        public event EventHandler<QRCodeEventArgs<QRCode>> QRCodeUpdated;
        public event EventHandler<QRCodeEventArgs<QRCode>> QRCodeRemoved;
        private SortedDictionary<Guid, QRCode> _qrCodesList = new();

        private QRCodeWatcher _qrTracker;
        private bool _capabilityInitialized = false;
        private QRCodeWatcherAccessStatus _accessStatus;
        private Task<QRCodeWatcherAccessStatus> _capabilityTask;
        
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

        protected virtual async void Start()
        {
            IsSupported = QRCodeWatcher.IsSupported();
            _capabilityTask = QRCodeWatcher.RequestAccessAsync();
            _accessStatus = await _capabilityTask;
            _capabilityInitialized = true;
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
                Debug.Log("QRCodesManager : exception starting the tracker " + ex);
            }

            if (AutoStartQRTracking)
            {
                StartQRTracking();
            }
        }

        public void StartQRTracking()
        {
            if (_qrTracker != null && !IsTrackerRunning)
            {
                Debug.Log("QRCodesManager starting QRCodeWatcher");
                try
                {
                    _qrTracker.Start();
                    IsTrackerRunning = true;
                    QRCodesTrackingStateChanged?.Invoke(this, true);
                }
                catch (Exception ex)
                {
                    Debug.Log("QRCodesManager starting QRCodeWatcher Exception: " + ex);
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
            Debug.Log("QRCodesManager QRCodeWatcher_Removed : " + args.Code.Data);

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
            Debug.Log("QRCodesManager QRCodeWatcher_Updated : " + args.Code.Data);

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
        }

        private void QRCodeWatcher_Added(object sender, QRCodeAddedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Added : " + args.Code.Data);

            lock (_qrCodesList)
            {
                _qrCodesList[args.Code.Id] = args.Code;
            }
            QRCodeAdded?.Invoke(this, QRCodeEventArgs.Create(args.Code));
        }

        private void QRCodeWatcher_EnumerationCompleted(object sender, object e)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_EnumerationCompleted");
        }

        private void Update()
        {
            if (_qrTracker == null && _capabilityInitialized && IsSupported)
            {
                if (_accessStatus == QRCodeWatcherAccessStatus.Allowed)
                {
                    SetupQRTracking();
                }
                else
                {
                    Debug.Log("Capability access status : " + _accessStatus);
                }
            }
        }
    }
}
