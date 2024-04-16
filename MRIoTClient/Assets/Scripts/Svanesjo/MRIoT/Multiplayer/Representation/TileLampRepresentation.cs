#nullable enable

using System;
using NaughtyAttributes;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Svanesjo.MRIoT.Multiplayer.Representation
{
    public class LampEventArgs : EventArgs
    {
        public TileLampRole Role { get; private set; }
        public TileLampState State { get; private set; }

        public LampEventArgs(TileLampRole role, TileLampState state)
        {
            Role = role;
            State = state;
        }
    }

    // TODO: Implement INetworkColorRing
    [RequireComponent(typeof(ColorRingRepresentation))]
    public class TileLampRepresentation : NetworkBehaviour, IMultiplayerRepresentation
    {
        [Required, SerializeField] private GameObject button = null!;
        [Required, SerializeField] private GameObject lamp = null!;

        private readonly NetworkVariable<TileLampRole> _role = new();
        private readonly NetworkVariable<TileLampState> _state = new();

        public UnityEvent lampOn = null!;
        public UnityEvent lampOff = null!;
        public UnityEvent lampDisconnected = null!;
        public EventHandler<LampEventArgs>? LampStateChanged;
        public EventHandler<EmptyEventArgs>? VirtualTapHappened;

        private void Awake()
        {
            if (button == null || lamp == null)
                throw new ArgumentNullException();
        }

        private void Start()
        {
            Debug.Log($"TileLampNetwork starting with role {_role.Value} and state {_state.Value}");
            UpdateView(_role.Value, _state.Value);
        }

        public override void OnNetworkSpawn()
        {
            _role.OnValueChanged += OnRoleChanged;
            _state.OnValueChanged += OnStateChanged;
            UpdateView(_role.Value, _state.Value);
        }

        private void OnRoleChanged(TileLampRole previousValue, TileLampRole newValue)
        {
            Debug.Log($"TileLampNetwork role changed to {newValue}, with state {_state.Value}");
            UpdateView(newValue, _state.Value);
        }

        private void OnStateChanged(TileLampState previousValue, TileLampState newValue)
        {
            Debug.Log($"TileLampNetwork state changed to {newValue}, with role {_role.Value}");
            LampStateChanged?.Invoke(this, new LampEventArgs(_role.Value, newValue));
            UpdateView(_role.Value, newValue);
        }

        private void UpdateView(TileLampRole role, TileLampState state)
        {
            Debug.Log($"TileLampNetwork updating view with role {role} and state {state}");
            switch (role)
            {
                case TileLampRole.Undefined:
                    button.SetActive(false);
                    lamp.SetActive(false);
                    break;
                case TileLampRole.Lamp:
                    button.SetActive(false);
                    lamp.SetActive(true);
                    break;
                case TileLampRole.On:
                case TileLampRole.Off:
                case TileLampRole.Toggle:
                    button.SetActive(true);
                    lamp.SetActive(false);
                    break;
                case TileLampRole.ToggleLamp:
                    button.SetActive(true);
                    lamp.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
            switch (state)
            {
                case TileLampState.Undefined:
                    lampDisconnected.Invoke();
                    break;
                case TileLampState.On:
                    lampOn.Invoke();
                    break;
                case TileLampState.Off:
                    lampOff.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void SetRole(TileLampRole role)
        {
            if (IsServer)
            {
                Debug.Log($"TileLampNetwork server setting role: {role}");
                _role.Value = role;
            }
            else
            {
                Debug.Log($"TileLampNetwork calling serverRPC to set role: {role}");
                SetRoleServerRpc(role);
            }
        }

        [ServerRpc]
        private void SetRoleServerRpc(TileLampRole role)
        {
            Debug.Log($"TileLampNetwork serverRPC setting role: {role}");
            SetRole(role);
        }

        public void SetState(TileLampState state)
        {
            if (IsServer)
            {
                Debug.Log($"TileLampNetwork server setting state: {state}");
                _state.Value = state;
            }
            else
            {
                Debug.Log($"TileLampNetwork calling serverRPC to set state: {state}");
                SetStateServerRpc(state);
            }
        }

        [ServerRpc]
        private void SetStateServerRpc(TileLampState state)
        {
            Debug.Log($"TileLampNetwork serverRPC setting state: {state}");
            SetState(state);
        }

        public void VirtualTap()
        {
            if (IsServer)
            {
                Debug.Log("TileLampNetwork server invoking VirtualTap");
                VirtualTapHappened?.Invoke(this, new EmptyEventArgs());
            }
            else
            {
                Debug.Log("TileLampNetwork calling serverRPC VirtualTap");
                VirtualTapServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void VirtualTapServerRpc()
        {
            Debug.Log("TileLampNetwork serverRPC VirtualTap");
            VirtualTap();
        }
    }
}
