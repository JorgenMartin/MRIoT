#nullable enable

using Exact.Example;
using Svanesjo.MRIoT.Multiplayer.Representation;
using UnityEngine;

namespace Svanesjo.MRIoT.Multiplayer.Device
{
    public class NetworkedColorRing : ColorRingBase
    {
        private ColorRingRepresentation? _colorRing;

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
            Debug.Log($"NetworkedColorRing ignoring SetNumberOfSegments({num})");
        }

        protected override void SetSegmentColorInternal(int segment, Color color)
        {
            throw new System.NotImplementedException();
        }

    }
}
