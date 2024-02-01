using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    public class RpcTest : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            Debug.Log($"NetworkSpawn Server: {IsServer}, Owner: {IsOwner}");
            if (!IsServer &&
                IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
            {
                TestServerRpc(0, NetworkObjectId);
            }
        }

        [ClientRpc]
        void TestClientRpc(int value, ulong sourceNetworkObjectId)
        {
            if (value > 10)
            {
                Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}, and stopped the chain");
                return;
            }
            Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
            {
                TestServerRpc(value + 1, sourceNetworkObjectId);
            }
        }

        [ServerRpc]
        void TestServerRpc(int value, ulong sourceNetworkObjectId)
        {
            if (value > 10)
            {
                Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}, and stopped the chain");
                return;
            }
            Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
            TestClientRpc(value, sourceNetworkObjectId);
        }
    }
}
