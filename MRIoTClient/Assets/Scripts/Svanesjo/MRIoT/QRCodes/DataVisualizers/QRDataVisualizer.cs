#nullable enable

#if UNITY_WSA

using System;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.Multiplayer.Calibration;
using Svanesjo.MRIoT.Multiplayer.Device;
using Svanesjo.MRIoT.Multiplayer.Representation;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.QRCodes.DataVisualizers
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRDataVisualizer : MonoBehaviour
    {
        [SerializeField] protected NetworkObject? networkPrefab;

        public QRCode? Code;

        protected NetworkObject? SpawnedNetworkObject;

        protected virtual void Start()
        {
            if (Code == null)
                throw new Exception("QR Code Empty");

            if (networkPrefab != null)
            {
                Debug.Log($"QRDataVisualizer instantiating prefab from {networkPrefab}");
                // Instantiate prefab and set same transform
                var instance = Instantiate(networkPrefab.gameObject);
                var spawnerTransform = transform;
                instance.transform.position = spawnerTransform.position;
                instance.transform.rotation = spawnerTransform.rotation;

                // If the prefabs are INetworkDevice and INetworkThing, then connect them
                var device = GetComponent<IMultiplayerDevice>();
                var thing = instance.GetComponent<IMultiplayerRepresentation>();
                if (device != null && thing != null)
                {
                    Debug.Log($"QRDataVisualizer connecting thing and device: {thing} and {device}");
                    // thing.SetNetworkDevice(device);
                    device.SetMultiplayerRepresentation(thing);
                }
                else
                {
                    Debug.Log($"QRDataVisualizer could not connect thing and device: {thing} and {device}");
                }

                // Call Spawn on the NetworkObject to replicate on clients
                SpawnedNetworkObject = instance.GetComponent<NetworkObject>();
                Debug.Log($"QRDataVisualizer spawning network object from {SpawnedNetworkObject}");
                SpawnedNetworkObject.Spawn();

                // Re-parent if CalibrationOrigin exists
                var origin = FindFirstObjectByType<CalibrationOrigin>();
                if (origin != null)
                    origin.ReParentIfValid(SpawnedNetworkObject);
            }
            else
            {
                Debug.Log("QRDataVisualizer no network prefab defined");
            }
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (networkPrefab is null || SpawnedNetworkObject == null) return;

            var tile = SpawnedNetworkObject.GetComponent<CalibrationTile>();
            if (tile != null)
                tile.SetPositionAndRotation(position, rotation);
            else
                SpawnedNetworkObject.transform.SetPositionAndRotation(position, rotation);
        }
    }
}

#endif
