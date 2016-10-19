using Assets.Hair.Editor.Engine;
using Scripts.HairTool.Settings;
using Scripts.HairTool.Settings.Data;
using UnityEditor;
using UnityEngine;

namespace Assets.Hair.Editor.Settings.Scene
{
    public class HairStandsEditor : EditorItemBase
    {
        private readonly HairSettings settings;

        private Vector3 cachedHeadCenter;

        public HairStandsEditor(HairSettings settings)
        {
            this.settings = settings;
        }

        public override void DrawScene()
        {
            Handles.color = Color.green;
            Handles.SphereCap(0, StandsSettings.HeadCenterWorld, Quaternion.identity, 0.1f);
            
            if (cachedHeadCenter != StandsSettings.HeadCenterWorld)
            {
                cachedHeadCenter = StandsSettings.HeadCenterWorld;
                EditorUtility.SetDirty(StandsSettings.Provider);
            }

            
        }

        public HairStandsSettings StandsSettings
        {
            get { return settings.StandsSettings; }
        }
    }
}
