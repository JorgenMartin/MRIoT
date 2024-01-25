using System;
using Exact;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things
{

    [RequireComponent(typeof(Device))]
    public class QRTileLogic : MonoBehaviour
    {
        private Device _device = null!;

        private void Awake()
        {
            _device = GetComponent<Device>();
            if (_device is null)
            {
                throw new Exception("Component Device not found");
            }
        }

        public void VirtualTap()
        {
            Debug.Log("QRTileLogic virtual tap registered");
            Demo01 game = FindFirstObjectByType<Demo01>();
            game.OnTapped(_device);
        }
    }
}
