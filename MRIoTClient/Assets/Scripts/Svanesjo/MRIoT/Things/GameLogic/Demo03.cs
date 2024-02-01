﻿#nullable enable

#if UNITY_WSA

using System;
using System.Collections;
using Exact;
using Exact.Example;
using Svanesjo.MRIoT.Things.Network;
using UnityEngine;

namespace Svanesjo.MRIoT.Things.GameLogic
{
    public class Demo03 : DemoGame
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
            Debug.Log($"Demo03 Startup waiting for {minimumConnectedBeforeStart} devices to connect");
            while (true)
            {
                // yield return null;
                yield return new WaitForSeconds(5);
                var devices = _exactManager.GetConnectedDevices();
                var count = devices.Count;
                Debug.Log($"Demo03 Startup: {count} connected, {minimumConnectedBeforeStart} required");
                if (count >= minimumConnectedBeforeStart)
                {
                    Restart();
                    break;
                }
            }
            Debug.Log("Demo03 Startup complete");
        }

        private void Restart()
        {
            Debug.Log("Demo03 running restart");
            StopAllCoroutines();

            var devices = _exactManager.GetConnectedDevices();

            for (int i = 1; i < devices.Count; i++)
            {
                TurnOffLight(devices[i]);
            }

            if (devices.Count < minimumConnectedBeforeStart)
            {
                Debug.Log("Demo03 Not enough devices to start, returning to startup and waiting for additional devices");
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
                    throw new Exception($"Demo03 Unsupported minimumConnectedBeforeStart: {devices.Count} connected, {minimumConnectedBeforeStart} required");
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
                Debug.LogWarning($"Demo03 Logic component not found on device '{device}'");
                return;
            }
            // TODO: Set lamp state in network
            switch (_devices.Length)
            {
                case 0:
                    Debug.Log("Demo03 No devices registered...");
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
            Debug.Log($"Demo03 toggle lamp '{_devices[0]}'");
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
            Debug.Log($"Demo03 turn on light '{device}' ({color}, {intensity})");
            if (device == _devices[0])
            {
                _lampOn = true;
            }
            device.GetComponent<LedRing>().SetColorAndIntensity(color, intensity);
        }

        private void TurnOffLight(Device device)
        {
            Debug.Log($"Demo03 turn off light '{device}'");
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
                Debug.Log($"Demo03 CheckConnections: {count} currently connected, was {connectedAtRestart} at last restart");
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
