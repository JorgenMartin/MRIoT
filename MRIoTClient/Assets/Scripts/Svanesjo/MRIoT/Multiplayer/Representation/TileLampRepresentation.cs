#nullable enable

using System;
using NaughtyAttributes;
using Svanesjo.MRIoT.Utility;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

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

        private ILogger _logger = new DebugLogger(typeof(TileLampRepresentation));

        private void Awake()
        {
            if (button == null || lamp == null)
                throw new ArgumentNullException();
        }

        private void Start()
        {
            _logger.Log($"starting with role {_role.Value} and state {_state.Value}");
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
            _logger.Log($"role changed to {newValue}, with state {_state.Value}");
            UpdateView(newValue, _state.Value);
        }

        private void OnStateChanged(TileLampState previousValue, TileLampState newValue)
        {
            _logger.Log($"state changed to {newValue}, with role {_role.Value}");
            LampStateChanged?.Invoke(this, new LampEventArgs(_role.Value, newValue));
            UpdateView(_role.Value, newValue);
        }

        private void UpdateView(TileLampRole role, TileLampState state)
        {
            _logger.Log($"updating view with role {role} and state {state}");
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
                _logger.Log($"server setting role: {role}");
                _role.Value = role;
            }
            else
            {
                _logger.Log($"calling serverRPC to set role: {role}");
                SetRoleServerRpc(role);
            }
        }

        [ServerRpc]
        private void SetRoleServerRpc(TileLampRole role)
        {
            _logger.Log($"serverRPC setting role: {role}");
            SetRole(role);
        }

        public void SetState(TileLampState state)
        {
            if (IsServer)
            {
                _logger.Log($"server setting state: {state}");
                _state.Value = state;
            }
            else
            {
                _logger.Log($"calling serverRPC to set state: {state}");
                SetStateServerRpc(state);
            }
        }

        [ServerRpc]
        private void SetStateServerRpc(TileLampState state)
        {
            _logger.Log($"serverRPC setting state: {state}");
            SetState(state);
        }

        public void VirtualTap()
        {
            if (IsServer)
            {
                _logger.Log("server invoking VirtualTap");
                VirtualTapHappened?.Invoke(this, new EmptyEventArgs());
            }
            else
            {
                _logger.Log("calling serverRPC VirtualTap");
                VirtualTapServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void VirtualTapServerRpc()
        {
            _logger.Log("serverRPC VirtualTap");
            VirtualTap();
        }
    }
}
