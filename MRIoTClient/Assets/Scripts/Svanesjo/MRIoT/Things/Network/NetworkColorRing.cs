#nullable enable

using System;
using Exact.Example;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Things.Network
{
    public class NetworkColorRing : NetworkBehaviour
    {
        private readonly NetworkVariable<Color> _color = new(Color.black);
        [Required, SerializeField] private ColorRingBase colorRing = null!;

        private void Awake()
        {
            if (colorRing == null)
                throw new ArgumentNullException();

            _color.OnValueChanged += OnColorChanged;
        }

        private void Start()
        {
            Debug.Log($"NetworkColorRing starting with color {_color.Value}");
            UpdateView(_color.Value);
        }

        public override void OnNetworkSpawn()
        {
            UpdateView(_color.Value);
        }

        private void OnColorChanged(Color previousValue, Color newValue)
        {
            Debug.Log($"NetworkColorRing changed color to {_color.Value}");
            UpdateView(newValue);
        }

        private void UpdateView(Color color)
        {
            Debug.Log($"NetworkColorRing updating view with color {color}");
            colorRing.SetUniformColor(color);
        }

        public void SetUniformColor(Color color)
        {
            _color.Value = color;
        }
    }
}
