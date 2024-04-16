#nullable enable

#if UNITY_WSA

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.QRCodes.DataVisualizers;
using Svanesjo.MRIoT.Utility;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.QRCodes
{
    [Serializable]
    public class QRCodePrefabEntry
    {
        public string? key;
        public QRDataVisualizer? prefab;
    }

    public class QRVisualSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<QRCodePrefabEntry> visualizerPrefabsList = new();
        private readonly Dictionary<string, QRDataVisualizer?> _visualizerPrefabsMap = new();

        [SerializeField]
        private QRDataVisualizer? fallbackPrefab;

        private readonly SortedDictionary<string, GameObject> _visualizersList = new();
        private bool _clearExisting; // = false
        private ILogger _logger = new DebugLogger(typeof(QRVisualSpawner));

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            foreach (var entry in visualizerPrefabsList)
            {
                if (entry.key is null)
                {
                    throw new Exception("Prefab key cannot be null");
                }

                _visualizerPrefabsMap.Add(entry.key, entry.prefab);
            }
        }

        private struct ActionData
        {
            public enum ActionType
            {
                Added,
                Updated,
                Removed
            }

            public readonly ActionType Type;
            public readonly QRCode Code;

            public ActionData(ActionType type, QRCode qrCode) : this()
            {
                Type = type;
                Code = qrCode;
            }
        }

        private readonly Queue<ActionData> _pendingActions = new();

        // Start is called before the first frame update
        void Start()
        {
            var qrLogger = QRCodesManager.Instance.Logger;
            if (qrLogger is FileLogger fileLogger)
                _logger = fileLogger.CreateSibling(typeof(QRVisualSpawner));

            _logger.Log("start");

            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
        }

        private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                _clearExisting = true;
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<QRCode> e)
        {
            _logger.Log("Instance_QRCodeAdded");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCode> e)
        {
            _logger.Log("Instance_QRCodeUpdated");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCode> e)
        {
            _logger.Log("Instance_QRCodeRemoved");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Removed, e.Data));
            }
        }

        private QRDataVisualizer? GetPrefabFromMap(QRCode qrCode)
        {
            if (_visualizerPrefabsMap.TryGetValue(qrCode.Data, out QRDataVisualizer? prefab))
            {
                return prefab;
            }

            _logger.LogWarning($"no prefab found for QRData = '{qrCode.Data}', using fallback prefab.");
            return fallbackPrefab;
        }

        private void HandleEvents()
        {
            lock (_pendingActions)
            {
                while (_pendingActions.Count > 0)
                {
                    var action = _pendingActions.Dequeue();
                    switch (action.Type)
                    {
                        case ActionData.ActionType.Added when _visualizersList.ContainsKey(action.Code.Data):
                        {
                            _logger.LogError($"aborted adding already existing visualizer");
                            break;
                        }
                        case ActionData.ActionType.Added:
                        {
                            var visualizer = GetPrefabFromMap(action.Code);
                            if (visualizer is null)
                            {
                                _logger.Log($"prefab for '{action.Code.Data}' is null and it will not be added");
                                return;
                            }

                            var prefab = visualizer.gameObject;
                            var visualizerObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                            visualizerObject.GetComponent<SpatialGraphNodeTracker>().Id = action.Code.SpatialGraphNodeId;
                            visualizerObject.GetComponent<QRDataVisualizer>().Code = action.Code;
                            _logger.Log("adding code with data = " + action.Code.Data);
                            _visualizersList.Add(action.Code.Data, visualizerObject);
                            break;
                        }
                        case ActionData.ActionType.Updated when !_visualizersList.ContainsKey(action.Code.Data):
                            _logger.LogError($"updating non-existent visualizer for data = '{action.Code.Data}', ignoring");
                            break;
                        case ActionData.ActionType.Updated:
                            _logger.Log("updating code with data = " + action.Code.Data);
                            break;
                        case ActionData.ActionType.Removed when _visualizersList.ContainsKey(action.Code.Data):
                            _logger.LogError("destroying code with data = " + action.Code.Data);
                            // Destroy(_visualizersList[action.Code.Data]);
                            // _visualizersList.Remove(action.Code.Data);
                            break;
                        case ActionData.ActionType.Removed:
                            _logger.Log("has already destroyed code with data = " + action.Code.Data);
                            break;
                    }
                }
            }

            if (_clearExisting)
            {
                _clearExisting = false;
                foreach (var obj in _visualizersList)
                {
                    Destroy(obj.Value);
                }
                _visualizersList.Clear();
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleEvents();
        }
    }
}

#endif
