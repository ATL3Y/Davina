using System;
using Scripts.HairTool.Settings.Data.Abstract;
using UnityEngine;
using UnityEngine.Assertions;

namespace Scripts.HairTool.Settings.Data
{
    [Serializable]
    public class HairRenderSettings : HairSettingsBase
    {
        public Material HairMaterial;

        //color
        public Color RootColor = new Color(0.35f, 0.15f, 0.15f);
        public Color TipColor = new Color(0.15f, 0.05f, 0.05f);
        public float ColorBlend = 0.5f;

        //specular
        public float PrimarySpecular = 50;
        public float SecondarySpecular = 50;
        public Color SpecularColor = new Color(0.15f, 0.15f, 0.15f);

        //lenght
        public float Length1 = 1;
        public float Length2 = 1;
        public float Length3 = 1;

        //waviness
        public float WavinessScale = 0;
        public float WavinessFrequency = 2;

        //interpoation
        public float Interpolation = 1;

        //volume 
        public float Volume = 0;

        public override void Validate()
        {
            Assert.IsNotNull(HairMaterial, "Add material to render settings");
        }
    }
}
