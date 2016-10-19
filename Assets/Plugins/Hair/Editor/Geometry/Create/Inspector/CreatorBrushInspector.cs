using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Geometry.Create;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Create.Inspector
{
    public class CreatorBrushInspector : EditorItemBase
    {
        private HairGeometryCreator creator;

        public CreatorBrushInspector(HairGeometryCreator creator)
        {
            this.creator = creator;
        }

        public override void DrawInspector()
        {
            if (creator.Geomery.Selected == null)
                return;

            var brush = creator.Brush;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Brush Settings", EditorStyles.boldLabel);

            brush.Radius = EditorGUILayout.Slider("Radius", brush.Radius, 0, 3);
            brush.Strength = EditorGUILayout.Slider("Strength", brush.Strength, 0, 1);
            brush.CollisionDistance = EditorGUILayout.Slider("Collision Distance", brush.CollisionDistance, 0, 1);
            brush.Lenght1 = EditorGUILayout.Slider("Lenght Front", brush.Lenght1, 0, 3);
            brush.Lenght2 = EditorGUILayout.Slider("Lenght Back", brush.Lenght2, 0, 3);

            GUILayout.EndVertical();
        }
    }
}
