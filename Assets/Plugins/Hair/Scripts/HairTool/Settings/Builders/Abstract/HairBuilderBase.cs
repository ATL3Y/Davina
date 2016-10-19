namespace Scripts.HairTool.Settings.Builders.Abstract
{
    public abstract class HairBuilderBase
    {
        public abstract void BuildData(HairSettings settings);
        public abstract void Destroy();
        public virtual void BuildCompute() { }
        public virtual void UpdateCompute() { }
    }
}
