using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Geometry.Create;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Create.Inspector
{
    public class CreatorInputInspector : EditorItemBase
    {
        private HairGeometryCreator creator;

        public CreatorInputInspector(HairGeometryCreator creator)
        {
            this.creator = creator;
        }

        public override void DrawInspector()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Source Settings", EditorStyles.boldLabel);

            creator.Debug = EditorGUILayout.Toggle("Debug", creator.Debug);

            if (creator.Geomery.Selected == null)
            {
                creator.Segments = Mathf.Clamp(EditorGUILayout.IntField("Segments", creator.Segments), 3, 30);
            }
            else
            {
                GUILayout.Label("Segments " + creator.Segments);
            }
            

            creator.ScalpFilter = (MeshFilter)EditorGUILayout.ObjectField("Scalp", creator.ScalpFilter, typeof(MeshFilter), true);
            CollidersList();

            GUILayout.EndVertical();
        }

        private void CollidersList()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var providers = creator.ColliderProviders;

            for (var i = 0; i < providers.Count; i++)
            {
                GUILayout.BeginHorizontal();

                providers[i] =
                    (GameObject)
                        EditorGUILayout.ObjectField("Colliders Provider", providers[i], typeof(GameObject), true);

                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.MaxWidth(20)))
                {
                    providers.RemoveAt(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(providers.Count == 0 ? "Add Collider" : "");
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                providers.Add(null);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
