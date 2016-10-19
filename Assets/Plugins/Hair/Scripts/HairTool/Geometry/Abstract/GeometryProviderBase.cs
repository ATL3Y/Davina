using System.Collections.Generic;
using UnityEngine;

namespace Scripts.HairTool.Geometry.Abstract
{
    public abstract class GeometryProviderBase : MonoBehaviour
    {
        public abstract int GetSegments();

        public abstract int[] GetIndices();
        public abstract List<Vector3> GetVertices();

        public virtual void Validate()
        {
            
        }
    }
}
