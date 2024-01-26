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
        [Required] [SerializeField] private NetworkManager networkManager;
        [Required] [SerializeField] private UnityTransport unityTransport;

        void Update()
        {
            textMesh.text = $"{GetLocalIPAddress()} connecting to {unityTransport.ConnectionData.Address} as {(networkManager.IsServer ? "Server" : "Client")}: {networkManager.LocalClientId}";
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
