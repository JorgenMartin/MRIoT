#nullable enable

using System;
using System.Collections.Generic;
using Svanesjo.MRIoT.Utility;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Multiplayer.Calibration
{
    [RequireComponent(typeof(NetworkObject))]
    public class CalibrationOrigin : NetworkBehaviour
    {
        private NetworkList<ulong> _tileIds = null!;
        private readonly Dictionary<ulong, CalibrationTile> _tileInstances = new();

        private NetworkObject _networkObject = null!;

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            if (_networkObject == null)
                throw new MissingComponentException(nameof(NetworkObject));

            _tileIds = new NetworkList<ulong>();
            _tileIds.OnListChanged += OnTileIdsListChanged;
        }

        private void Start()
        {
            Initialize();
        }

        public override void OnNetworkSpawn()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Register all tiles from _tileIds
            foreach (var id in _tileIds)
                RegisterCalibrationTile(id);

            // Server-side only logic below
            if (!IsServer) return;

            // Register pre-existing calibration tiles
            var preExistingCalibrationTiles =
                FindObjectsByType<CalibrationTile>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var tile in preExistingCalibrationTiles)
                RegisterCalibrationTile(tile);

            // Re-parent pre-existing NetworkObjects
            var networkObjects =
                FindObjectsByType<NetworkObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var networkObject in networkObjects)
                ReParentIfValid(networkObject);
        }

        private void OnTileIdsListChanged(NetworkListEvent<ulong> changeEvent)
        {
            Debug.Log($"CalibrationOrigin OnTileIdsListChanged called with event {changeEvent.Type} {changeEvent.PreviousValue} -> {changeEvent.Value} at index {changeEvent.Index}");
            var networkId = changeEvent.Value;

            // This ReSharper warning is wrong... Disabling once
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (changeEvent.Type)
            {
                case NetworkListEvent<ulong>.EventType.Add:
                case NetworkListEvent<ulong>.EventType.Insert:
                    RegisterCalibrationTile(networkId);
                    break;
                case NetworkListEvent<ulong>.EventType.Remove:
                case NetworkListEvent<ulong>.EventType.RemoveAt:
                    RemoveCalibrationTileInstance(networkId);
                    break;
                case NetworkListEvent<ulong>.EventType.Value:
                case NetworkListEvent<ulong>.EventType.Full:
                    throw new InvalidOperationException($"Values in the {nameof(_tileIds)} list should never be changed/replaced!");
                case NetworkListEvent<ulong>.EventType.Clear:
                    _tileInstances.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RegisterCalibrationTile(ulong networkId)
        {
            Debug.Log($"CalibrationOrigin RegisterCalibrationTile called with network id '{networkId}'");
            var tile = GetNetworkObject(networkId)?.GetComponent<CalibrationTile>();
            if (tile == null)
            {
                Debug.LogError($"CalibrationOrigin RegisterCalibrationTileInstance calibration tile with network id '{networkId}' not found");
                return;
            }

            RegisterCalibrationTile(tile);
        }

        public void RegisterCalibrationTile(CalibrationTile calibrationTile)
        {
            Debug.Log($"CalibrationOrigin RegisterCalibrationTile called with '{calibrationTile}'");
            var networkId = calibrationTile.GetComponent<NetworkObject>().NetworkObjectId;

            if (!_tileIds.Contains(networkId))
                _tileIds.Add(networkId);

            if (_tileInstances.ContainsValue(calibrationTile)) return;

            _tileInstances.TryAdd(networkId, calibrationTile);
            calibrationTile.OnMoved += OnCalibrationTileMoved;
        }

        private void RemoveCalibrationTileInstance(ulong networkId)
        {
            _tileInstances.Remove(networkId);
        }

        private void OnCalibrationTileMoved()
        {
            if (_tileInstances.Count < 1) return;

            // Locate the origin at the mean of all the CalibrationTiles
            var originTransform = transform;
            List<Vector3> positions = new();
            foreach (var tileId in _tileIds)
                positions.Add(_tileInstances[tileId].transform.position);
            var position = VectorTools.GetMeanVector(positions);
            originTransform.position = position;

            // Get direction by pointing the origin towards the first tile in the NetworkList _tileIds
            var directionTile = _tileInstances[_tileIds[0]];
            var direction = position - directionTile.transform.position;
            // Use y=0, assuming each device can handle up-down correctly
            direction.y = 0f;
            originTransform.forward = direction;
        }

        public void ReParentIfValid(NetworkObject networkObject)
        {
            // Do not re-parent CalibrationOrigin or CalibrationTile
            if (networkObject.GetComponent<CalibrationOrigin>() != null
                || networkObject.GetComponent<CalibrationTile>() != null)
                return;

            if (networkObject.TrySetParent(gameObject, false))
                Debug.Log($"CalibrationOrigin ReParentIfValid successfully set parent of {networkObject}", this);
            else
                Debug.LogError($"CalibrationOrigin ReParentIfValid failed to set parent of {networkObject}");
        }

        private void Update()
        {
            if (_tileIds.Count != _tileInstances.Count)
            {
                Debug.LogWarning($"CalibrationOrigin Update dictionary out of sync with list ({_tileInstances.Count} vs {_tileIds.Count})");
            }
        }
    }
}
