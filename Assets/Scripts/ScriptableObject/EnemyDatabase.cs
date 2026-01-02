using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField] EnemyData[] enemyDatas;
    public EnemyData[] EnemyData => enemyDatas;
}