#if UNITY_WSA

using System;
using Microsoft.MixedReality.OpenXR;
using Svanesjo.MRIoT.DataVisualizers;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.QRCodes
{
    [RequireComponent(typeof(QRDataVisualizer))]
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private Guid _id;
        private SpatialGraphNode? _node;
        private QRDataVisualizer _dataVisualizer = null!;

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
            {
                throw new Exception("QR Data Visualizer not found");
            }

            InitializeSpatialGraphNode();
        }

        // Update is called once per frame
        void Update()
        {
            if (_node == null || _node.Id != Id)
            {
                InitializeSpatialGraphNode();
            }

            if (_node != null)
            {
                if (_node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    Vector3 position = new Vector3(pose.position.x + xCorrection, pose.position.y + yCorrection, pose.position.z + zCorrection);
                    gameObject.transform.SetPositionAndRotation(position, pose.rotation);
                    // Call on QRDataVisualizer to update transform for NetworkObject
                    _dataVisualizer.SetPositionAndRotation(position, pose.rotation);
                    // Debug.Log("Tracker : Id= " + Id + " QRPose =" + position.ToString("F7") + " QRRot = " +
                    //           pose.rotation.ToString("F7"));
                }
            }
        }

        private void InitializeSpatialGraphNode(bool force = false)
        {
            if (_node == null || force)
            {
                _node = Id != Guid.Empty ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                Debug.Log("Initialize SpatialGraphNode Id= " + Id);
            }
        }
    }
}

#endif
