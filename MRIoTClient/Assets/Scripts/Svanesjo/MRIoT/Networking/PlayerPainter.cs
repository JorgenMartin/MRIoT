#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    [Serializable]
    public enum PlayerType
    {
        Server,
        Host,
        Client,
    }

    [Serializable]
    public class PlayerTypeMaterial
    {
        public PlayerType playerType;
        public Material? material;
    }

    public class PlayerPainter : NetworkBehaviour
    {
        [SerializeField, Required] private Material defaultMaterial = null!;
        [SerializeField] private List<PlayerTypeMaterial> playerTypeMaterials = new();

        private readonly NetworkVariable<PlayerType> _playerType = new(writePerm: NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            _playerType.OnValueChanged += OnPlayerTypeChanged;
        }

        private void Start()
        {
            Initialize();
        }

        public override void OnNetworkSpawn()
        {
            Initialize();
            base.OnNetworkSpawn();
        }

        private void Initialize()
        {
            if (IsOwner)
                _playerType.Value = IsHost ? PlayerType.Host : IsServer ? PlayerType.Server : PlayerType.Client;

            PaintPlayer();
        }

        private void OnPlayerTypeChanged(PlayerType previousValue, PlayerType newValue)
        {
            PaintPlayer(newValue);
        }

        private void PaintPlayer(PlayerType? type = null)
        {
            var useType = type ?? _playerType.Value;
            var material = playerTypeMaterials
                               .Where(e => e.playerType == useType)
                               .Select(e => e.material)
                               .FirstOrDefault()
                           ?? defaultMaterial;

            foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
                mesh.material = material;
        }
    }
}
