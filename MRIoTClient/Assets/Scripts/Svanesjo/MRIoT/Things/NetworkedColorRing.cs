using Exact.Example;
using Svanesjo.MRIoT.Things.Network;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things
{
    public class NetworkedColorRing : ColorRingBase
    {
        public NetworkColorRing? networkColorRing;

        public override void SetUniformColor(Color color)
        {
            if (networkColorRing != null)
            {
                networkColorRing.SetUniformColor(color);
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
