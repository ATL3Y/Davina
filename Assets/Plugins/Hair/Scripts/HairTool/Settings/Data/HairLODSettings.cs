using System;
using Scripts.HairTool.Settings.Data.Abstract;
using UnityEngine;

namespace Scripts.HairTool.Settings.Data
{
    /// <summary>
    /// Level of detail settings
    /// </summary>
    [Serializable]
    public class HairLODSettings : HairSettingsBase
    {
        public float DensityMin = 8;
        public float DensityMax = 32;
        public float DetailMin = 16;
        public float DetailMax = 64;
        public float WidthMin = 0.004f;
        public float WidthMax = 0.02f;
        public float StartDistance = 20;
        public float EndDistance = 60;

        public float GetWidth(Vector3 position)
        {
            return Mathf.Lerp(WidthMax, WidthMin, 1 - GetDistanceK(position));
        }

        public int GetDencity(Vector3 position)
        {
            return (int)Mathf.Lerp(DensityMax, DensityMin, GetDistanceK(position));
        }

        public int GetDetail(Vector3 position)
        {
            return (int)Mathf.Lerp(DetailMax, DetailMin, GetDistanceK(position));
        }

        public float GetDistanceK(Vector3 position)
        {
            var k = (GetDistanceToCamera(position) - StartDistance) /(EndDistance - StartDistance);

            return Mathf.Clamp(k, 0, 1);
        }

        public float GetDistanceToCamera(Vector3 position)
        {
            return (position - Camera.main.transform.position).magnitude;
        }
    }
}
