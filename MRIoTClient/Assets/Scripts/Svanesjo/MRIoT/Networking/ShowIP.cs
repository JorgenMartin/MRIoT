using System;
using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    public class ShowIP : MonoBehaviour
    {
        [Required] [SerializeField] private TextMesh textMesh;
        private NetworkManager _networkManager;
        private UnityTransport _unityTransport;

        private void Start()
        {
            _networkManager = FindFirstObjectByType<NetworkManager>();
            if (_networkManager is null)
            {
                throw new Exception("Network Manager not found");
            }

            _unityTransport = FindFirstObjectByType<UnityTransport>();
            if (_unityTransport is null)
            {
                throw new Exception("Unity Transport not found");
            }
        }

        void Update()
        {
            textMesh.text = $"{GetLocalIPAddress()} connecting to {_unityTransport.ConnectionData.Address} as {(_networkManager.IsServer ? "Server" : "Client")}: {_networkManager.LocalClientId}";
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
