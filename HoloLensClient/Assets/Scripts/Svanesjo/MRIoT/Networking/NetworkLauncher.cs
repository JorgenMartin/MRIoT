using System;
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
        [Required, SerializeField] private Material serverMaterial = null!;
        [Required, SerializeField] private Material hostMaterial = null!;
        [Required, SerializeField] private Material clientMaterial = null!;

        [SerializeField] private StartType editorStartType = StartType.Server;
        [SerializeField] private StartType deviceStartType = StartType.Client;

        [Required, SerializeField] private MeshRenderer floor = null!;

        private void Start()
        {
            if (serverMaterial is null || hostMaterial is null || clientMaterial is null || floor is null)
            {
                throw new Exception("Required fields are null");
            }

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
                    floor.SetMaterials(new List<Material> { serverMaterial });
                    var state = networkManager.StartServer();
                    Debug.LogWarning($"NetworkLauncher Started: {state}");
                    break;
                }
                case StartType.Host:
                {
                    Debug.Log("Starting Host...");
                    floor.SetMaterials(new List<Material> { hostMaterial });
                    var state = networkManager.StartHost();
                    Debug.LogWarning($"NetworkLauncher Started: {state}");
                    break;
                }
                case StartType.Client:
                {
                    Debug.Log("Starting Client...");
                    floor.SetMaterials(new List<Material> { clientMaterial });
                    var state = networkManager.StartClient();
                    Debug.LogWarning($"NetworkLauncher  Started: {state}");
                    break;
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log($"NetworkLauncher onApplicationPause({pauseStatus})");
            if (pauseStatus) return;
            Start();
        }
    }
}
