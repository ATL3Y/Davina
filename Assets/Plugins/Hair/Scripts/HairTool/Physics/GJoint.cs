namespace Scripts.HairTool.Physics
{
    public struct GJoint
    {
        public int BodyId;
        public float Pover;

        public GJoint(int bodyId, float pover)
        {
            BodyId = bodyId;
            Pover = pover;
        }

        public static int GetSizeBytes()
        {
            return sizeof(int) + sizeof(float);
        }
    }
}
