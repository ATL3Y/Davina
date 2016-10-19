using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Settings;
using Scripts.HairTool.Settings.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Settings.Inspector
{
    public class HairLODInspector : EditorItemBase
    {
        private HairSettings settings;

        public HairLODInspector(HairSettings settings)
        {
            this.settings = settings;
        }

        public override void DrawInspector()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("LOD Settings", EditorStyles.boldLabel);

            Lod.StartDistance = EditorGUILayout.FloatField("Start Distance", Lod.StartDistance);
            Lod.EndDistance = EditorGUILayout.FloatField("End Distance", Lod.EndDistance);

            Lod.DensityMin = EditorGUILayout.FloatField("Min Dencity", Lod.DensityMin);
            Lod.DensityMax = EditorGUILayout.FloatField("Max Dencity", Lod.DensityMax);

            Lod.DetailMin = EditorGUILayout.FloatField("Min Detail", Lod.DetailMin);
            Lod.DetailMax = EditorGUILayout.FloatField("Max Detail", Lod.DetailMax);

            Lod.WidthMin = EditorGUILayout.FloatField("Min Width", Lod.WidthMin);
            Lod.WidthMax = EditorGUILayout.FloatField("Max Width", Lod.WidthMax);

            GUILayout.EndVertical();
        }

        public HairLODSettings Lod
        {
            get { return settings.LODSettings; }
        }
    }
}
