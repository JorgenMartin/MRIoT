using System;
using Microsoft.MixedReality.QR;
using UnityEngine;

namespace Svanesjo.MRIoT.DataVisualizers
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRDataVisualizer : MonoBehaviour
    {
        public QRCode qrCode;

        private void Start()
        {
            if (qrCode == null)
            {
                throw new Exception("QR Code Empty");
            }
        }
    }
}
