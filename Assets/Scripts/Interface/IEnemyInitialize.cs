using ENUM;

public interface IEnemyInitialize
{
    Difficulty dif { get; }
    void EnemyInitialize(EnemyData data, Difficulty dif);
}