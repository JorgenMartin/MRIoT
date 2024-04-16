#nullable enable

#if UNITY_WSA

using System;
using Exact;
using Svanesjo.MRIoT.QRCodes.DataVisualizers;
using Svanesjo.MRIoT.GameLogic;
using Svanesjo.MRIoT.Multiplayer.Representation;
using Svanesjo.MRIoT.Utility;
using Unity.VisualScripting;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.Multiplayer.Device
{
    [RequireComponent(typeof(Exact.Device))]
    [RequireComponent(typeof(NetworkedColorRing))]
    public class TileLampDevice : QRDataVisualizer, IMultiplayerDevice
    {
        private TileLampRepresentation? _tileLampRepresentation;
        private Exact.Device _device = null!;
        private ExactManager _exactManager = null!;
        private DemoGameLogic _gameLogic = null!;
        private ILogger _logger = new DebugLogger(typeof(TileLampDevice));

        protected new void Start()
        {
            base.Start();

            _device = GetComponent<Exact.Device>();
            _exactManager = FindFirstObjectByType<ExactManager>();
            if (Code == null || _device == null || _exactManager == null)
                throw new ArgumentNullException();

            _gameLogic = _exactManager.GetComponent<DemoGameLogic>();
            if (_gameLogic == null)
                throw new ArgumentNullException();

            _device.useDeviceName = true;
            _device.SetDeviceName(Code.Data);

            _logger.Log($"adding device {_device.GetDeviceName()}");
            _exactManager.AddDevice(_device);
        }

        private void LampStateChanged(object sender, LampEventArgs e)
        {
            _logger.Log($"lampStateChanged, role: {e.Role}, state: {e.State}");
            _logger.LogWarning("TODO: Doing nothing...");
        }

        private void VirtualTapHappened(object sender, EmptyEventArgs e)
        {
            _logger.Log("virtualTapHappened");
            OnTap();
        }

        public void SetLampRole(TileLampRole role)
        {
            if (_tileLampRepresentation != null)
            {
                _logger.Log($"setting lamp role: {role}");
                _tileLampRepresentation.SetRole(role);
            }
            else
            {
                _logger.LogWarning("tileLampNetwork not found");
            }
        }

        public void OnConnect()
        {
            _logger.Log("onConnect setting lamp state to off");
            SetLampState(TileLampState.Off);
        }

        public void OnDisconnect()
        {
            _logger.Log("onDisconnect setting lamp state to undefined");
            SetLampState(TileLampState.Undefined);
        }

        public void OnTap()
        {
            _logger.Log("forwarding tap to game");
            _gameLogic.OnTapped(_device);
        }

        public void SetLampState(TileLampState state)
        {
            if (_tileLampRepresentation != null)
            {
                _logger.Log($"setting lamp state: {state}");
                _tileLampRepresentation.SetState(state);
            }
            else
            {
                _logger.LogWarning("tileLampNetwork not found");
            }
        }

        public void SetMultiplayerRepresentation(IMultiplayerRepresentation multiplayerRepresentation)
        {
            if (multiplayerRepresentation is not TileLampRepresentation representation)
                throw new ArgumentException();

            // Unregister listener
            if (_tileLampRepresentation != null)
            {
                _tileLampRepresentation.LampStateChanged -= LampStateChanged;
                _tileLampRepresentation.VirtualTapHappened -= VirtualTapHappened;
            }

            _logger.Log("setting MultiplayerRepresentation");
            _tileLampRepresentation = representation;
            representation.LampStateChanged += LampStateChanged;
            representation.VirtualTapHappened += VirtualTapHappened;

            GetComponent<NetworkedColorRing>()
                .Initialize(representation.GetComponent<ColorRingRepresentation>());
        }
    }
}

#endif
