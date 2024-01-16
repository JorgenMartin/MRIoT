using System;
using Exact;
using Exact.Example;
using Svanesjo.MRIoT.DataVisualizers;
using UnityEngine;

namespace Svanesjo.MRIoT
{

    public class RedTile3DVisualizer : QRDataVisualizer
    {
        private Device _device;
        private ExactManager _exactManager;
        private FollowTheRedDot _game;
        
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

            _game = exactManager.GetComponent<FollowTheRedDot>();
            if (_game == null)
            {
                throw new Exception("Could not find game");
            }

            _device = GetComponent<Device>();
            _device.useDeviceName = true;
            _device.SetDeviceName(qrCode.Data);

            Debug.Log($"RedTile3DVisualizer adding device {_device.GetDeviceName()}");
            _exactManager.AddDevice(_device);
            
            // TODO: unnecessary?
            // _game.AddedNewDevice();
        }

        public void tapped()
        {
            Device device = GetComponent<Device>();
            FollowTheRedDot game = _exactManager.GetComponent<FollowTheRedDot>();
            Debug.Log($"RedTile3DVisualizer tapped on {device.GetDeviceName()}, calling {game}");
            game.OnTapped(device);
        }
    }
}
