using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace MRIoT.Scripts.Networking
{
    public class ShowIP : MonoBehaviour
    {
        [Required] [SerializeField] private TextMesh _textMesh;
        [Required] [SerializeField] private NetworkManager _networkManager;
        [Required] [SerializeField] private UnityTransport _unityTransport;

        void Update()
        {
            _textMesh.text = $"{GetLocalIPAddress()} connecting to {_unityTransport.ConnectionData.Address} as {(_networkManager.IsServer ? "Server" : "Client")}: {_networkManager.LocalClientId}";
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
