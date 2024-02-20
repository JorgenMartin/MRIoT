#if UNITY_WSA

using System;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.DataVisualizers
{
    public class DebuggerPrefabDataVisualizer : QRDataVisualizer
    {
        private float PhysicalSize { get; set; } = 0.1f;
        private string CodeText { get; set; } = "Dummy";

        private GameObject _qrCodeCube = null!;
        private GameObject _qrInfo = null!;
        private TextMesh _qrID = null!;
        private TextMesh _qrNodeID = null!;
        private TextMesh _qrText = null!;
        private TextMesh _qrVersion = null!;
        private TextMesh _qrTimeStamp = null!;
        private TextMesh _qrSize = null!;

        private long _lastTimeStamp = 0;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            if (Code == null)
                throw new Exception("QR Code Empty");

            PhysicalSize = Code.PhysicalSideLength;
            CodeText = Code.Data;

            _qrCodeCube = gameObject.transform.Find("Cube").gameObject;
            _qrInfo = gameObject.transform.Find("QRInfo").gameObject;
            _qrID = _qrInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            _qrNodeID = _qrInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            _qrText = _qrInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            _qrVersion = _qrInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            _qrTimeStamp = _qrInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            _qrSize = _qrInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            if (_qrInfo is null || _qrCodeCube is null || _qrID is null || _qrNodeID is null || _qrText is null
                || _qrVersion is null || _qrTimeStamp is null || _qrSize is null)
            {
                throw new Exception("Components not found");
            }

            _qrID.text = "Id:" + Code.Id;
            _qrNodeID.text = "NodeId:" + Code.SpatialGraphNodeId;
            _qrText.text = CodeText;

            _qrVersion.text = "Ver: " + Code.Version;
            _qrSize.text = "Size:" + Code.PhysicalSideLength.ToString("F04") + "m";
            _qrTimeStamp.text = "Time:" + Code.LastDetectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _qrTimeStamp.color = Color.yellow;
            Debug.Log("Id= " + Code.Id + "NodeId= " + Code.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + Code.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + Code.Version + " QRData = " + CodeText);
        }

        void UpdatePropertiesDisplay()
        {
            if (Code == null || _lastTimeStamp == Code.SystemRelativeLastDetectedTime.Ticks) return;

            // Update properties that change
            _qrSize.text = "Size:" + Code.PhysicalSideLength.ToString("F04") + "m";

            _qrTimeStamp.text = "Time:" + Code.LastDetectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _qrTimeStamp.color = _qrTimeStamp.color==Color.yellow? Color.white: Color.yellow;
            PhysicalSize = Code.PhysicalSideLength;
            Debug.Log("Id= " + Code.Id + "NodeId= " + Code.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + Code.SystemRelativeLastDetectedTime.Ticks + " Time = " + Code.LastDetectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            _qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
            _qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
            _lastTimeStamp = Code.SystemRelativeLastDetectedTime.Ticks;
            _qrInfo.transform.localScale = new Vector3(PhysicalSize/0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
        }
    }
}

#endif
