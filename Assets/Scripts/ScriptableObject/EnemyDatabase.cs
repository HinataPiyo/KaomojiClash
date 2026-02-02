using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Database/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField] EnemyData[] enemyDatas;
    public EnemyData[] GetAllEnemyData() => enemyDatas;
}