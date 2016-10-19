using Scripts.HairTool.Geometry.Import;
using Scripts.HairTool.Settings;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Import
{
    [CustomEditor(typeof(GeometryProvider))]
    public class GeometryProviderEditor : UnityEditor.Editor
    {
        private GeometryProvider settings;

        private void OnEnable()
        {
            settings = target as GeometryProvider;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                settings.Process();
            }
        }

        private void OnSceneGUI()
        {

        }
    }
}
