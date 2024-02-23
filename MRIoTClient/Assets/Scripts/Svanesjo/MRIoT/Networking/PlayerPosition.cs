using System;
using Svanesjo.MRIoT.Things.Calibration;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    [RequireComponent(typeof(NetworkObject))]
    class PlayerPosition : NetworkBehaviour
    {
        [SerializeField] private Vector3 offset = Vector3.zero;

        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera is null)
            {
                throw new Exception("Camera not found");
            }

            if (!IsServer) return;

            var calibrationOrigin = FindFirstObjectByType<CalibrationOrigin>();
            if (calibrationOrigin != null)
                calibrationOrigin.ReParentIfValid(GetComponent<NetworkObject>());
        }

        private void Update()
        {
            var mainCameraTransform = _mainCamera.transform;
            gameObject.transform.SetPositionAndRotation(mainCameraTransform.position + offset,
                mainCameraTransform.rotation);
        }
    }
}
