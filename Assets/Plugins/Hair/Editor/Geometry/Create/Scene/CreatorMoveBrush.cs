using Scripts.HairTool.Geometry.Create;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Create.Scene
{
    public class CreatorMoveBrush : CreatorBaseBrush
    {
        private Vector3 oldPosition;

        public CreatorMoveBrush(HairGeometryCreator creator) : base(creator)
        {

        }

        public override void StartDrawScene()
        {
            oldPosition = Creator.Brush.Position;
        }

        public override void DrawScene()
        {
            base.DrawScene();

            var vertices = Creator.Geomery.Selected.Vertices;
            var guideVertices = Creator.Geomery.Selected.GuideVertices;
            var dir = Creator.ScalpFilter.transform.InverseTransformDirection(Creator.Brush.Position - oldPosition)*Creator.Brush.Strength;

            for (int i = 0; i < vertices.Count; i++)
            {
                if(i % Creator.Segments == 0)
                    continue;

                var vertex = vertices[i];

                var wordVertex = Creator.transform.TransformPoint(vertex);
                var distance = (guideVertices[i - 1] - guideVertices[i]).magnitude;

                if (Creator.Brush.Contains(wordVertex))
                {
                    var newVertex = FixCollisions(vertex, Colliders);
                    newVertex = FixDistance(vertices[i - 1], newVertex + dir, distance);
                    vertices[i] = newVertex;
                }
                else
                {
                    var newVertex = FixCollisions(vertex, Colliders);
                    newVertex = FixDistance(vertices[i - 1], newVertex, distance);
                    vertices[i] = newVertex;
                }
            }

            oldPosition = Creator.Brush.Position;
        }
        

        private Vector3 FixDistance(Vector3 upperVertex, Vector3 newVertex, float guideDistance)
        {
            var relPosition = upperVertex - newVertex;
            var actualDistance = relPosition.magnitude;

            var penetration = (guideDistance - actualDistance) / actualDistance;
            var correction = relPosition * penetration;
            
            return newVertex - correction;
        }
    }
}
