using Scripts.HairTool.Settings;
using UnityEngine;

namespace Scripts.HairTool.Render
{
    public class HairRender : MonoBehaviour
    {
        private Mesh mesh;
        private new MeshRenderer renderer;
        private HairSettings settings;

        private void Awake()
        {
            mesh = new Mesh();
            renderer = gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
        }

        public void Initialize(HairSettings settings)
        {
            this.settings = settings;

            renderer.material = settings.RenderSettings.HairMaterial;
            renderer.material.SetBuffer("_BarycentricBuffer", settings.Builder.Barycentric.Buffer);

            renderer.material.SetBuffer("_BodiesBuffer", settings.Builder.Stands.Buffer);
            renderer.material.SetVector("_Size", new Vector4(settings.Builder.Stands.SizeX, settings.Builder.Stands.SizeY));

            mesh.vertices = new Vector3[settings.Builder.Stands.SizeX];
            mesh.SetIndices(settings.Builder.Stands.Indices, MeshTopology.Triangles, 0);

            mesh.bounds = new Bounds(Vector3.zero, settings.StandsSettings.BoundsSize);
        }

        private void LateUpdate()
        {
            var position = settings.StandsSettings.HeadCenterWorld;

            renderer.material.SetVector("_LightCenter", settings.StandsSettings.HeadCenterWorld);

            renderer.material.SetVector("_TessFactor", new Vector4(settings.LODSettings.GetDetail(position), settings.LODSettings.GetDencity(position)));
            renderer.material.SetFloat("_StandWidth", settings.LODSettings.GetWidth(position));

            renderer.material.SetColor("_TipColor", settings.RenderSettings.TipColor);
            renderer.material.SetColor("_RootColor", settings.RenderSettings.RootColor);
            renderer.material.SetFloat("_ColorBlend", settings.RenderSettings.ColorBlend);

            var lenght = new Vector4(settings.RenderSettings.Length1, settings.RenderSettings.Length2, settings.RenderSettings.Length3);
            renderer.material.SetVector("_Length", lenght);

            renderer.material.SetFloat("_SpecularShift", 0.01f);
            renderer.material.SetFloat("_PrimarySpecular", settings.RenderSettings.PrimarySpecular);
            renderer.material.SetFloat("_SecondarySpecular", settings.RenderSettings.SecondarySpecular);
            renderer.material.SetColor("_SpecularColor", settings.RenderSettings.SpecularColor);

            renderer.material.SetFloat("_WavinessScale", settings.RenderSettings.WavinessScale);
            renderer.material.SetFloat("_WavinessFrequency", settings.RenderSettings.WavinessFrequency);

            renderer.material.SetFloat("_Interpolation", settings.RenderSettings.Interpolation);

            renderer.material.SetFloat("_Volume", settings.RenderSettings.Volume);
        }
    }
}
