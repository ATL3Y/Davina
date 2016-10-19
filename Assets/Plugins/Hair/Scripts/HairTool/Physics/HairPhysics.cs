using Scripts.HairTool.Settings;
using Scripts.HairTool.Wind;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.HairTool.Physics
{
    public class HairPhysics : MonoBehaviour
    {
        private HairSettings settings;
        private ComputeShader shader;
        private WindReceiver wind;

        private void Start()
        {
            wind = gameObject.AddComponent<WindReceiver>();
        }

        public void Initialize(HairSettings settings)
        {
            this.settings = settings;
            Assert.IsNotNull(settings.PhysicsSettings.Shader, "Add compute shader in Physics Settings");
            shader = Instantiate(settings.PhysicsSettings.Shader);

            InitBuffers();
        }

        private void InitBuffers()
        {
            SetBuffersToKernel(0);
            SetBuffersToKernel(1);
            SetBuffersToKernel(2);
            
            UpdateParams();

            DispatchKernel(0);
        }

        private void LateUpdate()
        {
            UpdateParams();
            DispatchKernel(1);
            shader.Dispatch(2, settings.Builder.Joints.Joints.Length, 6, 1);
        }

        private void SetBuffersToKernel(int kernel)
        {
            shader.SetBuffer(kernel, "bodiesBuffer", settings.Builder.Stands.Buffer);
            shader.SetBuffer(kernel, "staticBodiesBuffer", settings.Builder.Colliders.Buffer);
            shader.SetBuffer(kernel, "guideJointsBuffer", settings.Builder.Joints.Buffer);
        }

        private void DispatchKernel(int kernel)
        {
            shader.Dispatch(kernel, settings.Builder.Stands.SizeX, settings.Builder.Stands.SizeY, 1);
        }

        private void UpdateParams()
        {
            var matrix = settings.StandsSettings.Provider.transform.localToWorldMatrix;
            var floats = new float[16];
            for (var i = 0; i < 16; i++)
            {
                floats[i] = matrix[i];
            }

            shader.SetFloats("transform", floats);
            shader.SetInt("sizeY", settings.Builder.Stands.SizeY);
            shader.SetFloat("gravity", settings.PhysicsSettings.Gravity);
            shader.SetFloat("drag", 1 - settings.PhysicsSettings.Drag);
            shader.SetFloat("elasticyRoot", settings.PhysicsSettings.ElasticyRoot);
            shader.SetFloat("elasticyTip", settings.PhysicsSettings.ElasticyTip);

            if (wind != null)
                shader.SetVector("wind", wind.GetWind(settings.StandsSettings.HeadCenterWorld) * 0.01f);

            settings.Builder.Colliders.UpdateCompute();
            settings.Builder.Joints.UpdateCompute();
        }
    }
}
