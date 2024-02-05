#if UNITY_WSA

using System;
using Exact;
using Exact.Example;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.DataVisualizers
{
    public class RedTile3DVisualizer : QRDataVisualizer
    {
        private Device _device = null!;
        private ExactManager _exactManager = null!;
        private FollowTheRedDot _game = null!;
        
        // Start is called before the first frame update
        protected new void Start()
        {
            base.Start();
            if (Code == null)
                throw new Exception("QR Code Empty");

            _exactManager = FindFirstObjectByType<ExactManager>();
            if (_exactManager == null)
            {
                throw new Exception("Component ExactManager not found");
            }

            _game = _exactManager.GetComponent<FollowTheRedDot>();
            if (_game == null)
            {
                throw new Exception("Component FollowTheRedDot not found");
            }

            _device = GetComponent<Device>();
            if (_device is null)
            {
                throw new Exception("Component Device not found");
            }
            _device.useDeviceName = true;
            _device.SetDeviceName(Code.Data);

            Debug.Log($"RedTile3DVisualizer adding device {_device.GetDeviceName()}");
            _exactManager.AddDevice(_device);
        }

        public void Tapped()
        {
            Device device = GetComponent<Device>();
            Debug.Log($"RedTile3DVisualizer tapped on {device.GetDeviceName()}, calling {_game}");
            _game.OnTapped(device);
        }
    }
}

#endif
