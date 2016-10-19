using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.HairTool.Settings.Data.Abstract;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.HairTool.Settings.Data
{
    /// <summary>
    /// Physics simulation settings 
    /// </summary>
    [Serializable]
    public class HairPhysicsSettings : HairSettingsBase
    {
        //engine
        public ComputeShader Shader;

        //stands
        public float Gravity = -1;
        public float Drag = 0;
        public float StandRadius = 0.1f;

        //stands elasticy
        public float ElasticyRoot = 0.25f;
        public float ElasticyTip = 0;

        //colliders
        public List<GameObject> ColliderProviders;

        //Joints
        public List<HairJointArea> JointAreas; 

        public List<SphereCollider> Colliders
        {
            get { return colliders ?? (colliders = GetColliders()); }
        }

        #region compute data

        private List<SphereCollider> colliders;

        public List<SphereCollider> GetColliders()
        {
            var list = new List<SphereCollider>();

            foreach (var provider in ColliderProviders)
                list.AddRange(provider.GetComponents<SphereCollider>().ToList());

            return list;
        }

        #endregion

        public override void Validate()
        {
            Assert.IsNotNull(Shader, "Add compute shader to physics settings");
            foreach (var colliderProvider in ColliderProviders)
                Assert.IsNotNull(colliderProvider, "Setup Colliders Provider in Physics Settings it can't be null.");
        }
    }
}
