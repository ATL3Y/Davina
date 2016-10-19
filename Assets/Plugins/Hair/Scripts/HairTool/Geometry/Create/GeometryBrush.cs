using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.HairTool.Geometry.Create
{
    [Serializable]
    public class GeometryBrush
    {
        public Vector3 Position;
        public Vector3 Dirrection;
        public float Radius = 1;
        public float Lenght1 = 1;
        public float Lenght2 = 0;
        public float Strength = 0.5f;
        
        ///distance to colliders
        public float CollisionDistance = 0.2f;

        private bool willDraw;

        public void Draw()
        {
            willDraw = true;
        }

        public void OnDrawGizmos()
        {
            if(!willDraw)
                return;

            willDraw = false;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Position, 0.1f);

            var m = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(Dirrection), Vector3.one);

            var step = 2 * Mathf.PI / 20;
            for (var i = 0; i < 20; i++)
            {
                var a = i*step;
                var dir = new Vector3(Mathf.Cos(a), Mathf.Sin(a));
                var dirNext = new Vector3(Mathf.Cos(a + step), Mathf.Sin(a + step));

                var p1 = dir*Radius + Vector3.forward*Lenght1;
                var p1Next = dirNext* Radius + Vector3.forward*Lenght1;

                var p2 = dir*Radius + Vector3.back*Lenght2;
                var p2Next = dirNext* Radius + Vector3.back*Lenght2;

                Gizmos.DrawLine(ToWorld(m, p1), ToWorld(m, p1Next));
                Gizmos.DrawLine(ToWorld(m, p2), ToWorld(m, p2Next));

                Gizmos.DrawLine(ToWorld(m, p1), ToWorld(m, p2));
            }
        }

        public Vector3 ToWorld(Matrix4x4 m, Vector3 local)
        {
            return Position + (Vector3) (m*local);
        }

        public bool Contains(Vector3 point)
        {
            //Assert.IsNotNull(SceneView.currentDrawingSceneView, "Use this method only from editor scripts");

            var camera = Camera.current;

            var localPoint = camera.transform.InverseTransformPoint(point);
            var localPosition = camera.transform.InverseTransformPoint(Position);

            var radiusCondition = ((Vector2)localPoint - (Vector2)localPosition).magnitude < Radius;

            var depthCondition1 = (localPosition.z - localPoint.z) > -Lenght1;
            var depthCondition2 = (localPosition.z - localPoint.z) < Lenght2;

            return radiusCondition && depthCondition1 && depthCondition2;
        }

    }
}
