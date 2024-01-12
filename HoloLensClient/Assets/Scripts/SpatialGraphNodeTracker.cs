using System;
using Microsoft.MixedReality.OpenXR;
using Svanesjo.MRIoT.DataVisualizers;
using UnityEngine;

namespace Svanesjo.MRIoT
{
    [RequireComponent(typeof(QRDataVisualizer))]
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private Guid _id;
        private SpatialGraphNode node;

        public float xCorrection;
        public float yCorrection = 1.6f;
        public float zCorrection;

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
            InitializeSpatialGraphNode();
        }

        // Update is called once per frame
        void Update()
        {
            if (node == null || node.Id != Id)
            {
                InitializeSpatialGraphNode();
            }

            if (node != null)
            {
                if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    Vector3 position = new Vector3(pose.position.x + xCorrection, pose.position.y + yCorrection, pose.position.z + zCorrection);
                    gameObject.transform.SetPositionAndRotation(position, pose.rotation);
                    Debug.Log("Tracker : Id= " + Id + " QRPose =" + position.ToString("F7") + " QRRot = " +
                              pose.rotation.ToString("F7"));
                }
            }
        }

        private void InitializeSpatialGraphNode(bool force = false)
        {
            if (node == null || force)
            {
                node = Id != Guid.Empty ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                Debug.Log("Initialize SpatialGraphNode Id= " + Id);
            }
        }
    }
}
