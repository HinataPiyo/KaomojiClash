using UnityEngine;
using Constants.Global;
using System.Collections;


public class WaveController : MonoBehaviour
{
    [SerializeField] EnemySpawnController enemySpawn;
    int waveCount;
    public bool IsWaving { get; private set; }



    public void WaveStart(Transform enemy)
    {
        StartCoroutine(WaveLoop(enemy));
    }

    IEnumerator WaveLoop(Transform enemy)
    {
        IsWaving = true;        // Wave開始
        EnemyData encountEnemy = enemy.GetComponent<EnemyController>().EnemyData;
        
        for(int ii = 0; ii < 3; ii++)
        {
            waveCount = ii;
            enemySpawn.SpawnEnemyInWall(waveCount, encountEnemy);
            Debug.Log("WaveLoop");
            yield return new WaitUntil(() => BattleFlowManager.I.NoneEnemy());
        }

        BattleFlowManager.I.EndBattle();        // 戦闘終了
        IsWaving = false;       // Wave終了
    }

    public void CreateWaveData(EnemyData firstEnemy, int firstEnemyLevel)
    {
        Wave w = new Wave();
        for(int ii = 0; ii < 3; ii++)
        {
            w.elements.Add(new Wave.Element());
        }

        foreach(Wave.Element elem in w.elements)
        {
            int r_EnemyAmount = Random.Range(1, 8);
            for(int ii = 0; ii < r_EnemyAmount; ii++)
            {
                int r_level = Random.Range(firstEnemyLevel - 2, firstEnemyLevel + 2);
                EnemyData select = enemySpawn.SelectEnemyData(r_level);
                elem.datas.Add(select);              
            }
        }

        firstEnemy.SetWaveData(w);
    }
}