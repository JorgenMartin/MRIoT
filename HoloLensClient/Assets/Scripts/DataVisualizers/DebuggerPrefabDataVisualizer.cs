using System;
using UnityEngine;

namespace Svanesjo.MRIoT.DataVisualizers
{
    public class DebuggerPrefabDataVisualizer : QRDataVisualizer
    {
        private GameObject _qrCodeCube;

        private float PhysicalSize { get; set; } = 0.1f;
        private string CodeText { get; set; } = "Dummy";

        private TextMesh _qrID;
        private TextMesh _qrNodeID;
        private TextMesh _qrText;
        private TextMesh _qrVersion;
        private TextMesh _qrTimeStamp;
        private TextMesh _qrSize;
        private GameObject _qrInfo;

        private long _lastTimeStamp = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (qrCode == null)
            {
                throw new Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;

            _qrCodeCube = gameObject.transform.Find("Cube").gameObject;
            _qrInfo = gameObject.transform.Find("QRInfo").gameObject;
            _qrID = _qrInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            _qrNodeID = _qrInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            _qrText = _qrInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            _qrVersion = _qrInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            _qrTimeStamp = _qrInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            _qrSize = _qrInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            _qrID.text = "Id:" + qrCode.Id;
            _qrNodeID.text = "NodeId:" + qrCode.SpatialGraphNodeId;
            _qrText.text = CodeText;

            _qrVersion.text = "Ver: " + qrCode.Version;
            _qrSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";
            _qrTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _qrTimeStamp.color = Color.yellow;
            Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && _lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                _qrSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                _qrTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                _qrTimeStamp.color = _qrTimeStamp.color==Color.yellow? Color.white: Color.yellow;
                PhysicalSize = qrCode.PhysicalSideLength;
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                _qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                _qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                _lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
                _qrInfo.transform.localScale = new Vector3(PhysicalSize/0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
        }
    }
}
