using System.Collections.Generic;
using System.Linq;
using Scripts.HairTool.Geometry.Abstract;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 649

namespace Scripts.HairTool.Geometry.Import
{
    [ExecuteInEditMode]
    public class GeometryProvider : GeometryProviderBase
    {
        [SerializeField] private bool debug = true;
        [SerializeField] [Range(3, 25)] private int segments = 5;
        [SerializeField] private MeshFilter scalpFilter;
        [SerializeField] private MeshFilter[] hairFilters;

        public int[] Indices { get; private set; }
        public List<Vector3> Vertices { get; private set; }

        public override void Validate()
        {
            Assert.IsNotNull(scalpFilter, "Add scalp filter in Geometry provider");
            Assert.IsTrue(hairFilters.Length > 0, "Add at least one hair mesh in Geometry provider");

            foreach (var hairFilter in hairFilters)
                Assert.IsNotNull(hairFilter);
        }

        public void Process()
        {
            Validate();

            ProcessIndices();
            ProcessVertices();
        }

        private void Awake()
        {
            Process();
        }

        private void ProcessIndices()
        {
            var hairIndices = new List<int>();

            var scalpIndices = scalpFilter.sharedMesh.GetIndices(0).ToList();
            var scalpVertices = GetVertices(scalpFilter);

            var grouStartIndex = 0;
            foreach (var hairFilter in hairFilters)
            {
                var hairVertices = GetVertices(hairFilter);

                var groupIndices = ProcessIndicesForMesh(grouStartIndex, scalpVertices, scalpIndices, hairVertices);
                hairIndices.AddRange(groupIndices);

                grouStartIndex += hairVertices.Count + 1;
            }
            
            for (int i = 0; i < hairIndices.Count; i++)
            {
                hairIndices[i] = hairIndices[i]/segments;
            }

            Indices = hairIndices.ToArray();
        }

        private List<int> ProcessIndicesForMesh(int startIndex, List<Vector3> scalpVertices, List<int> scalpIndices, List<Vector3> hairVertices)
        {
            var hairIndices = new List<int>();

            for (var i = 0; i < scalpIndices.Count; i++)
            {
                var index = scalpIndices[i];
                var scalpVertex = scalpVertices[index];

                if (i % 3 == 0)
                    FixNotCompletedPolygon(hairIndices);

                for (var j = 0; j < hairVertices.Count; j += segments)
                {
                    var hairVertex = hairVertices[j];

                    if ((hairVertex - scalpVertex).sqrMagnitude < 0.0001f)
                    {
                        hairIndices.Add(startIndex + j);
                        break;
                    }
                }
            }
            
            FixNotCompletedPolygon(hairIndices);

            return hairIndices;
        }

        private void FixNotCompletedPolygon(List<int> hairIndices)
        {
            var countToRemove = hairIndices.Count % 3;
            if(countToRemove > 0)
            hairIndices.RemoveRange(hairIndices.Count - countToRemove, countToRemove);
        }

        private void ProcessVertices()
        {
            var vertices = new List<Vector3>();
            foreach (var hairFilter in hairFilters)
            {
                if (hairFilter != null)
                {
                    vertices.AddRange(GetVertices(hairFilter));
                }
                else
                {
                    return;
                }
            }
            Vertices = vertices;
        }

        /// <summary>
        /// Vetices in provider local space
        /// </summary>
        private List<Vector3> GetVertices(MeshFilter filter)
        {
            var list = new List<Vector3>();

            for (var i = 0; i < filter.sharedMesh.vertexCount; i++)
            {
                var vertex = filter.sharedMesh.vertices[i];
                var worldVertex = filter.transform.TransformPoint(vertex);
                var providerVertex = transform.InverseTransformPoint(worldVertex);
                list.Add(providerVertex);
            }

            return list;
        }

        public List<Vector3> WorldVertices
        {
            get
            {
                if(Vertices == null || Vertices.Count == 0)
                    ProcessVertices();

                return Vertices.Select(v => transform.TransformPoint(v)).ToList();
            }
        }

        public int Segments
        {
            get { return segments; }
        }

        private void OnDrawGizmos()
        {
            if(!debug || Vertices == null)
                return;

            var worldVertices = WorldVertices;
            for (var i = 1; i < worldVertices.Count; i++)
            {
                if (i % Segments == 0)
                    continue;

                var vertex1 = worldVertices[i - 1];
                var vertex2 = worldVertices[i];

                Gizmos.DrawLine(vertex1, vertex2);
            }
        }

        public override int GetSegments()
        {
            return Segments;
        }

        public override int[] GetIndices()
        {
            return Indices;
        }

        public override List<Vector3> GetVertices()
        {
            return Vertices;
        }
    }
}
