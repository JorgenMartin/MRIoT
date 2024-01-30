using System;
using Exact;
using NaughtyAttributes;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things
{
    public enum Role
    {
        Lamp,
        On,
        Off,
        Toggle,
        ToggleLamp
    }

    [RequireComponent(typeof(Device))]
    public class QRTileLampLogic : MonoBehaviour
    {
        [Required, SerializeField] private GameObject button = null!;
        [Required, SerializeField] private GameObject lamp = null!;

        private Device _device = null!;
        private DemoGame _game = null!;
        private Role? _role; // = null;
        private Role? _newRole; // = null;

        private void Awake()
        {
            if (button is null || lamp is null)
            {
                throw new Exception("Required field is null");
            }

            _device = GetComponent<Device>();
            if (_device is null)
            {
                throw new Exception("Component Device not found");
            }

            _game = FindFirstObjectByType<DemoGame>();
            if (_game is null)
            {
                throw new Exception("Component DemoGame not found");
            }
        }

        public void VirtualTap()
        {
            Debug.Log("QRTileLogic virtual tap registered");
            _game.OnTapped(_device);
        }

        public void SetRole(Role role)
        {
            _newRole = role;
        }

        private void Update()
        {
            if (_role == _newRole)
            {
                return;
            }

            switch (_newRole)
            {
                case Role.Lamp:
                    button.SetActive(false);
                    lamp.SetActive(true);
                    break;
                case Role.On:
                case Role.Off:
                case Role.Toggle:
                    button.SetActive(true);
                    lamp.SetActive(false);
                    break;
                case Role.ToggleLamp:
                    button.SetActive(true);
                    lamp.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _role = _newRole;
        }
    }
}
