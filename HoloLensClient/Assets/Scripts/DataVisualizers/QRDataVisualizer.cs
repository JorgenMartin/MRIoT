using System;
using Microsoft.MixedReality.QR;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.DataVisualizers
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRDataVisualizer : MonoBehaviour
    {
        public QRCode? Code;

        private void Start()
        {
            if (Code == null)
            {
                throw new Exception("QR Code Empty");
            }
        }
    }
}
