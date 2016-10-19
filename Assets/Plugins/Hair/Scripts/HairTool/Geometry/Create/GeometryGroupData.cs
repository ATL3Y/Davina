using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.HairTool.Geometry.Create
{
    [Serializable]
    public class GeometryGroupData
    {
        public float Length = 2;

        public List<Vector3> GuideVertices;
        public List<Vector3> Vertices;
        public MeshFilter Filter;

        public void Generate(MeshFilter meshFilter, int segments)
        {
            Filter = meshFilter;
            var mesh = meshFilter.sharedMesh;

            Vertices = new List<Vector3>();
            GuideVertices = new List<Vector3>();
            
            var firstVertices = new List<Vector3>();

            for (var i = 0; i < mesh.vertices.Length; i++)
            {
                var vertex = mesh.vertices[i];
                var normal = mesh.normals[i];

                if (firstVertices.Contains(vertex))
                    continue;

                var stand = CreateStand(vertex, normal, segments);
                Vertices.AddRange(stand);
                GuideVertices.AddRange(stand);
                firstVertices.Add(vertex);
            }

            UnityEngine.Debug.Log("Total nodes:" + Vertices.Count);
        }

        public void Reset()
        {
            Vertices.Clear();
            Vertices = null;
        }

        private List<Vector3> CreateStand(Vector3 start, Vector3 normal, int segments)
        {
            var list = new List<Vector3>();

            var step = Length/segments;
            for (var i = 0; i < segments; i++)
            {
                list.Add(start + normal*(step*i));
            }

            return list;
        } 

        public void OnDrawGizmos(int segments, Color color)
        {
            Gizmos.color = color;

            if (Vertices == null)
                return;
            
            for (var i = 1; i < Vertices.Count; i++)
            {
                if (i%segments == 0)
                    continue;

                var vertex1 = Filter.transform.TransformPoint(Vertices[i - 1]);
                var vertex2 = Filter.transform.TransformPoint(Vertices[i]);

                Gizmos.DrawLine(vertex1, vertex2);
            }
        }
    }
}
