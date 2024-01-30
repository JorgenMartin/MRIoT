using System;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    class PlayerPosition : NetworkBehaviour
    {
        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera is null)
            {
                throw new Exception("Camera not found");
            }
        }

        private void Update()
        {
            var mainCameraTransform = _mainCamera.transform;
            gameObject.transform.SetPositionAndRotation(mainCameraTransform.position, mainCameraTransform.rotation);
        }
    }
}
