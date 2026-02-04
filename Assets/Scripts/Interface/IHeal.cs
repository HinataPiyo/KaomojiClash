interface IHeal
{
    float GetMaxHealthAmount { get; }
    void Heal(float amount);
}