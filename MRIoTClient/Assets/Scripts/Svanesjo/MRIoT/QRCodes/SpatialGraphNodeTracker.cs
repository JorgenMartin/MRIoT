#if UNITY_WSA

using System;
using System.IO;
using System.Text;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.QR;
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

        private void DebugLog(string message)
        {
            Debug.Log(message);
            LogStr($"[DEBUG] {message}");
        }

        private void LogStr(string message)
        {
            if (!QRCodesManager.Instance.runningEvaluation)
                return;

            var filePath = Path.Combine(Application.persistentDataPath, "MRIoTSpatialGraphNodeTracker.log");
            if (!File.Exists(filePath))
                Debug.LogError($"Creating log file: {filePath}");
            using var file = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write);
            using var writer = new StreamWriter(file, Encoding.UTF8);
            writer.WriteLineAsync($"{DateTime.Now} {message}");
        }

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

            if (_node == null || !_node.TryLocate(FrameTime.OnUpdate, out Pose pose)) return;

            var position = new Vector3(pose.position.x + xCorrection,
                pose.position.y + yCorrection,
                pose.position.z + zCorrection);
            gameObject.transform.SetPositionAndRotation(position, pose.rotation);
            // Call on QRDataVisualizer to update transform for NetworkObject
            _dataVisualizer.SetPositionAndRotation(position, pose.rotation);
            LogStr($"{Id}; {position.ToString("F7")}; {pose.rotation.ToString("F7")}");
        }

        private void InitializeSpatialGraphNode(bool force = false)
        {
            if (_node == null || force)
            {
                _node = Id != Guid.Empty ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                DebugLog("Initialize SpatialGraphNode Id= " + Id);
            }
        }
    }
}

#endif
