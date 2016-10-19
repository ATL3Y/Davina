using System;
using UnityEngine;

namespace Scripts.HairTool.Physics
{
    [Serializable]
    public struct GBody
    {
        public Vector3 GuidePosition;
        public Vector3 Position;
        public Vector3 LastPosition;
        public float Radius;

        public GBody(Vector3 position, float radius)
        {
            GuidePosition = position;
            Position = position;
            LastPosition = position;
            Radius = radius;
        }

        public static int GetSizeBytes()
        {
            return sizeof(float)*10;
        }
    }
}
