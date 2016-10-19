using Scripts.HairTool.Geometry.Create;

namespace Assets.Hair.Editor.Geometry.Create.Scene
{
    public class CreatorShrinkBrush : CreatorBaseBrush
    {
        private readonly float ratio;

        public CreatorShrinkBrush(HairGeometryCreator creator, float ratio) : base(creator)
        {
            this.ratio = ratio;
        }

        public override void DrawScene()
        {
            base.DrawScene();

            var vertices = Creator.Geomery.Selected.Vertices;

            for (var i = 0; i < vertices.Count; i += Creator.Segments)
            {
                if (IsBrushContainsStand(i))
                {
                    Shrink(i);
                }
            }
        }

        private bool IsBrushContainsStand(int startIndex)
        {
            var group = Creator.Geomery.Selected;
            var vertices = group.Vertices;

            for (var i = startIndex; i < startIndex + Creator.Segments; i++)
            {
                var vertex = vertices[i];
                var wordVertex = Creator.ScalpFilter.transform.TransformPoint(vertex);

                if (Creator.Brush.Contains(wordVertex))
                    return true;
            }

            return false;
        }

        private void Shrink(int startIndex)
        {
            var group = Creator.Geomery.Selected;
            var vertices = group.Vertices;

            for (var i = startIndex + 1; i < startIndex + Creator.Segments; i++)
            {
                var vertex1 = vertices[i - 1];
                var vertex2 = vertices[i];

                var diff = (vertex2 - vertex1)*(1 + ratio*Creator.Brush.Strength);

                vertex2 = vertex1 + diff;
                vertices[i] = FixCollisions(vertex2, Colliders);
            }
        }
    }
}
