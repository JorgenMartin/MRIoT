﻿#nullable enable

#if UNITY_WSA

using System;
using System.Collections;
using Exact;
using Exact.Example;
using Svanesjo.MRIoT.Multiplayer.Device;
using Svanesjo.MRIoT.Multiplayer.Representation;
using Svanesjo.MRIoT.Utility;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.GameLogic
{
    public class LampDemo : DemoGameLogic
    {
        [SerializeField] private int minimumConnectedBeforeStart = 1;
        [SerializeField] private float lightIntensity = 0.5f;
        [SerializeField] private Color lampColor = Color.magenta;
        [SerializeField] private Color onColor = Color.green;
        [SerializeField] private Color offColor = Color.red;
        [SerializeField] private Color toggleColor = Color.yellow;

        private ExactManager _exactManager = null!;
        private Device[] _devices = {};
        private bool _lampOn; // = false;
        private ILogger _logger = new DebugLogger(typeof(LampDemo));

        private void Awake()
        {
            _exactManager = GetComponent<ExactManager>();
            if (_exactManager == null)
                throw new ArgumentNullException();
        }

        private void Start()
        {
            StartCoroutine(Startup());
        }

        private IEnumerator Startup()
        {
            _logger.Log($"Startup waiting for {minimumConnectedBeforeStart} devices to connect");
            while (true)
            {
                // yield return null;
                yield return new WaitForSeconds(5);
                var devices = _exactManager.GetConnectedDevices();
                var count = devices.Count;
                _logger.Log($"Startup: {count} connected, {minimumConnectedBeforeStart} required");
                if (count >= minimumConnectedBeforeStart)
                {
                    Restart();
                    break;
                }
            }
            _logger.Log("Startup complete");
        }

        private void Restart()
        {
            _logger.Log("running restart");
            StopAllCoroutines();

            var devices = _exactManager.GetConnectedDevices();

            for (int i = 1; i < devices.Count; i++)
            {
                TurnOffLight(devices[i]);
            }

            if (devices.Count < minimumConnectedBeforeStart)
            {
                _logger.Log("Not enough devices to start, returning to startup and waiting for additional devices");
                _lampOn = false;
                _devices = new Device[] { };
                StartCoroutine(Startup());
                return;
            }

            _lampOn = true;
            int count = devices.Count;
            switch (count)
            {
                case >= 3:
                    _devices = new[] { devices[0], devices[1], devices[2] };
                    SetDeviceRoleAndColor(_devices[0], TileLampRole.Lamp, lampColor);
                    SetDeviceRoleAndColor(_devices[1], TileLampRole.On, onColor);
                    SetDeviceRoleAndColor(_devices[2], TileLampRole.Off, offColor);
                    break;
                case 2:
                    _devices = new[] { devices[0], devices[1] };
                    SetDeviceRoleAndColor(_devices[0], TileLampRole.Lamp, lampColor);
                    SetDeviceRoleAndColor(_devices[1], TileLampRole.Toggle, toggleColor);
                    break;
                case 1:
                    _devices = new[] { devices[0] };
                    SetDeviceRoleAndColor(_devices[0], TileLampRole.ToggleLamp, lampColor);
                    break;
                default:
                    throw new Exception($"Unsupported minimumConnectedBeforeStart: {devices.Count} connected, {minimumConnectedBeforeStart} required");
            }

            StartCoroutine(CheckConnections(count));
        }

        private void SetDeviceRoleAndColor(Device device, TileLampRole tileLampRole, Color color)
        {
            device.GetComponent<TileLampDevice>().SetLampRole(tileLampRole);
            device.GetComponent<LedRing>().SetColorAndIntensity(color, lightIntensity);
        }

        public override void OnTapped(Device device)
        {
            var logic = device.GetComponent<TileLampDevice>();
            if (logic is null)
            {
                _logger.LogWarning($"Logic component not found on device '{device}'");
                return;
            }
            // TODO: Set lamp state in network
            switch (_devices.Length)
            {
                case 0:
                    _logger.Log("No devices registered...");
                    return;
                case 1:
                    if (device == _devices[0])
                        ToggleLamp();
                    break;
                case 2:
                    if (device == _devices[1])
                        ToggleLamp();
                    break;
                case 3:
                    if (device == _devices[1])
                        TurnOnLight(_devices[0], lampColor, lightIntensity);
                    else if (device == _devices[2])
                        TurnOffLight(_devices[0]);
                    break;
            }
        }

        private void ToggleLamp()
        {
            _logger.Log($"toggle lamp '{_devices[0]}'");
            if (_lampOn)
            {
                TurnOffLight(_devices[0]);
            }
            else
            {
                TurnOnLight(_devices[0], lampColor, lightIntensity);
            }
        }

        private void TurnOnLight(Device device, Color color, float intensity)
        {
            _logger.Log($"turn on light '{device}' ({color}, {intensity})");
            if (device == _devices[0])
            {
                _lampOn = true;
            }
            device.GetComponent<LedRing>().SetColorAndIntensity(color, intensity);
        }

        private void TurnOffLight(Device device)
        {
            _logger.Log($"turn off light '{device}'");
            if (_devices.Length > 0 && device == _devices[0])
            {
                _lampOn = false;
            }
            var ledRing = device.GetComponent<LedRing>();
            ledRing.StopFading();
            ledRing.SetColor(Color.black);
        }

        private IEnumerator CheckConnections(int connectedAtRestart)
        {
            while (true)
            {
                var connectedNow = _exactManager.GetConnectedDevices()!;
                int count = connectedNow.Count;
                _logger.Log($"CheckConnections: {count} currently connected, was {connectedAtRestart} at last restart");
                if (count < minimumConnectedBeforeStart || count > connectedAtRestart)
                {
                    Restart();
                    break;
                }

                yield return new WaitForSeconds(10);
            }
        }
    }
}

#endif
