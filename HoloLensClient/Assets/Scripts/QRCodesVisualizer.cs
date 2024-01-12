using System;
using System.Collections.Generic;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.DataVisualizers;
using UnityEngine;

namespace Svanesjo.MRIoT
{
    [Serializable]
    public class StringGameObjectPair
    {
        public string key;
        public GameObject value;
    }

    public class QRCodesVisualizer : MonoBehaviour
    {
        [SerializeField]
        private List<StringGameObjectPair> visualizerPrefabsList = new();
        private Dictionary<string, GameObject> _visualizerPrefabsMap = new();

        public GameObject fallbackPrefab;

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            foreach (var pair in visualizerPrefabsList)
            {
                if (pair.value.GetComponent<QRDataVisualizer>() == null)
                {
                    throw new Exception("Prefab must extend QRDataVisualizer");
                }
                _visualizerPrefabsMap.Add(pair.key, pair.value);
            }
        }

        private SortedDictionary<Guid, GameObject> _visualizersList;
        private bool _clearExisting = false;

        private struct ActionData
        {
            public enum Type
            {
                Added,
                Updated,
                Removed
            }

            public readonly Type type;
            public readonly QRCode qrCode;

            public ActionData(Type type, QRCode qrCode) : this()
            {
                this.type = type;
                this.qrCode = qrCode;
            }
        }

        private readonly Queue<ActionData> _pendingActions = new();

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("QRCodesVisualizer start");
            _visualizersList = new SortedDictionary<Guid, GameObject>();

            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;

            if (fallbackPrefab == null)
            {
                throw new Exception("Fallback Prefab not assigned");
            }
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
                _pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeRemoved");

            lock (_pendingActions)
            {
                _pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        private GameObject GetPrefabFromMap(QRCode qrCode)
        {
            if (_visualizerPrefabsMap.TryGetValue(qrCode.Data, out GameObject prefab))
            {
                return prefab;
            }

            Debug.LogWarning("QRCodesVisualizer no prefab found for QRDdata = '" + qrCode.Data +"', using fallback prefab.");
            return fallbackPrefab;
        }

        private void HandleEvents()
        {
            lock (_pendingActions)
            {
                while (_pendingActions.Count > 0)
                {
                    var action = _pendingActions.Dequeue();
                    if (action.type == ActionData.Type.Added)
                    {
                        GameObject prefab = GetPrefabFromMap(action.qrCode);
                        GameObject visualizerObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

                        visualizerObject.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                        visualizerObject.GetComponent<QRDataVisualizer>().qrCode = action.qrCode;
                        Debug.Log("QRCodesVisualizer adding code with data = " + action.qrCode.Data);
                        _visualizersList.Add(action.qrCode.Id, visualizerObject);
                    }
                    else if (action.type == ActionData.Type.Updated)
                    {
                        if (!_visualizersList.ContainsKey(action.qrCode.Id))
                        {
                            GameObject prefab = GetPrefabFromMap(action.qrCode);
                            GameObject visualizerObject =
                                    Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);

                            visualizerObject.GetComponent<SpatialGraphNodeTracker>().Id =
                                action.qrCode.SpatialGraphNodeId;
                            visualizerObject.GetComponent<QRDataVisualizer>().qrCode = action.qrCode;
                            Debug.Log("QRCodesVisualizer adding updated code with data = " + action.qrCode.Data);
                            _visualizersList.Add(action.qrCode.Id, visualizerObject);
                        }
                        else
                        {
                            Debug.Log("QRCodesVisualizer updating code with data = " + action.qrCode.Data);
                        }
                    }
                    else if (action.type == ActionData.Type.Removed)
                    {
                        if (_visualizersList.ContainsKey(action.qrCode.Id))
                        {
                            Debug.Log("QRCodesVisualizer destroying code with data = " + action.qrCode.Data);
                            Destroy(_visualizersList[action.qrCode.Id]);
                            _visualizersList.Remove(action.qrCode.Id);
                        }
                        else
                        {
                            Debug.Log("QRCodesVisualizer has already destroyed code with data = " + action.qrCode.Data);
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
