using System;
using Microsoft.MixedReality.OpenXR;
using UnityEngine;

namespace Svanesjo.MRIoT
{
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private Guid _id;
        private SpatialGraphNode node;

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
                    // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                    // to these objects so apply the inverse
                    // TODO: CameraCache from MRTK v2:
                    // => probably not necessary (no teleportation)


                    Vector3 position = new Vector3(pose.position.x, pose.position.y + 1.5f, pose.position.z);
                    gameObject.transform.SetPositionAndRotation(position, pose.rotation);
                    Debug.Log("Tracker : Id= " + Id + " QRPose =" + position.ToString("F7") + " QRRot = " +
                              pose.rotation.ToString("F7"));
                }
                else
                {
                    Debug.LogWarning("Cannot locate " + Id);
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
