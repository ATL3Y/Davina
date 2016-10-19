using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Settings;
using Scripts.HairTool.Settings.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Settings.Inspector
{
    public class HairPhysicsInspector : EditorItemBase
    {
        private HairSettings settings;

        public HairPhysicsInspector(HairSettings settings)
        {
            this.settings = settings;
        }

        public override void DrawInspector()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Physics Settings", EditorStyles.boldLabel);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            Physics.Shader = (ComputeShader)EditorGUILayout.ObjectField("Shader", Physics.Shader, typeof(ComputeShader), true);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            Physics.Gravity = EditorGUILayout.FloatField("Gravity", Physics.Gravity);
            Physics.Drag = Mathf.Clamp(EditorGUILayout.FloatField("Drag", Physics.Drag), 0, 1);
            Physics.StandRadius = Mathf.Clamp(EditorGUILayout.FloatField("Stand Radius", Physics.StandRadius), 0, 1);
            Physics.ElasticyRoot = Mathf.Clamp(EditorGUILayout.FloatField("Elasticity Root", Physics.ElasticyRoot), 0,1);
            Physics.ElasticyTip = Mathf.Clamp(EditorGUILayout.FloatField("Elasticity Tip", Physics.ElasticyTip), 0,1);
            GUILayout.EndVertical();

            CollidersList();
            JointsList();

            GUILayout.EndVertical();
        }

        private void CollidersList()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var providers = Physics.ColliderProviders;
            if (Physics.ColliderProviders != null)
            {
                for (int i = 0; i < providers.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    providers[i] =
                        (GameObject)
                            EditorGUILayout.ObjectField("Colliders Provider", providers[i], typeof (GameObject), true);

                    if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.MaxWidth(20)))
                    {
                        providers.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(providers == null || providers.Count == 0 ? "Add Collider" : "");
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                providers.Add(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public void JointsList() 
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var joints = Physics.JointAreas;
            if (Physics.ColliderProviders != null)
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    joints[i] =
                        (HairJointArea)
                            EditorGUILayout.ObjectField("Joint Area", joints[i], typeof(HairJointArea), true);

                    if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.MaxWidth(20)))
                    {
                        joints.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(joints == null || joints.Count == 0 ? "Add Joint Area" : "");
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                joints.Add(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        public HairPhysicsSettings Physics
        {
            get { return settings.PhysicsSettings; }
        }
    }
}
