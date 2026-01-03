using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [Header("敵のデータベース"), SerializeField] EnemyDatabase enemy_DB;
    [Header("敵の元Prefab"), SerializeField] GameObject enemy_Prefab;
    [Header("戦闘時の囲いを制御するスクリプト"), SerializeField] WallController wallCtrl;

    public List<GameObject> CurrentEnemies { get; private set; } = new List<GameObject>();

    [Header("INFO")]
    [Tooltip("最初の敵の出現数"), SerializeField] int spawnAmount;
    [Tooltip("エリアの大きさ"), SerializeField] Vector2 fieldAreaSize;

    void Start()
    {
        SpawnEnemy(spawnAmount);
    }

    void Update()
    {
        if(Context.I.BattleStat == ENUM.BattleStat.Now
        && BattleFlowManager.I.BattleEnemies.Count <= 0)
        {
            BattleFlowManager.I.EndBattle();
            return;
        }
    }


    public void WallSpawnedSpawnEnemy(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Vector2 spawnAreaSize = wallCtrl.GetWall().SpawnArea;
            Spawn(RandomPosition(spawnAreaSize), SelectEnemyData());
        }
    }

    public void SpawnEnemy(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Spawn(RandomPosition(fieldAreaSize), SelectEnemyData());
        }
    }

    Vector2 RandomPosition(Vector2 spawnAreaSize)
    {
        Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0,
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
            ) + transform.position;

        return (Vector2)randomPosition;
    }

    EnemyData SelectEnemyData()
    {
        int index = Random.Range(0, enemy_DB.EnemyData.Length);
        return enemy_DB.EnemyData[index];
    }

    void Spawn(Vector2 pos, EnemyData data)
    {
        GameObject enemy = Instantiate(enemy_Prefab, pos, Quaternion.identity);
        enemy.GetComponent<EnemyController>().EnemyInitialize(data);
        enemy.GetComponent<EnemyMovement>().InitLaunchRoutine();

        CurrentEnemies.Add(enemy);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, fieldAreaSize);
    }

    public Transform[] GetEnemiesTransform()
    {
        Transform[] result = new Transform[CurrentEnemies.Count];
        for(int i = 0; i < CurrentEnemies.Count; i++)
        {
            result[i] = CurrentEnemies[i].transform;
        }

        return result;
    }

}