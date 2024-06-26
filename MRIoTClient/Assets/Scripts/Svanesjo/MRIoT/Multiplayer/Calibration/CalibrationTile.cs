﻿#nullable enable

using System;
using NaughtyAttributes;
using Svanesjo.MRIoT.Utility;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.Multiplayer.Calibration
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class CalibrationTile : NetworkBehaviour
    {
        [SerializeField, Required] private TextMeshPro text = null!;

        public Action? OnMoved;
        private NetworkObject _networkObject = null!;
        private readonly NetworkVariable<int> _id = new();

        private ILogger _logger = new DebugLogger(typeof(CalibrationTile));

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            if (_networkObject == null)
                throw new MissingComponentException(nameof(NetworkObject));
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            _id.OnValueChanged += OnIdChanged;
        }

        private void Start()
        {
            Initialize();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Initialize();
        }

        private void Initialize()
        {
            text.text = _id.Value.ToString();

            if (!IsServer) return;

            // Disable interaction on the server, since it will use QR Codes instead
            GetComponent<XRGrabInteractable>().enabled = false;

            // Register this CalibrationTile with CalibrationOrigin if it exists
            var origin = FindFirstObjectByType<CalibrationOrigin>();
            if (origin != null)
                origin.RegisterCalibrationTile(this);
        }

        private void Update()
        {
            // TODO: check if the tile is currently selected (only call OnMoved on selection exit?)
            if (!transform.hasChanged) return;
            OnMoved?.Invoke();
            transform.hasChanged = false;
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            _logger.Log("SetPositionAndRotation called");
            transform.SetPositionAndRotation(position, rotation);
        }

        public void SetId(int id)
        {
            _logger.Log($"SetId {_id.Value} -> {id}");
            _id.Value = id;
            text.text = id.ToString();
        }

        private void OnIdChanged(int previousValue, int newValue)
        {
            _logger.Log($"OnIdChanged {previousValue} -> {newValue}");
            text.text = newValue.ToString();
        }

        public override string ToString()
        {
            return $"{gameObject.name}: {_networkObject.NetworkObjectId}";
        }
    }
}
