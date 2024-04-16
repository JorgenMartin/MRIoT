#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Svanesjo.MRIoT.Utility;
using Unity.Netcode;
using UnityEngine;
using Application = UnityEngine.Device.Application;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

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
        private ILogger _logger = new DebugLogger(typeof(NetworkLauncher));

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
            _logger.Log("starting...");
            StartAs(Application.isEditor ? editorStartType : deviceStartType);
        }

        private void StartAs(StartType startType)
        {
            switch (startType)
            {
                case StartType.Server:
                {
                    _logger.Log("Starting Server...");
                    floor.SetMaterials(new List<Material> { serverMaterial });
                    var state = _networkManager.StartServer();
                    _logger.LogWarning($"Started: {state}");
                    break;
                }
                case StartType.Host:
                {
                    _logger.Log("Starting Host...");
                    floor.SetMaterials(new List<Material> { hostMaterial });
                    var state = _networkManager.StartHost();
                    _logger.LogWarning($"Started: {state}");
                    break;
                }
                case StartType.Client:
                {
                    _logger.Log("Starting Client...");
                    floor.SetMaterials(new List<Material> { clientMaterial });
                    var state = _networkManager.StartClient();
                    _logger.LogWarning($"Started: {state}");
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
            _logger.Log($"onApplicationPause paused: {pauseStatus}, connected: {connected}, isServer: {isServer}");
            if (pauseStatus || isServer || connected)
                return;

            _logger.Log("onApplicationPause rerunning Initialize");
            Initialize();
        }

        private IEnumerator CheckConnection()
        {
            _logger.Log($"checkConnection-coroutine running in background, checking every {checkConnectionInterval} seconds");
            while (true)
            {
                if (_networkManager.IsServer)
                {
                    _logger.LogError("checkConnection running on server, which should never happen");
                    break;
                }

                var connected = _networkManager.IsConnectedClient;
                _logger.Log($"checkConnection connected: {connected}, rechecking in {checkConnectionInterval} seconds");
                if (!connected)
                {
                    _logger.Log($"checkConnection not connected, rechecking in {reinitializeIfConnectionLostFor} seconds before rerunning Initialize");
                    yield return new WaitForSeconds(reinitializeIfConnectionLostFor);

                    connected = _networkManager.IsConnectedClient;
                    if (!connected)
                    {
                        _logger.Log("checkConnection still not connected, rerunning Initialize");
                        Initialize();
                        break;
                    }
                }

                yield return new WaitForSeconds(checkConnectionInterval);
            }
        }
    }
}
