#if UNITY_WSA

using System;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.QRCodes;
using Svanesjo.MRIoT.Things.Network;
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

        protected void Start()
        {
            if (Code == null)
            {
                throw new Exception("QR Code Empty");
            }

            if (networkPrefab != null)
            {
                Debug.Log($"QRDataVisualizer instantiating prefab from {networkPrefab}");
                // Instantiate prefab and set same transform
                var instance = Instantiate(networkPrefab.gameObject);
                var spawnerTransform = transform;
                instance.transform.position = spawnerTransform.position;
                instance.transform.rotation = spawnerTransform.rotation;

                // If the prefabs are INetworkDevice and INetworkThing, then connect them
                var device = GetComponent<INetworkDevice>();
                var thing = instance.GetComponent<INetworkThing>();
                if (device != null && thing != null)
                {
                    Debug.Log($"QRDataVisualizer connecting thing and device: {thing} and {device}");
                    // thing.SetNetworkDevice(device);
                    device.SetNetworkThing(thing);
                }
                else
                {
                    Debug.Log($"QRDataVisualizer could not connect thing and device: {thing} and {device}");
                }

                // Call Spawn on the NetworkObject to replicate on clients
                _spawnedNetworkObject = instance.GetComponent<NetworkObject>();
                Debug.Log($"QRDataVisualizer spawning network object from {_spawnedNetworkObject}");
                _spawnedNetworkObject.Spawn();
            }
            else
            {
                Debug.Log("QRDataVisualizer no network prefab defined");
            }
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (networkPrefab is not null && _spawnedNetworkObject != null)
            {
                _spawnedNetworkObject.transform.SetPositionAndRotation(position, rotation);
            }
        }
    }
}

#endif
