using Exact.Example;
using UnityEngine;

#nullable enable

namespace Svanesjo.MRIoT.Things
{
    public class LightColorChanger : ColorRingBase
    {
        [SerializeField] private Light[] lights = {};
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public override void SetUniformColor(Color color)
        {
            foreach (var light1 in lights)
            {
                light1.color = color;
            }
        }

        public override void SetNumberOfSegments(int num)
        {
            Debug.Log($"LightColorChanger ignoring SetNumberOfSegments({num})");
        }

        protected override void SetSegmentColorInternal(int segment, Color color)
        {
            throw new System.NotImplementedException();
        }
    }
}
