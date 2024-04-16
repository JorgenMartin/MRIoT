#nullable enable

#if UNITY_WSA

using System;
using Microsoft.MixedReality.OpenXR;
using Svanesjo.MRIoT.QRCodes.DataVisualizers;
using Svanesjo.MRIoT.Utility;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.QRCodes
{
    [RequireComponent(typeof(QRDataVisualizer))]
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private Guid _id;
        private SpatialGraphNode? _node;
        private QRDataVisualizer _dataVisualizer = null!;
        private ILogger _logger = new DebugLogger(typeof(SpatialGraphNodeTracker));
        private bool _firstLog = true;

        [SerializeField] private float xCorrection; // = 0f
        [SerializeField] private float yCorrection = 1.6f;
        [SerializeField] private float zCorrection; // = 0f

        public Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    InitializeSpatialGraphNode(force: true);
                }
            }
        }

        // Initialize
        private void Start()
        {
            _dataVisualizer = GetComponent<QRDataVisualizer>();
            if (_dataVisualizer is null)
                throw new Exception("QR Data Visualizer not found");

            var qrLogger = QRCodesManager.Instance.Logger;
            if (qrLogger is FileLogger fileLogger)
                _logger = fileLogger.CreateSubLogger(typeof(SpatialGraphNodeTracker));

            InitializeSpatialGraphNode();
        }

        // Update is called once per frame
        void Update()
        {
            if (_node == null || _node.Id != Id)
            {
                InitializeSpatialGraphNode();
            }

            if (_node == null || !_node.TryLocate(FrameTime.OnUpdate, out Pose pose)) return;

            var newPos = new Vector3(pose.position.x + xCorrection,
                pose.position.y + yCorrection,
                pose.position.z + zCorrection);

            var gameObjectTransform = gameObject.transform;
            var oldPos = gameObjectTransform.position;
            var oldRot = gameObjectTransform.rotation;
            var distance = newPos.DistanceFrom(oldPos);
            var postDelta = newPos.DifferanceFrom(oldPos);
            var rotDelta = pose.rotation.DifferenceFrom(oldRot);

            gameObject.transform.SetPositionAndRotation(newPos, pose.rotation);
            // Call on QRDataVisualizer to update transform for NetworkObject
            _dataVisualizer.SetPositionAndRotation(newPos, pose.rotation);

            if (_firstLog)
            {
                _firstLog = false;
                _logger.Log("Id; Position; Rotation; Distance; PositionDelta; RotationDelta");
            }
            _logger.Log($"{Id}; {newPos.ToString("F7")}; {pose.rotation.ToString("F7")}; {distance}; {postDelta.ToString("F7")}; {rotDelta.ToString("F7")}");
        }

        private void InitializeSpatialGraphNode(bool force = false)
        {
            if (_node == null || force)
            {
                _node = Id != Guid.Empty ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                _logger.Log("Initialize SpatialGraphNode Id= " + Id);
            }
        }
    }
}

#endif
