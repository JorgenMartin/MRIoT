#nullable enable

using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Svanesjo.MRIoT.Things.Network
{
    [RequireComponent(typeof(XRGrabInteractable))]
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

            // Disable interaction on the server, since it will use QR Codes instead
            GetComponent<XRGrabInteractable>().enabled = false;

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
