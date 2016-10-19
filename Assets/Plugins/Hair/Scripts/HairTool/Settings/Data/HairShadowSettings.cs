using System;
using Scripts.HairTool.Settings.Data.Abstract;
using UnityEngine;

namespace Scripts.HairTool.Settings.Data
{
    /// <summary>
    /// Hair Shadow settings
    /// </summary>
    [Serializable]
    public class HairShadowSettings : HairSettingsBase
    {
        public bool CastShadows;
        public Material Material;
    }
}
