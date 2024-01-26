using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using Application = UnityEngine.Device.Application;

#nullable enable

namespace Svanesjo.MRIoT.Networking
{
    public enum StartType
    {
        Server,
        Host,
        Client
    }

    public class NetworkLauncher : MonoBehaviour
    {
        [Required] public Material serverMaterial = null!;
        [Required] public Material hostMaterial = null!;
        [Required] public Material clientMaterial = null!;

        public StartType editorStartType = StartType.Server;
        public StartType deviceStartType = StartType.Client;

        private void Start()
        {
            Debug.Log("NetworkLauncher starting...");
            StartAs(Application.isEditor ? editorStartType : deviceStartType);
        }

        private void StartAs(StartType startType)
        {
            NetworkManager networkManager = GetComponentInParent<NetworkManager>();
            switch (startType)
            {
                case StartType.Server:
                {
                    Debug.Log("Starting Server...");
                    GameObject.Find("FloorPlane").GetComponent<MeshRenderer>()
                        .SetMaterials(new List<Material> { serverMaterial });
                    networkManager.StartServer();
                    break;
                }
                case StartType.Host:
                {
                    Debug.Log("Starting Host...");
                    GameObject.Find("FloorPlane").GetComponent<MeshRenderer>()
                        .SetMaterials(new List<Material> { hostMaterial });
                    networkManager.StartHost();
                    break;
                }
                case StartType.Client:
                {
                    Debug.Log("Starting Client...");
                    GameObject.Find("FloorPlane").GetComponent<MeshRenderer>()
                        .SetMaterials(new List<Material> { clientMaterial });
                    networkManager.StartClient();
                    break;
                }
            }
        }
    }
}
