using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Demo
{
    public class SimplifierUtils
    {
        public static List<Vector3> Do(List<Vector3> vertices, int[] indices, int segments)
        {
            var dstVertices = vertices.ToList();
            
            for (var i = 0; i < indices.Length; i += 3)
            {
                var index0 = indices[i + 0]*segments;
                var index1 = indices[i + 1]*segments;
                var index2 = indices[i + 2]*segments;

                var b0 = CanInterpolate(vertices, index1, index2, segments);
                var b1 = CanInterpolate(vertices, index0, index2, segments);
                var b2 = CanInterpolate(vertices, index0, index1, segments);

                if (b0 && !b1 && !b2)
                {
                    indices[i + 0] = indices[dstVertices.Count/segments];
                    dstVertices.AddRange(Copy(vertices, index1, index0, segments));
                    //indices[i + 0] = indices[i + 1];
                }

                if (!b0 && b1 && !b2)
                {
                    indices[i + 1] = indices[dstVertices.Count/segments];
                    dstVertices.AddRange(Copy(vertices, index2, index1, segments));
                    //indices[i + 1] = indices[i + 2];
                }

                if (!b0 && !b1 && b2)
                    indices[i + 2] = indices[dstVertices.Count/segments];
                    dstVertices.AddRange(Copy(vertices, index1, index2, segments));
                    //indices[i + 2] = indices[i + 1];

                if (!b0 && !b1 && !b2)
                {
                    indices[i + 0] = indices[i + 2];
                    indices[i + 1] = indices[i + 2];
                }
            }

            return dstVertices;
        }

        private static bool CanInterpolate(List<Vector3> vertices, int index1, int index2, int segments)
        {
            var step1 = Mathf.RoundToInt(segments*0.5f);
            var step2 = segments - 1;

            var stepCondition1 = (vertices[index1 + step1] - vertices[index2 + step1]).sqrMagnitude < 0.1f;
            var stepCondition2 = (vertices[index1 + step2] - vertices[index2 + step2]).sqrMagnitude < 0.1f;

            return stepCondition1 && stepCondition2;
        }

        private static List<Vector3> Copy(List<Vector3> vertices, int iSrc, int iDst, int segments)
        {
            var list = vertices.GetRange(iDst, iDst + segments - 1);
            list[0] = vertices[iSrc];
            return list;
        }
    }
}
