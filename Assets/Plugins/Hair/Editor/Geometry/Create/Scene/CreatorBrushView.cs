using System.Collections.Generic;
using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Geometry.Create;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Geometry.Create.Scene
{
    public class CreatorBrushView : EditorItemBase
    {
        protected HairGeometryCreator Creator;

        public CreatorBrushView(HairGeometryCreator creator)
        {
            Creator = creator;
        }

        public override void DrawScene()
        {
            var ray = Camera.current.ScreenPointToRay(GetMousePos());
            var distanceToCamera = Camera.current.transform.InverseTransformPoint(Creator.transform.position).z;

            Creator.Brush.Position = ray.GetPoint(distanceToCamera);
            Creator.Brush.Dirrection = Camera.current.transform.TransformDirection(Vector3.forward);

            Creator.Brush.Draw();
            EditorUtility.SetDirty(Creator);
        }

        private Vector2 GetMousePos()
        {
            var mousePos = Event.current.mousePosition;
            mousePos.y = Camera.current.pixelHeight - mousePos.y;
            return mousePos;
        }
    }
}
