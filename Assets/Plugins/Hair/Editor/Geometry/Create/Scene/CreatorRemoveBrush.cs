using Scripts.HairTool.Geometry.Create;

namespace Assets.Hair.Editor.Geometry.Create.Scene
{
    public class CreatorRemoveBrush : CreatorBaseBrush
    {
        public CreatorRemoveBrush(HairGeometryCreator creator) : base(creator)
        {
        }

        public override void DrawScene()
        {
            var vertices = Creator.Geomery.Selected.Vertices;

            for (var i = 0; i < vertices.Count; i += Creator.Segments)
            {
                if (IsBrushContainsStand(i))
                {
                    vertices.RemoveRange(i, Creator.Segments);
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
    }
}
