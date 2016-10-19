using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Settings;
using Scripts.HairTool.Settings.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Settings.Inspector
{
    public class HairShadowInspector : EditorItemBase
    {
        private HairSettings settings;

        public HairShadowInspector(HairSettings settings)
        {
            this.settings = settings;
        }

        public override void DrawInspector()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Shadow Settings", EditorStyles.boldLabel);

            Shadow.CastShadows = EditorGUILayout.Toggle("Cast Shadows", Shadow.CastShadows);

            if(Shadow.CastShadows)
                Shadow.Material = (Material)EditorGUILayout.ObjectField("Material", Shadow.Material, typeof(Material), true);
            
            GUILayout.EndVertical();
        }

        public HairShadowSettings Shadow
        {
            get { return settings.ShadowSettings; }
        }
    }
}
