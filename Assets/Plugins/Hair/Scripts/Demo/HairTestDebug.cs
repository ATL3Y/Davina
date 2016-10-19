using Scripts.HairTool.Physics;
using Scripts.HairTool.Settings;
using UnityEngine;

namespace Scripts.Demo
{
    [RequireComponent(typeof(HairSettings))]
    public class HairTestDebug : MonoBehaviour
    {
        private HairSettings settings;
        private GBody[] bodies;

        private void Start()
        {
            settings = GetComponent<HairSettings>();
            bodies = settings.Builder.Stands.Bodies;
        }

        private void OnDrawGizmos()
        {
            if(settings == null)
                return;

            DrawJoints();
            DrawStands();
        }

        private void DrawJoints()
        {
            Gizmos.color = Color.green;

            foreach (var joint in settings.Builder.Joints.Joints)
            {
                var body = settings.Builder.Stands.Bodies[joint.BodyId];
                Gizmos.DrawWireSphere(body.Position, 0.01f);
            }
        }

        private void DrawStands()
        {
            Gizmos.color = Color.red;

            settings.Builder.Stands.Buffer.GetData(bodies);

            for (int i = 0; i < bodies.Length; i++)
            {
                if (i % settings.StandsSettings.Segments == 0)
                    continue;

                var body1 = bodies[i - 1];
                var body2 = bodies[i];

                Gizmos.DrawLine(body1.Position, body2.Position);
            }
        }

        private Vector3 T(Vector3 p)
        {
            return settings.StandsSettings.Provider.transform.TransformPoint(p);

        }
    }
}
