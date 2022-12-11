namespace UnityEngine.PostProcessing
{
    public sealed class Min_AttributeAttribute : PropertyAttribute
    {
        public readonly float min;

        public Min_AttributeAttribute(float min)
        {
            this.min = min;
        }
    }
}
