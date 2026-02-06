using System.Collections.Generic;
using ENUM;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [Header("敵のデータベース"), SerializeField] EnemyDatabase enemy_DB;
    [Header("敵の元Prefab"), SerializeField] GameObject enemy_Prefab;

    [Header("INFO")]
    [Tooltip("エリアの大きさ"), SerializeField] Vector2 fieldAreaSize;

    [Header("戦闘時の囲いを制御するスクリプト"), SerializeField] WallController wallCtrl;
    [Header("Waveを管理するスクリプト"), SerializeField] WaveController waveCtrl;
    [Header("CameraのTargetingを制御するスクリプト"), SerializeField] TargetGroupController targetGroupCtrl;
    public List<GameObject> CurrentEnemies { get; private set; } = new List<GameObject>();

    public bool IsAllEnemyDefeated() => CurrentEnemies.Count == 0;


    void Start()
    {
        // AreaManagerがない場合はSerialideFieldで設定したDBを使用
        if(AreaManager.I != null) enemy_DB = AreaManager.I.CurrentAreaData.Build.spawnDatabase;
        FirstSpawnEnemy();
    }

    /// <summary>
    /// 壁の内側に敵を生成させる
    /// </summary>
    /// <param name="count">生成量</param>
    public void SpawnEnemyInWall(int waveCount, EnemyData firstEnemyData, Difficulty dif)
    {
        // 現在生成されている壁から生成範囲を取得
        Wall wall = wallCtrl.GetWall();
        Vector2 spawnAreaSize = wall.SpawnArea;

        // 今のところ敵はランダム抽選
        foreach(EnemyData data in firstEnemyData.Wave.elements[waveCount].datas)
        {
            // WaveControllerでFirstEnemyに入れたWaveDataの中にあるEnemyDataをSpawn()関数に入れ実行
            GameObject e = Spawn(RandomPosition(wall.CenterPosition, spawnAreaSize), data, dif);
            BattleFlowManager.I.BattleEnemies.Add(e.transform);
            targetGroupCtrl.AddTarget(e.transform);
        }

        BattleFlowManager.I.OnBattleEnemies();
    }

    /// <summary>
    /// 一番最初の敵を生成する関数
    /// 生成後Waveデータを作成
    /// </summary>
    /// <param name="count">生成量</param>
    public void FirstSpawnEnemy()
    {
        for(int q = 0; q < enemy_DB.GetAmountByDifficulties().Length; q++)
        {
            int spawnAmount = enemy_DB.GetAmountByDifficulties()[q].amount;
            for(int i = 0; i < spawnAmount; i++)
            {
                Difficulty dif = enemy_DB.GetAmountByDifficulties()[q].difficulty;
                GameObject e = Spawn(RandomPosition(Vector2.zero, fieldAreaSize), SelectEnemyData(), dif);
                EnemyController eCtrl = e.GetComponent<EnemyController>();
                waveCtrl.CreateWaveData(eCtrl.EnemyData, dif);
                int avgLevel = Constants.AreaBuild.GetEnemyAverageLevelByWaveDifficulty(AreaManager.I.CurrentAreaData.Build.cultureLevel, dif);
                eCtrl.SetEnemyWorldUI(avgLevel, dif);       // 最初の敵のUIを設定（平均レベルと難易度）
            }
        }
    }

    /// <summary>
    /// 設定したエリア内でランダムな位置を取得する
    /// </summary>
    /// <param name="spawnAreaSize"></param>
    /// <returns></returns>

    Vector2 RandomPosition(Vector2 center, Vector2 spawnAreaSize)
    {
        Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                0
            ) + (Vector3)center;

        return (Vector2)randomPosition;
    }

    /// <summary>
    /// 敵のデータベースからランダムに抽選する
    /// </summary>
    public EnemyData SelectEnemyData()
    {
        int index = Random.Range(0, enemy_DB.GetAllEnemyData().Length);
        EnemyData copy = Instantiate(enemy_DB.GetAllEnemyData()[index]);
        return copy;
    }

    /// <summary>
    /// 敵の生成処理
    /// </summary>
    /// <param name="pos">出現位置</param>
    /// <param name="data">敵のデータ</param>
    GameObject Spawn(Vector2 pos, EnemyData data, Difficulty dif)
    {
        GameObject enemy = Instantiate(enemy_Prefab, pos, Quaternion.identity);
        enemy.GetComponent<EnemyController>().EnemyInitialize(data, dif);
        enemy.GetComponent<EnemyMovement>().InitLaunchRoutine();

        CurrentEnemies.Add(enemy);

        return enemy;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, fieldAreaSize);
    }
}