#if UNITY_WSA

#nullable enable

using System;
using Svanesjo.MRIoT.Things.Calibration;

namespace Svanesjo.MRIoT.DataVisualizers
{
    public class QRCalibrationTileVisualizer : QRDataVisualizer
    {

        protected override void Start()
        {
            base.Start();

            if (Code == null)
                throw new ArgumentNullException(nameof(Code));
            if (SpawnedNetworkObject == null)
                throw new ArgumentNullException(nameof(SpawnedNetworkObject));

            var calibrationTile = SpawnedNetworkObject.GetComponent<CalibrationTile>();
            if (calibrationTile != null && int.TryParse(Code.Data, out var id))
                calibrationTile.SetId(id);
        }
    }
}

#endif
