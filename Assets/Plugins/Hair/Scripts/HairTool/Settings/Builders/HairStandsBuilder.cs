using System.Linq;
using Scripts.HairTool.Physics;
using Scripts.HairTool.Settings.Builders.Abstract;
using UnityEngine;

namespace Scripts.HairTool.Settings.Builders
{
    public class HairStandsBuilder : HairBuilderBase
    {
        public GBody[] Bodies { private set; get; }
        public int[] Indices { private set; get; }

        public int SizeX { private set; get; }
        public int SizeY{ private set; get; }

        public ComputeBuffer Buffer { private set; get; }

        public override void BuildData(HairSettings settings)
        {
            var provider = settings.StandsSettings.Provider;
            var standRadius = settings.PhysicsSettings.StandRadius*provider.transform.lossyScale.x;

            Indices = provider.GetIndices();
            Bodies = GetGuideBodies(provider.GetVertices().ToArray(), standRadius);

            SizeY = settings.StandsSettings.Segments;
            SizeX = Bodies.Length/SizeY;
        }

        public override void BuildCompute()
        {
            Buffer = new ComputeBuffer(Bodies.Length, GBody.GetSizeBytes());
            Buffer.SetData(Bodies);
        }

        public override void Destroy()
        {
            Buffer.Dispose();
        }

        private GBody[] GetGuideBodies(Vector3[] vertices, float radius)
        {
            return vertices.Select(vertex => new GBody(vertex, radius)).ToArray();
        }
    }
}
