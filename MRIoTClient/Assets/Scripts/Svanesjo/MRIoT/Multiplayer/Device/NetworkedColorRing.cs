#nullable enable

using Exact.Example;
using Svanesjo.MRIoT.Multiplayer.Representation;
using Svanesjo.MRIoT.Utility;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.Multiplayer.Device
{
    public class NetworkedColorRing : ColorRingBase
    {
        private ColorRingRepresentation? _colorRing;
        private ILogger _logger = new DebugLogger(typeof(NetworkedColorRing));

        public void Initialize(ColorRingRepresentation colorRingRepresentation)
        {
            _colorRing = colorRingRepresentation;
        }

        public override void SetUniformColor(Color color)
        {
            if (_colorRing != null)
            {
                _colorRing.SetUniformColor(color);
            }
        }

        public override void SetNumberOfSegments(int num)
        {
            _logger.Log($"ignoring SetNumberOfSegments({num})");
        }

        protected override void SetSegmentColorInternal(int segment, Color color)
        {
            throw new System.NotImplementedException();
        }

    }
}
