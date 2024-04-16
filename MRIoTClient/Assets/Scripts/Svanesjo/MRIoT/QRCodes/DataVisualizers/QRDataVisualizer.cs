#nullable enable

#if UNITY_WSA

using System;
using Microsoft.MixedReality.QR;
using Svanesjo.MRIoT.Multiplayer.Calibration;
using Svanesjo.MRIoT.Multiplayer.Device;
using Svanesjo.MRIoT.Multiplayer.Representation;
using Svanesjo.MRIoT.Utility;
using Unity.Netcode;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.QRCodes.DataVisualizers
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRDataVisualizer : MonoBehaviour
    {
        [SerializeField] protected NetworkObject? networkPrefab;

        public QRCode? Code;

        protected NetworkObject? SpawnedNetworkObject;

        private ILogger _logger = new DebugLogger(typeof(QRDataVisualizer));

        protected virtual void Start()
        {
            if (Code == null)
                throw new Exception("QR Code Empty");

            if (networkPrefab != null)
            {
                _logger.Log($"instantiating prefab from {networkPrefab}");
                // Instantiate prefab and set same transform
                var instance = Instantiate(networkPrefab.gameObject);
                var spawnerTransform = transform;
                instance.transform.position = spawnerTransform.position;
                instance.transform.rotation = spawnerTransform.rotation;

                // If the prefabs are IMultiplayerDevice and IMultiplayerRepresentation, then connect them
                var device = GetComponent<IMultiplayerDevice>();
                var representation = instance.GetComponent<IMultiplayerRepresentation>();
                if (device != null && representation != null)
                {
                    _logger.Log($"connecting representation and device: {representation} and {device}");
                    device.SetMultiplayerRepresentation(representation);
                }
                else
                {
                    _logger.Log($"could not connect representation and device: {representation} and {device}");
                }

                // Call Spawn on the NetworkObject to replicate on clients
                SpawnedNetworkObject = instance.GetComponent<NetworkObject>();
                _logger.Log($"spawning network object from {SpawnedNetworkObject}");
                SpawnedNetworkObject.Spawn();

                // Re-parent if CalibrationOrigin exists
                var origin = FindFirstObjectByType<CalibrationOrigin>();
                if (origin != null)
                    origin.ReParentIfValid(SpawnedNetworkObject);
            }
            else
            {
                _logger.Log("no network prefab defined");
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
