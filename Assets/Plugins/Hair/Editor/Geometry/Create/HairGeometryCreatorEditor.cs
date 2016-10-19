using System.Collections.Generic;
using Assets.Hair.Editor.Engine;
using Assets.Hair.Editor.Geometry.Create.Inspector;
using Assets.Hair.Editor.Geometry.Create.Scene;
using Scripts.HairTool.Geometry.Create;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Create
{
    [CustomEditor(typeof(HairGeometryCreator))]
    public class HairGeometryCreatorEditor : UnityEditor.Editor
    {
        private Dictionary<KeyCode, CreatorBaseBrush> brushes = new Dictionary<KeyCode, CreatorBaseBrush>();
        private Processor processor = new Processor();
        private HairGeometryCreator creator;
        private EditorInput input = new EditorInput();

        private KeyCode prevKey;

        private void OnEnable()
        {
            creator = target as HairGeometryCreator;

            processor.Add(new CreatorInputInspector(creator));
            processor.Add(new CreatorGroupInspector(creator));
            processor.Add(new CreatorBrushInspector(creator));
            processor.Add(new CreatorBrushView(creator));

            brushes.Add(KeyCode.M, new CreatorMoveBrush(creator));
            brushes.Add(KeyCode.R, new CreatorRemoveBrush(creator));
            brushes.Add(KeyCode.G, new CreatorShrinkBrush(creator, 0.1f));
            brushes.Add(KeyCode.S, new CreatorShrinkBrush(creator, -0.1f));
        }

        public override void OnInspectorGUI()
        {
            processor.DrawInspector();
        }

        private void OnSceneGUI()
        {
            input.Update();
            processor.DrawScene();

            if (creator.Geomery.Selected == null)
                return;

            foreach (var pair in brushes)
            {
                if (input.IsKey(pair.Key))
                {
                    if(pair.Key != prevKey)
                        pair.Value.StartDrawScene();

                    pair.Value.DrawScene();
                    prevKey = pair.Key;
                    break;
                }

                prevKey = KeyCode.A;
            }

            

            /*EditorGUI.BeginChangeCheck();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Area Of Effect");
            }*/
        }
    }
}
