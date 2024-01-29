#if UNITY_WSA

using System;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.QRCodes;
using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.DataVisualizers
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRDataVisualizer : MonoBehaviour
    {
        [SerializeField] protected NetworkObject? networkPrefab;

        public QRCode? Code;

        private NetworkObject? _spawnedNetworkObject;

        private void Start()
        {
            if (Code == null)
            {
                throw new Exception("QR Code Empty");
            }

            if (networkPrefab != null)
            {
                Debug.Log($"QRDataVisualizer instantiating prefab from {networkPrefab}");
                var instance = Instantiate(networkPrefab.gameObject);
                var spawnerTransform = transform;
                instance.transform.position = spawnerTransform.position;
                instance.transform.rotation = spawnerTransform.rotation;

                _spawnedNetworkObject = instance.GetComponent<NetworkObject>();
                Debug.Log($"QRDataVisualizer spawning network object from {_spawnedNetworkObject}");
                _spawnedNetworkObject.Spawn();
            }
            else
            {
                Debug.Log("QRDataVisualizer no network prefab defined");
            }
        }

        private void Update()
        {
            if (networkPrefab != null && _spawnedNetworkObject != null)
            {
                _spawnedNetworkObject.transform.position = transform.position;
                _spawnedNetworkObject.transform.rotation = transform.rotation;
            }
        }
    }
}

#endif
