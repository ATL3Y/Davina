using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.HairTool.Geometry.Abstract;
using Scripts.HairTool.Geometry.Tools;
using UnityEngine;

namespace Scripts.HairTool.Geometry.Create
{
    [Serializable]
    public class HairGeometryCreator : GeometryProviderBase
    {
        public bool Debug = false;
        public int Segments = 5;

        public GeometryBrush Brush = new GeometryBrush();

        public MeshFilter ScalpFilter;
        public List<GameObject> ColliderProviders = new List<GameObject>();
        public CreatorGeometry Geomery = new CreatorGeometry();

        private int[] indices;
        private List<Vector3> vertices;

        private void Awake()
        {
            var verticesList = new List<Vector3>();
            var listVerticesGroup = new List<List<Vector3>>();
            foreach (var groupData in Geomery.List)
            {
                listVerticesGroup.Add(groupData.Vertices);
                verticesList.AddRange(groupData.Vertices);
            }

            indices = ScalpProcessingTools.ProcessIndices(ScalpFilter.mesh.GetIndices(0).ToList(), ScalpFilter.mesh.vertices.ToList(), listVerticesGroup, Segments).ToArray();
            vertices = verticesList;
        }

        private void OnDrawGizmos()
        {
            if(!Debug)
                return;

            Brush.OnDrawGizmos();

            foreach (var data in Geomery.List)
            {
                var color = Geomery.Selected == data 
                    ? Color.green 
                    : Color.grey;

                data.OnDrawGizmos(Segments, color);
            }
        }

        public override int GetSegments()
        {
            return Segments;
        }

        public override int[] GetIndices()
        {
            return indices;
        }

        public override List<Vector3> GetVertices()
        {
            return vertices;
        }
    }
}
