﻿using UnityEngine;
#pragma warning disable 649

namespace Scripts.HairTool.Settings
{
    public class HairJointArea : MonoBehaviour
    {
        [SerializeField] private float radius;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public float Radius
        {
            get { return radius; }
        }
    }
}
