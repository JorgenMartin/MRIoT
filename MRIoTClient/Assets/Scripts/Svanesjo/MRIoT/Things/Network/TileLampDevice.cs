#if UNITY_WSA

using System;
using Exact;
using Svanesjo.MRIoT.DataVisualizers;
using Svanesjo.MRIoT.Things.ColorRing;
using Svanesjo.MRIoT.Things.GameLogic;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things.Network
{
    [RequireComponent(typeof(Device))]
    [RequireComponent(typeof(NetworkedColorRing))]
    public class TileLampDevice : QRDataVisualizer, INetworkDevice
    {
        private TileLampNetwork? _tileLampNetwork;
        private Device _device = null!;
        private ExactManager _exactManager = null!;
        private DemoGameLogic _gameLogic = null!;

        protected new void Start()
        {
            base.Start();

            _device = GetComponent<Device>();
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
            if (_tileLampNetwork != null)
            {
                Debug.Log($"TileLampDevice setting lamp role: {role}");
                _tileLampNetwork.SetRole(role);
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
            if (_tileLampNetwork != null)
            {
                Debug.Log($"TileLampDevice setting lamp state: {state}");
                _tileLampNetwork.SetState(state);
            }
            else
            {
                Debug.LogWarning("TileLampDevice tileLampNetwork not found");
            }
        }

        public void SetNetworkThing(INetworkThing networkThing)
        {
            if (networkThing is not TileLampNetwork thing)
                throw new ArgumentException();

            // Unregister listener
            if (_tileLampNetwork != null)
            {
                _tileLampNetwork.LampStateChanged -= LampStateChanged;
                _tileLampNetwork.VirtualTapHappened -= VirtualTapHappened;
            }

            Debug.Log("TileLampDevice setting NetworkThing");
            _tileLampNetwork = thing;
            thing.LampStateChanged += LampStateChanged;
            thing.VirtualTapHappened += VirtualTapHappened;

            // TODO: Call an initialize-method instead? Encapsulation?
            GetComponent<NetworkedColorRing>().networkColorRing = thing.GetComponent<NetworkColorRing>();
        }
    }
}

#endif
