using System;
using Exact;
using Svanesjo.MRIoT.Things;
using UnityEngine;

namespace Svanesjo.MRIoT.DataVisualizers
{
    public class QRTileVisualizer : QRDataVisualizer
    {
        private Device _device;
        private ExactManager _exactManager;
        private Demo01 _game;

        // Start is called before the first frame update
        void Start()
        {
            if (qrCode == null)
            {
                throw new Exception("QR Code Empty");
            }

            GameObject exactManager = GameObject.Find("ExactManager");
            if (exactManager == null)
            {
                throw new Exception("Could not find ExactManager");
            }

            _exactManager = exactManager.GetComponent<ExactManager>();
            if (_exactManager == null)
            {
                throw new Exception("ExactManager is missing script ExactManager");
            }

            _game = exactManager.GetComponent<Demo01>();
            if (_game == null)
            {
                throw new Exception("Could not find game");
            }

            _device = GetComponent<Device>();
            _device.useDeviceName = true;
            _device.SetDeviceName(qrCode.Data);

            Debug.Log($"QRTileVisualizer adding device {_device.GetDeviceName()}");
            _exactManager.AddDevice(_device);
        }

        public void tapped()
        {
            Device device = GetComponent<Device>();
            Debug.Log($"RedTile3DVisualizer tapped on {device.GetDeviceName()}, calling {_game}");
            _game.OnTapped(device);
        }
    }
}
