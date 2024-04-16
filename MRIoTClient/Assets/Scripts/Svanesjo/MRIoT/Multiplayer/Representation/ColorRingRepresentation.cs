#nullable enable

using System;
using Exact.Example;
using NaughtyAttributes;
using Svanesjo.MRIoT.Utility;
using Unity.Netcode;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.Multiplayer.Representation
{
    public class ColorRingRepresentation : NetworkBehaviour
    {
        private readonly NetworkVariable<Color> _color = new(Color.black);
        [Required, SerializeField] private ColorRingBase colorRing = null!;

        private ILogger _logger = new DebugLogger(typeof(ColorRingRepresentation));

        private void Awake()
        {
            if (colorRing == null)
                throw new ArgumentNullException();

            _color.OnValueChanged += OnColorChanged;
        }

        private void Start()
        {
            _logger.Log($"starting with color {_color.Value}");
            UpdateView(_color.Value);
        }

        public override void OnNetworkSpawn()
        {
            UpdateView(_color.Value);
        }

        private void OnColorChanged(Color previousValue, Color newValue)
        {
            _logger.Log($"changed color to {_color.Value}");
            UpdateView(newValue);
        }

        private void UpdateView(Color color)
        {
            _logger.Log($"updating view with color {color}");
            colorRing.SetUniformColor(color);
        }

        public void SetUniformColor(Color color)
        {
            _color.Value = color;
        }
    }
}
