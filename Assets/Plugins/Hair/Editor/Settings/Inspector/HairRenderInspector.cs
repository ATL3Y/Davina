using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Settings;
using Scripts.HairTool.Settings.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Settings.Inspector
{
    public class HairRenderInspector : EditorItemBase
    {
        private HairSettings settings;

        public HairRenderInspector(HairSettings settings)
        {
            this.settings = settings;
        }

        public override void DrawInspector()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Render Settings", EditorStyles.boldLabel);
            Render.HairMaterial = (Material)EditorGUILayout.ObjectField("Hair Material", Render.HairMaterial, typeof(Material), true);

            //color
            GUILayout.BeginVertical(EditorStyles.helpBox);
            Render.RootColor = EditorGUILayout.ColorField("Root Color", Render.RootColor);
            Render.TipColor = EditorGUILayout.ColorField("Tip Color", Render.TipColor);
            Render.ColorBlend = EditorGUILayout.FloatField("Color Blend", Render.ColorBlend);
            GUILayout.EndVertical();

            //specular
            GUILayout.BeginVertical(EditorStyles.helpBox);
            Render.PrimarySpecular = EditorGUILayout.FloatField("Primary Specular", Render.PrimarySpecular);
            Render.SecondarySpecular = EditorGUILayout.FloatField("Secondary Specular", Render.SecondarySpecular);
            Render.SpecularColor = EditorGUILayout.ColorField("Specular Color", Render.SpecularColor);
            GUILayout.EndVertical();

            //lenght
            GUILayout.BeginVertical(EditorStyles.helpBox);
            Render.Length1 = Mathf.Clamp(EditorGUILayout.FloatField("Length 1", Render.Length1), 0, 1);
            Render.Length2 = Mathf.Clamp(EditorGUILayout.FloatField("Length 2", Render.Length2), 0, 1);
            Render.Length3 = Mathf.Clamp(EditorGUILayout.FloatField("Length 3", Render.Length3), 0, 1);
            GUILayout.EndVertical();

            //interpolation
            GUILayout.BeginVertical(EditorStyles.helpBox);
            Render.Interpolation = Mathf.Clamp(EditorGUILayout.FloatField("Interpolation", Render.Interpolation), 0, 1);
            GUILayout.EndVertical();

            //waviness
            GUILayout.BeginVertical(EditorStyles.helpBox);
            Render.WavinessScale = EditorGUILayout.FloatField("Waviness Scale", Render.WavinessScale);
            Render.WavinessFrequency = EditorGUILayout.FloatField("Waviness Frequency", Render.WavinessFrequency);
            GUILayout.EndVertical();
            
            //volume
            /*GUILayout.BeginVertical(EditorStyles.helpBox);
            Render.Volume = EditorGUILayout.FloatField("Volume", Render.Volume);
            GUILayout.EndVertical();*/

            GUILayout.EndVertical();
        }

        public HairRenderSettings Render
        {
            get { return settings.RenderSettings; }
        }
    }
}
