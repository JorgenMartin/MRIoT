using System;
using Exact;
using Exact.Example;
using UnityEngine;
using System.Collections;

#nullable enable

namespace Svanesjo.MRIoT.Things
{
    [RequireComponent(typeof(ExactManager))]
    public class Demo01 : DemoGame
    {
        [SerializeField] private int minimumConnectedBeforeStart = 1;
        [SerializeField] private float intensity = 0.5f;
        [SerializeField] private Color[] colors = {};
        [SerializeField] private int colorIndex = 0;

        private ExactManager _exactManager = null!;
        private Device? _active;

        private void Start()
        {
            if (colorIndex >= colors.Length)
            {
                throw new Exception($"colorIndex {colorIndex} out of bounds for color list of length {colors.Length}");
            }
            _exactManager = GetComponent<ExactManager>();
            if (_exactManager is null)
            {
                throw new Exception("Component ExactManager not found");
            }
            StartCoroutine(Startup());
        }

        IEnumerator Startup()
        {
            while (true)
            {
                yield return null;
                var devices = _exactManager.GetConnectedDevices();
                if (devices.Count >= minimumConnectedBeforeStart)
                {
                    Restart();
                    break;
                }
            }
            Debug.Log("Startup complete");
        }

        void Restart()
        {
            StopAllCoroutines();

            var devices = _exactManager.GetConnectedDevices();

            for (int i = 1; i < devices.Count; i++)
            {
                var led = devices[i].GetComponent<LedRing>();
                led.StopFading();
                led.SetColor(Color.black);
            }

            _active = devices[0];
            _active.GetComponent<LedRing>().SetColorAndIntensity(NextColor(), intensity);
        }

        private Color NextColor()
        {
            var index = colorIndex;
            if (++colorIndex >= colors.Length)
            {
                colorIndex = 0;
            }

            return colors[index];
        }

        public override void OnTapped(Device device)
        {
            SetNewColor();
        }

        private void SetNewColor()
        {
            StopAllCoroutines();
            if (_active is not null && _active.isActiveAndEnabled) // TODO
            {
                _active.GetComponent<LedRing>().SetColorAndIntensity(NextColor(), intensity);
            }
        }
    }
}
