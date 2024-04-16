#nullable enable

#if UNITY_WSA

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.QRCodes.DataVisualizers;
using UnityEngine;

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
            Debug.Log("QRVisualSpawner start");

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
            Debug.Log("QRVisualSpawner Instance_QRCodeAdded");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCode> e)
        {
            Debug.Log("QRVisualSpawner Instance_QRCodeUpdated");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCode> e)
        {
            Debug.Log("QRVisualSpawner Instance_QRCodeRemoved");

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

            Debug.LogWarning($"QRVisualSpawner no prefab found for QRData = '{qrCode.Data}', using fallback prefab.");
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
                            Debug.LogError($"QRVisualSpawner aborted adding already existing visualizer");
                            break;
                        }
                        case ActionData.ActionType.Added:
                        {
                            QRDataVisualizer? visualizer = GetPrefabFromMap(action.Code);
                            if (visualizer is null)
                            {
                                Debug.Log($"QRVisualSpawner prefab for '{action.Code.Data}' is null and it will not be added");
                                return;
                            }

                            GameObject prefab = visualizer.gameObject;
                            GameObject visualizerObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

                            visualizerObject.GetComponent<SpatialGraphNodeTracker>().Id = action.Code.SpatialGraphNodeId;
                            visualizerObject.GetComponent<QRDataVisualizer>().Code = action.Code;
                            Debug.Log("QRVisualSpawner adding code with data = " + action.Code.Data);
                            _visualizersList.Add(action.Code.Data, visualizerObject);
                            break;
                        }
                        case ActionData.ActionType.Updated when !_visualizersList.ContainsKey(action.Code.Data):
                            Debug.LogError($"QRVisualSpawner updating non-existent visualizer for data = '{action.Code.Data}', ignoring");
                            break;
                        case ActionData.ActionType.Updated:
                            Debug.Log("QRVisualSpawner updating code with data = " + action.Code.Data);
                            break;
                        case ActionData.ActionType.Removed when _visualizersList.ContainsKey(action.Code.Data):
                            Debug.LogError("QRVisualSpawner destroying code with data = " + action.Code.Data);
                            // Destroy(_visualizersList[action.Code.Data]);
                            // _visualizersList.Remove(action.Code.Data);
                            break;
                        case ActionData.ActionType.Removed:
                            Debug.Log("QRVisualSpawner has already destroyed code with data = " + action.Code.Data);
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
