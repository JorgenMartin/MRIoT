using System;
using System.Collections.Generic;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.DataVisualizers;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT
{
    [Serializable]
    public class StringGameObjectPair
    {
        public string? key;
        public GameObject? value;
    }

    public class QRCodesVisualizer : MonoBehaviour
    {
        [SerializeField]
        private List<StringGameObjectPair> visualizerPrefabsList = new();
        private readonly Dictionary<string, GameObject?> _visualizerPrefabsMap = new();

        [SerializeField]
        private GameObject? fallbackPrefab;

        private readonly SortedDictionary<Guid, GameObject> _visualizersList = new();
        private bool _clearExisting = false;

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            foreach (var pair in visualizerPrefabsList)
            {
                if (pair.key is null)
                {
                    throw new Exception("Prefab key cannot be null");
                }
                if (pair.value is not null && pair.value.GetComponent<QRDataVisualizer>() == null)
                {
                    throw new Exception("Prefab must extend QRDataVisualizer");
                }
                _visualizerPrefabsMap.Add(pair.key, pair.value);
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
            Debug.Log("QRCodesVisualizer start");

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
            Debug.Log("QRCodesVisualizer Instance_QRCodeAdded");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeRemoved");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.ActionType.Removed, e.Data));
            }
        }

        private GameObject? GetPrefabFromMap(QRCode qrCode)
        {
            if (_visualizerPrefabsMap.TryGetValue(qrCode.Data, out GameObject? prefab))
            {
                return prefab;
            }

            Debug.LogWarning($"QRCodesVisualizer no prefab found for QRData = '{qrCode.Data}', using fallback prefab.");
            return fallbackPrefab;
        }

        private void HandleEvents()
        {
            lock (_pendingActions)
            {
                while (_pendingActions.Count > 0)
                {
                    var action = _pendingActions.Dequeue();
                    if (action.Type == ActionData.ActionType.Added)
                    {
                        GameObject? prefab = GetPrefabFromMap(action.Code);
                        if (prefab is null)
                        {
                            Debug.Log($"QRCodesVisualizer prefab for '{action.Code.Data}' is null and it will not be added");
                            return;
                        }
                        GameObject visualizerObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

                        visualizerObject.GetComponent<SpatialGraphNodeTracker>().Id = action.Code.SpatialGraphNodeId;
                        visualizerObject.GetComponent<QRDataVisualizer>().Code = action.Code;
                        Debug.Log("QRCodesVisualizer adding code with data = " + action.Code.Data);
                        _visualizersList.Add(action.Code.Id, visualizerObject);
                    }
                    else if (action.Type == ActionData.ActionType.Updated)
                    {
                        if (!_visualizersList.ContainsKey(action.Code.Id))
                        {
                            Debug.LogError($"QRCodesVisualizer updating non-existent visualizer for data = '{action.Code.Data}', ignoring");
                        }
                        else
                        {
                            Debug.Log("QRCodesVisualizer updating code with data = " + action.Code.Data);
                        }
                    }
                    else if (action.Type == ActionData.ActionType.Removed)
                    {
                        if (_visualizersList.ContainsKey(action.Code.Id))
                        {
                            Debug.Log("QRCodesVisualizer destroying code with data = " + action.Code.Data);
                            Destroy(_visualizersList[action.Code.Id]);
                            _visualizersList.Remove(action.Code.Id);
                        }
                        else
                        {
                            Debug.Log("QRCodesVisualizer has already destroyed code with data = " + action.Code.Data);
                        }
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
