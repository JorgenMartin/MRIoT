#nullable enable

using Exact.Example;
using UnityEngine;

namespace Svanesjo.MRIoT.Things.ColorRing
{
    public class MeshColorChanger : ColorRingBase
    {
        [SerializeField] private MeshRenderer[] meshRenderers = {};
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public override void SetUniformColor(Color color)
        {
            Debug.Log($"MeshColorChanger setting uniform color to {color}");
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.SetColor(Color1, color);
            }
        }

        public override void SetNumberOfSegments(int num)
        {
            Debug.Log($"MeshColorChanger ignoring SetNumberOfSegments({num})");
        }

        protected override void SetSegmentColorInternal(int segment, Color color)
        {
            throw new System.NotImplementedException();
        }
    }
}
