#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Svanesjo.MRIoT.NetcodeSetup
{
    public enum StartType
    {
        Server,
        Host,
        Client
    }

    [RequireComponent(typeof(NetworkManager))]
    public class NetworkLauncher : MonoBehaviour
    {
        [Required, SerializeField] private Material serverMaterial = null!;
        [Required, SerializeField] private Material hostMaterial = null!;
        [Required, SerializeField] private Material clientMaterial = null!;

        [SerializeField] private StartType editorStartType = StartType.Server;
        [SerializeField] private StartType deviceStartType = StartType.Client;

        [Required, SerializeField] private MeshRenderer floor = null!;

        [SerializeField] private int checkConnectionInterval = 10;
        [SerializeField] private int reinitializeIfConnectionLostFor = 30;

        private NetworkManager _networkManager = null!;

        private void Start()
        {
            _networkManager = GetComponent<NetworkManager>();
            if (_networkManager == null)
                throw new MissingComponentException();

            if (serverMaterial is null || hostMaterial is null || clientMaterial is null || floor is null)
                throw new Exception("Required fields are null");

            Initialize();
        }

        private void Initialize()
        {
            Debug.Log("NetworkLauncher starting...");
            StartAs(Application.isEditor ? editorStartType : deviceStartType);
        }

        private void StartAs(StartType startType)
        {
            switch (startType)
            {
                case StartType.Server:
                {
                    Debug.Log("Starting Server...");
                    floor.SetMaterials(new List<Material> { serverMaterial });
                    var state = _networkManager.StartServer();
                    Debug.LogWarning($"NetworkLauncher Started: {state}");
                    break;
                }
                case StartType.Host:
                {
                    Debug.Log("Starting Host...");
                    floor.SetMaterials(new List<Material> { hostMaterial });
                    var state = _networkManager.StartHost();
                    Debug.LogWarning($"NetworkLauncher Started: {state}");
                    break;
                }
                case StartType.Client:
                {
                    Debug.Log("Starting Client...");
                    floor.SetMaterials(new List<Material> { clientMaterial });
                    var state = _networkManager.StartClient();
                    Debug.LogWarning($"NetworkLauncher Started: {state}");
                    StartCoroutine(CheckConnection());
                    break;
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_networkManager == null) return;
            var isServer = _networkManager.IsServer;
            var connected = _networkManager.IsConnectedClient;
            Debug.Log($"NetworkLauncher onApplicationPause paused: {pauseStatus}, connected: {connected}, isServer: {isServer}");
            if (pauseStatus || isServer || connected)
                return;

            Debug.Log("NetworkLauncher onApplicationPause rerunning Initialize");
            Initialize();
        }

        private IEnumerator CheckConnection()
        {
            Debug.Log($"NetworkLauncher checkConnection-coroutine running in background, checking every {checkConnectionInterval} seconds");
            while (true)
            {
                if (_networkManager.IsServer)
                {
                    Debug.LogError("NetworkLauncher checkConnection running on server, which should never happen");
                    break;
                }

                var connected = _networkManager.IsConnectedClient;
                Debug.Log($"NetworkLauncher checkConnection connected: {connected}, rechecking in {checkConnectionInterval} seconds");
                if (!connected)
                {
                    Debug.Log($"NetworkLauncher checkConnection not connected, rechecking in {reinitializeIfConnectionLostFor} seconds before rerunning Initialize");
                    yield return new WaitForSeconds(reinitializeIfConnectionLostFor);

                    connected = _networkManager.IsConnectedClient;
                    if (!connected)
                    {
                        Debug.Log("NetworkLauncher checkConnection still not connected, rerunning Initialize");
                        Initialize();
                        break;
                    }
                }

                yield return new WaitForSeconds(checkConnectionInterval);
            }
        }
    }
}
