using System;
using Exact;
using Svanesjo.MRIoT.Things;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.DataVisualizers
{
    public class QRTileVisualizer : QRDataVisualizer
    {
        private Device _device = null!;
        private ExactManager _exactManager = null!;
        private DemoGame _game = null!;

        // Start is called before the first frame update
        private void Start()
        {
            if (Code == null)
            {
                throw new Exception("QR Code Empty");
            }

            _exactManager = FindFirstObjectByType<ExactManager>();
            if (_exactManager == null)
            {
                throw new Exception("Component ExactManager not found");
            }

            _game = _exactManager.GetComponent<DemoGame>();
            if (_game == null)
            {
                throw new Exception("Component DemoGame not found");
            }

            _device = GetComponent<Device>();
            if (_device is null)
            {
                throw new Exception("Component Device not found");
            }
            _device.useDeviceName = true;
            _device.SetDeviceName(Code.Data);

            Debug.Log($"QRTileVisualizer adding device {_device.GetDeviceName()}");
            _exactManager.AddDevice(_device);
        }

        public void Tapped()
        {
            Device device = GetComponent<Device>();
            Debug.Log($"QRTileVisualizer tapped on {device.GetDeviceName()}, calling {_game}");
            _game.OnTapped(device);
        }
    }
}
