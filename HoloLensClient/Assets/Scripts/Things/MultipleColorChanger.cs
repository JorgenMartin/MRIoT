using Exact.Example;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things
{
    public class MultipleColorChanger : ColorRingBase
    {
        [SerializeField] private ColorRingBase[] colorRings = { };

        public override void SetNumberOfSegments(int num)
        {
            if (num == numSegments) { return; }

            if (num > numSegments)
            {
                for (int i = numSegments; i < num; i++)
                {
                    segmentColors.Add(Color.black);
                }
            }
            else
            {
                for (int i = numSegments - 1; i >= num; i--)
                {
                    segmentColors.RemoveAt(i);
                }
            }
            numSegments = num;

            foreach (var ring in colorRings)
            {
                ring.SetNumberOfSegments(num);
            }
        }

        protected override void SetSegmentColorInternal(int segment, Color color)
        {
            // Debug.Log("Ignoring SetSegmentColorInternal, should be handled by children");
        }

        public override void SetSegmentColor(int segment, Color color)
        {
            // Passed to base for GetColor to return correctly
            base.SetSegmentColor(segment, color);
            foreach (var ring in colorRings)
            {
                ring.SetSegmentColor(segment, color);
            }
        }

        public override void SetUniformColor(Color color)
        {
            // Passed to base for GetColor to return correctly
            base.SetUniformColor(color);
            foreach (var ring in colorRings)
            {
                ring.SetUniformColor(color);
            }
        }

        public override void SetIntensity(float intensity)
        {
            foreach (var ring in colorRings)
            {
                ring.SetIntensity(intensity);
            }
        }

        public override void SetRotation(float rotation)
        {
            foreach (var ring in colorRings)
            {
                ring.SetRotation(rotation);
            }
        }
    }
}
