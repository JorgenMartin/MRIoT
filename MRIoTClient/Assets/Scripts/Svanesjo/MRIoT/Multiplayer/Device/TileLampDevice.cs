#nullable enable

#if UNITY_WSA

using System;
using Exact;
using Svanesjo.MRIoT.QRCodes.DataVisualizers;
using Svanesjo.MRIoT.GameLogic;
using Svanesjo.MRIoT.Multiplayer.Representation;
using Unity.VisualScripting;
using UnityEngine;

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

            Debug.Log($"TileLampDevice adding device {_device.GetDeviceName()}");
            _exactManager.AddDevice(_device);
        }

        private void LampStateChanged(object sender, LampEventArgs e)
        {
            Debug.Log($"TileLampDevice lampStateChanged, role: {e.Role}, state: {e.State}");
            Debug.LogWarning("TODO: Doing nothing...");
        }

        private void VirtualTapHappened(object sender, EmptyEventArgs e)
        {
            Debug.Log("TileLampDevice virtualTapHappened");
            OnTap();
        }

        public void SetLampRole(TileLampRole role)
        {
            if (_tileLampRepresentation != null)
            {
                Debug.Log($"TileLampDevice setting lamp role: {role}");
                _tileLampRepresentation.SetRole(role);
            }
            else
            {
                Debug.LogWarning("TileLampDevice tileLampNetwork not found");
            }
        }

        public void OnConnect()
        {
            Debug.Log("TileLampDevice onConnect setting lamp state to off");
            SetLampState(TileLampState.Off);
        }

        public void OnDisconnect()
        {
            Debug.Log("TileLampDevice onDisconnect setting lamp state to undefined");
            SetLampState(TileLampState.Undefined);
        }

        public void OnTap()
        {
            Debug.Log("TileLampDevice forwarding tap to game");
            _gameLogic.OnTapped(_device);
        }

        public void SetLampState(TileLampState state)
        {
            if (_tileLampRepresentation != null)
            {
                Debug.Log($"TileLampDevice setting lamp state: {state}");
                _tileLampRepresentation.SetState(state);
            }
            else
            {
                Debug.LogWarning("TileLampDevice tileLampNetwork not found");
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

            Debug.Log("TileLampDevice setting NetworkThing");
            _tileLampRepresentation = representation;
            representation.LampStateChanged += LampStateChanged;
            representation.VirtualTapHappened += VirtualTapHappened;

            GetComponent<NetworkedColorRing>()
                .Initialize(representation.GetComponent<ColorRingRepresentation>());
        }
    }
}

#endif
