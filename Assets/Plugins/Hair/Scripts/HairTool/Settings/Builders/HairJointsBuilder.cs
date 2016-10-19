using System.Collections.Generic;
using Scripts.HairTool.Physics;
using Scripts.HairTool.Settings.Builders.Abstract;
using UnityEngine;

namespace Scripts.HairTool.Settings.Builders
{
    public class HairJointsBuilder : HairBuilderBase
    {
        public GJoint[] Joints { private set; get; }
        public ComputeBuffer Buffer { private set; get; }
        private HairSettings settings;
        public override void BuildData(HairSettings settings)
        {
            this.settings = settings;
            var joints  = new List<GJoint>();
            
            foreach (var jointArea in settings.PhysicsSettings.JointAreas)
                joints.AddRange(ProcessJointArea(jointArea, settings.Builder.Stands.Bodies));

            Joints = joints.ToArray();
        }

        private List<GJoint> ProcessJointArea(HairJointArea jointArea, GBody[] bodies)
        {
            var result  = new List<int>();
            var usedXs = new List<int>();

            for (var i = 0; i < bodies.Length; i++)
            {
                var body = bodies[i];
                var diff = body.Position - jointArea.transform.localPosition;
                var x = Mathf.FloorToInt((float)i/settings.StandsSettings.Segments);

                if (diff.magnitude < jointArea.Radius && !usedXs.Contains(x))
                {
                    result.Add(i);
                    usedXs.Add(x);
                }
            }
            
            return CreateJoints(result);
        }

        private List<GJoint> CreateJoints(List<int> bodies)
        {
            var joints = new List<GJoint>();

            for (var i = 0; i < bodies.Count; i++)
            {
                var body = bodies[i];
                joints.Add(new GJoint(body, 1));
            }

            return joints;
        }

        public override void BuildCompute()
        {
            Buffer = new ComputeBuffer(Joints.Length, GJoint.GetSizeBytes());
            Buffer.SetData(Joints);
        }

        public override void Destroy()
        {
            Buffer.Dispose();
        }
    }
}
