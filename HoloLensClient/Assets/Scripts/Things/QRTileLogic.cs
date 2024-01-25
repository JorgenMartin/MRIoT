using Exact;
using UnityEngine;

namespace Svanesjo.MRIoT.Things
{

    [RequireComponent(typeof(Device))]
    public class QRTileLogic : MonoBehaviour
    {
        private Device _device;

        private void Awake()
        {
            _device = GetComponent<Device>();
        }

        public void VirtualTap()
        {
            Debug.Log("QRTileLogic virtual tap registered");
            Demo01 game = FindFirstObjectByType<Demo01>();
            game.OnTapped(_device);
        }
    }
}
