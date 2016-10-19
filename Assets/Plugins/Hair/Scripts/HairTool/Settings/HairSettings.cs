using Scripts.HairTool.Settings.Builders;
using Scripts.HairTool.Settings.Data;
using UnityEngine;

namespace Scripts.HairTool.Settings
{
    /// <summary>
    /// This class is access point to all hair settings. 
    /// Hair settings consist of one or multiple hair group settings
    /// On start it creates game object (to render hair) for each group settings
    /// </summary>
    public class HairSettings : MonoBehaviour
    {
        public HairStandsSettings StandsSettings;
        public HairPhysicsSettings PhysicsSettings;
        public HairRenderSettings RenderSettings;
        public HairShadowSettings ShadowSettings;
        public HairLODSettings LODSettings;

        public HairBuilder Builder;
     
        public void Start()
        {
            Validate();

            Builder = new HairBuilder();
            Builder.Build(this);
        }

        private void Validate()
        {
            StandsSettings.Validate();
            PhysicsSettings.Validate();
            RenderSettings.Validate();
            ShadowSettings.Validate();
            LODSettings.Validate();
        }

        private void OnDestroy()
        {
            if(Builder != null)
                Builder.Destroy();
        }
    }
}
