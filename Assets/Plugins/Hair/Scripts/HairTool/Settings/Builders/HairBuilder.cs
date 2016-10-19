using Scripts.HairTool.Physics;
using Scripts.HairTool.Render;
using UnityEngine;

namespace Scripts.HairTool.Settings.Builders
{
    public class HairBuilder
    {
        public HairStandsBuilder Stands;
        public HairCollidersBuilder Colliders;
        public BarycentricBuilder Barycentric;
        public HairJointsBuilder Joints;

        public HairBuilder()
        {
            Colliders = new HairCollidersBuilder();
            Stands = new HairStandsBuilder();
            Barycentric = new BarycentricBuilder();
            Joints = new HairJointsBuilder();
        }

        public void Build(HairSettings settings)
        {
            Stands.BuildData(settings);//todo merge methods
            Stands.BuildCompute();

            Colliders.BuildData(settings);
            Colliders.BuildCompute();

            Barycentric.BuildData(settings);
            Barycentric.BuildCompute();

            Joints.BuildData(settings);
            Joints.BuildCompute();

            var hairObj = BuildHair(settings);
            hairObj.transform.SetParent(settings.transform.parent, false);

            if (settings.ShadowSettings.CastShadows && settings.ShadowSettings.Material != null)
            {
                BuildShadow(settings, hairObj.transform);
            }
        }

        public void Destroy()
        {
            Colliders.Destroy();
            Stands.Destroy();
            Barycentric.Destroy();
            Joints.Destroy();
        }

        private GameObject BuildHair(HairSettings settings)
        {
            var obj = new GameObject("Render");

            var render = obj.AddComponent<HairRender>();
            render.Initialize(settings);

            var physics = obj.AddComponent<HairPhysics>();
            physics.Initialize(settings);

            return obj;
        }

        /// <summary>
        /// Build hair shadow gameobject 
        /// </summary>
        private void BuildShadow(HairSettings group, Transform parent)
        {
            var shadow = new GameObject("Shadow");
            var shadowRenderer = shadow.AddComponent<ShadowRenderer>();
            shadowRenderer.Initialize(group);
            shadow.transform.SetParent(parent);
        }
    }
}
