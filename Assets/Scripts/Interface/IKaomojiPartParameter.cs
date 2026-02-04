public interface IKaomojiPartParameter
{
    public float GetParameterByLevel(int level);
    public ENUM.GrowthRateType GrowthRateType { get; }
    public float GetInitialParam();
}