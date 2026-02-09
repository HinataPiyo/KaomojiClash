using UnityEngine;
using ENUM;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Database/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [System.Serializable]
    public class AmountByDifficulty
    {
        public Difficulty difficulty;
        public int amount;
    }

    [SerializeField] EnemyData[] enemyDatas;
    [SerializeField] AmountByDifficulty[] amountByDifficulties = new AmountByDifficulty[]
    {
        new AmountByDifficulty { difficulty = Difficulty.Easy, amount = 2 },
        new AmountByDifficulty { difficulty = Difficulty.Normal, amount = 3 },
        new AmountByDifficulty { difficulty = Difficulty.Hard, amount = 2 },
        new AmountByDifficulty { difficulty = Difficulty.Extreme, amount = 1 },
    };

    public EnemyData[] GetAllEnemyData() => enemyDatas;
    public AmountByDifficulty[] GetAmountByDifficulties() => amountByDifficulties;
    public int GetTotalSpawnAmount() 
    {
        int total = 0;
        foreach(var item in amountByDifficulties)
        {
            total += item.amount;
        }
        return total;
    }
}