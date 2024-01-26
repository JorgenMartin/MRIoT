using System;
using Exact;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things
{

    [RequireComponent(typeof(Device))]
    public class QRTileCylinderLogic : MonoBehaviour
    {
        private Device _device = null!;
        private DemoGame _game = null!;

        private void Awake()
        {
            _device = GetComponent<Device>();
            if (_device is null)
            {
                throw new Exception("Component Device not found");
            }

            _game = FindFirstObjectByType<DemoGame>();
            if (_game is null)
            {
                throw new Exception("Component DemoGame not found");
            }
        }

        public void VirtualTap()
        {
            Debug.Log("QRTileLogic virtual tap registered");
            _game.OnTapped(_device);
        }
    }
}
