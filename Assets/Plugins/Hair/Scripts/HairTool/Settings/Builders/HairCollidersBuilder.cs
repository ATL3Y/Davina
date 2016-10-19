using System.Collections.Generic;
using Scripts.HairTool.Physics;
using Scripts.HairTool.Settings.Builders.Abstract;
using UnityEngine;

namespace Scripts.HairTool.Settings.Builders
{
    public class HairCollidersBuilder : HairBuilderBase
    {
        public GBody[] Bodies { private set; get; }
        public ComputeBuffer Buffer { private set; get; }

        private HairSettings settings;

        public override void BuildData(HairSettings settings)
        {
            this.settings = settings;
            CreateBodyArray(settings.PhysicsSettings.GetColliders());
        }

        public override void BuildCompute()
        {
            Buffer = new ComputeBuffer(Bodies.Length, GBody.GetSizeBytes());
            Buffer.SetData(Bodies);
        }

        public override void Destroy()
        {
            Buffer.Dispose();
        }

        public override void UpdateCompute()
        {
            CreateBodyArray(settings.PhysicsSettings.GetColliders());
            Buffer.SetData(Bodies);
        }

        private void CreateBodyArray(List<SphereCollider> colliders)
        {
            if(Bodies == null)
                Bodies = new GBody[colliders.Count];

            for (var i = 0; i < colliders.Count; i++)
            {
                var sphereCollider = colliders[i];
                var transformedPoint = sphereCollider.transform.TransformPoint(sphereCollider.center);
                var transformedRadius = sphereCollider.transform.lossyScale.x*sphereCollider.radius;

                Bodies[i] = new GBody(transformedPoint, transformedRadius);
            }
        }
    }
}
