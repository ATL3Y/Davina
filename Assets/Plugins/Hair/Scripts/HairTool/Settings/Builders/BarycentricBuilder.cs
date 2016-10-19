using System.Collections.Generic;
using Scripts.HairTool.Settings.Builders.Abstract;
using UnityEngine;

namespace Scripts.HairTool.Settings.Builders
{
    public class BarycentricBuilder : HairBuilderBase
    {
        public ComputeBuffer Buffer { private set; get; }

        private List<Vector3> barycentricList;

        public override void BuildData(HairSettings settings)
        {
            barycentricList = new List<Vector3>();
            barycentricList.Add(new Vector3(1, 0, 0));
            barycentricList.Add(new Vector3(0, 1, 0));
            barycentricList.Add(new Vector3(0, 0, 1));

            //AddPointRecursive(barycentricList, new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1), 0);
            //Debug.Log(barycentricList.Count);
            //Debug.Log(Barycentric(new Vector3(0, 0, 0), aa, bb, cc));
            while (barycentricList.Count <= 64)
            {
                var k = GetRandomK();
                if(!barycentricList.Contains(k))
                    barycentricList.Add(GetRandomK());
            }
        }

        private void AddPointRecursive(List<Vector3> list, Vector3 a, Vector3 b, Vector3 c, int depth)
        {
            if(depth >= 5)
                return;

            var center = (a + b + c)/3;
            depth++;

            list.Add(center);
            AddPointRecursive(list, a, b, center, depth);
            AddPointRecursive(list, center, b, c, depth);
            AddPointRecursive(list, b, center, a, depth);
        }

        private Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {  
            Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;
            var v = (d11 * d20 - d01 * d21) / denom;
            var w = (d00 * d21 - d01 * d20) / denom;
            var u = 1.0f - v - w;
            return new Vector3(v,w,u);
        }

        public override void BuildCompute()
        {         
            Buffer = new ComputeBuffer(64, sizeof(float) * 3);
            Buffer.SetData(barycentricList.ToArray());
        }

        private Vector3 GetRandomK()
        {
            var ka = Random.Range(0f, 1f);
            var kb = Random.Range(0f, 1f);

            if (ka + kb > 1)
            {
                ka = 1 - ka;
                kb = 1 - kb;
            }

            var kc = 1 - (ka + kb);
            return new Vector3(ka, kb, kc);
        }

        public override void Destroy()
        {
            Buffer.Dispose();
        }
    }
}
