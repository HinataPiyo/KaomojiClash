using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] Vector2 fieldAreaSize;
    [SerializeField] EnemyDatabase enemy_DB;
    [SerializeField] GameObject enemy_Prefab;
    [SerializeField] WallController wallCtrl;

    List<GameObject> currentEnemies = new List<GameObject>();


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

        currentEnemies.Add(enemy);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, fieldAreaSize);
    }

}