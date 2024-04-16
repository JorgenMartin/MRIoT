#nullable enable

using Exact.Example;
using Svanesjo.MRIoT.Utility;
using UnityEngine;
using ILogger = Svanesjo.MRIoT.Utility.ILogger;

namespace Svanesjo.MRIoT.Multiplayer.Representation
{
    public class MeshColorChanger : ColorRingBase
    {
        [SerializeField] private MeshRenderer[] meshRenderers = {};
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private ILogger _logger = new DebugLogger(typeof(MeshColorChanger));

        public override void SetUniformColor(Color color)
        {
            _logger.Log($"setting uniform color to {color}");
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.SetColor(Color1, color);
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
