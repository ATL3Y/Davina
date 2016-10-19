using Scripts.HairTool.Settings;
using UnityEngine;
using UnityEngine.Rendering;

namespace Scripts.HairTool.Render
{
    public class ShadowRenderer : MonoBehaviour
    {
        private Mesh mesh;
        private new MeshRenderer renderer;
        private HairSettings settings;

        public void Initialize(HairSettings settings)
        {
            this.settings = settings;

            renderer.material = settings.ShadowSettings.Material;

            renderer.material.SetBuffer("_BarycentricBuffer", settings.Builder.Barycentric.Buffer);

            renderer.material.SetBuffer("_BodiesBuffer", settings.Builder.Stands.Buffer);
            renderer.material.SetVector("_Size", new Vector4(settings.Builder.Stands.SizeX, settings.Builder.Stands.SizeY));

            mesh.vertices = new Vector3[settings.Builder.Stands.SizeX];
            mesh.SetIndices(settings.Builder.Stands.Indices, MeshTopology.Triangles, 0);
        }

        private void Awake()
        {
            mesh = new Mesh();
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            renderer.receiveShadows = true;
        }

        private void Update()
        {
            var position = settings.StandsSettings.HeadCenterWorld;

            renderer.material.SetVector("_LightCenter", settings.StandsSettings.HeadCenterWorld);

            renderer.material.SetVector("_TessFactor", new Vector4(settings.LODSettings.GetDetail(position)*0.1f, settings.LODSettings.GetDencity(position)*0.5f));
            renderer.material.SetFloat("_StandWidth", settings.LODSettings.GetWidth(position)*10);

            renderer.material.SetFloat("_WavinessScale", settings.RenderSettings.WavinessScale);
            renderer.material.SetFloat("_WavinessFrequency", settings.RenderSettings.WavinessFrequency);

            renderer.material.SetFloat("_Interpolation", settings.RenderSettings.Interpolation);
        }
    }
}
