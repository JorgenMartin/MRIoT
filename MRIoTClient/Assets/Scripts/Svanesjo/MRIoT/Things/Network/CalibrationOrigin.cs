#nullable enable

using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Things.Network
{
    public class CalibrationOrigin : NetworkBehaviour
    {
        private void Start()
        {
            Initialize();
        }

        public override void OnNetworkSpawn()
        {
            Initialize();
            base.OnNetworkSpawn();
        }

        private void Initialize()
        {
            if (!IsServer) return;

            var arr = FindObjectsByType<NetworkObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var networkObject in arr)
            {
                if (networkObject.TrySetParent(gameObject, false))
                    Debug.Log($"CalibrationOrigin Initialize successfully set parent of {networkObject}");
                else
                    Debug.LogWarning($"CalibrationOrigin Initialize failed to set parent of {networkObject}");
            }
        }
    }
}
