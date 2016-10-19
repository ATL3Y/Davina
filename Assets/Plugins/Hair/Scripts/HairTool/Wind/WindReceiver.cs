using UnityEngine;

namespace Scripts.HairTool.Wind
{
    public class WindReceiver : MonoBehaviour
    {
        public Vector3 Vector { get; set; }

        private WindZone[] winds;
        private float angle;

        private void Awake()
        {
            winds = FindObjectsOfType<WindZone>();
        }

        public Vector3 GetWind(Vector3 position)
        {
            Vector = Vector3.zero;

            foreach (var windZone in winds)
            {
                if (windZone.mode == WindZoneMode.Directional)
                {
                    UpdateDirectionalWind(windZone);
                }
                else
                {
                    UpdateSphericalWind(windZone, position);
                }
            }

            return Vector;
        }

        private void UpdateDirectionalWind(WindZone wind)
        {
            angle += wind.windPulseFrequency;

            var cos = Mathf.Cos(angle)*Mathf.Cos(angle*3)*Mathf.Cos(angle*5)*Mathf.Cos(angle*7)*Mathf.Cos(angle*25);
            var amplitude = wind.windMain + cos*wind.windPulseMagnitude;
            var direction = wind.transform.rotation*Vector3.forward;

            Vector += direction*amplitude;
        }

        private void UpdateSphericalWind(WindZone wind, Vector3 center)
        {
            var diff = center - wind.transform.position;

            if(diff.magnitude > wind.radius)
                return;

            angle += wind.windPulseFrequency;
            
            var cos = Mathf.Cos(angle)*Mathf.Cos(angle*3)*Mathf.Cos(angle*5)*Mathf.Cos(angle*7)*Mathf.Cos(angle*25);
            var amplitude = wind.windMain + cos*wind.windPulseMagnitude;
            
            Vector += diff.normalized*amplitude;
        }
    }
}
