using System.Collections.Generic;
using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Geometry.Create;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Create.Scene
{
    public class ColliderData
    {
        public Vector3 Position;
        public float Radius;
    }

    public class CreatorBaseBrush : EditorItemBase
    {
        protected HairGeometryCreator Creator;
        protected List<ColliderData> Colliders;

        public CreatorBaseBrush(HairGeometryCreator creator)
        {
            Creator = creator;
        }

        public virtual void StartDrawScene()
        {
            
        }

        public override void DrawScene()
        {
            UpdateGuides();
            Colliders = GetColliders();
        }

        public List<ColliderData> GetColliders()
        {
            Colliders = Colliders ?? new List<ColliderData>();
            Colliders.Clear();

            for (var i = 0; i < Creator.ColliderProviders.Count; i++)
            {
                var colliderProvider = Creator.ColliderProviders[i];
                var colliders = colliderProvider.GetComponents<SphereCollider>();

                for (var j = 0; j < colliders.Length; j++)
                {
                    var collider = colliders[j];
                    var p = colliderProvider.transform.TransformPoint(collider.center);
                    p = Creator.transform.InverseTransformPoint(p);

                    var r = collider.radius*colliderProvider.transform.lossyScale.x;
                    r /= Creator.transform.lossyScale.x;
                    var newCollider = new ColliderData
                    {
                        Position = p,
                        Radius = r
                    };
                    Colliders.Add(newCollider);
                }
            }

            return Colliders;
        }

        protected Vector3 FixCollisions(Vector3 vertex, List<ColliderData> colliders)
        { 
            for (var i = 0; i < colliders.Count; i++)
                vertex = FixCollision(vertex, colliders[i]);

            return vertex;
        }

        protected Vector3 FixCollision(Vector3 vertex, ColliderData collider)
        {
            var relPosition = vertex - collider.Position;
            var sumRadius = collider.Radius + Creator.Brush.CollisionDistance;

            if (relPosition.sqrMagnitude > sumRadius*sumRadius)
                return vertex;

            var penetration = sumRadius - relPosition.magnitude;
            var normal = relPosition.normalized;

            var correction = normal * penetration;
            vertex += correction * 0.9f;

            return vertex;
        }

        private void UpdateGuides()
        {
            var vertices = Creator.Geomery.Selected.Vertices;
            var guideVertices = Creator.Geomery.Selected.GuideVertices;

            for (var i = 0; i < vertices.Count; i++)
            {
                guideVertices[i] = vertices[i];
            }
        }
    }
}
