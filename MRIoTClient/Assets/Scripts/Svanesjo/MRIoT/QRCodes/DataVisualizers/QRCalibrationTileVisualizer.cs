#nullable enable

#if UNITY_WSA

using System;
using Svanesjo.MRIoT.Multiplayer.Calibration;

namespace Svanesjo.MRIoT.QRCodes.DataVisualizers
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
