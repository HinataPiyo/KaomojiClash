using System.Collections.Generic;
using UnityEngine;
using Constants.Global;

public class EnemySpawnController : MonoBehaviour
{
    [Header("敵のデータベース"), SerializeField] EnemyDatabase enemy_DB;
    [Header("敵の元Prefab"), SerializeField] GameObject enemy_Prefab;
    [Header("戦闘時の囲いを制御するスクリプト"), SerializeField] WallController wallCtrl;
    [Header("Waveを管理するスクリプト"), SerializeField] WaveController waveCtrl;
    [Header("CameraのTargetingを制御するスクリプト"), SerializeField] TargetGroupController targetGroupCtrl;

    public List<GameObject> CurrentEnemies { get; private set; } = new List<GameObject>();

    [Header("INFO")]
    [Tooltip("最初の敵の出現数"), SerializeField] int spawnAmount;
    [Tooltip("エリアの大きさ"), SerializeField] Vector2 fieldAreaSize;

    void Start()
    {
        // AreaManagerがない場合はSerialideFieldで設定したDBを使用
        if(AreaManager.I != null) enemy_DB = AreaManager.I.CurrentAreaData.AreaBuild.spawnDatabase;
        FirstSpawnEnemy(spawnAmount);
    }

    /// <summary>
    /// 壁の内側に敵を生成させる
    /// </summary>
    /// <param name="count">生成量</param>
    public void SpawnEnemyInWall(int waveCount, EnemyData firstEnemyData)
    {
        // 現在生成されている壁から生成範囲を取得
        Wall wall = wallCtrl.GetWall();
        Vector2 spawnAreaSize = wall.SpawnArea;

        // 今のところ敵はランダム抽選
        foreach(EnemyData data in firstEnemyData.Wave.elements[waveCount].datas)
        {
            // WaveControllerでFirstEnemyに入れたWaveDataの中にあるEnemyDataをSpawn()関数に入れ実行
            GameObject e = Spawn(RandomPosition(wall.CenterPosition, spawnAreaSize), data);
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
    public void FirstSpawnEnemy(int count)
    {
        for(int i = 0; i < count; i++)
        {
            ENUM.Difficulty dif = (ENUM.Difficulty)Random.Range(0, (int)ENUM.Difficulty.Max);
            GameObject e = Spawn(RandomPosition(Vector2.zero, fieldAreaSize), SelectEnemyData(dif));
            EnemyController eCtrl = e.GetComponent<EnemyController>();
            waveCtrl.CreateWaveData(eCtrl.EnemyData, dif);
            eCtrl.SetEnemyWorldUI(eCtrl.EnemyData.E_Status.GetLevel(), dif);
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
    public EnemyData SelectEnemyData(ENUM.Difficulty difficulty)
    {
        int index = Random.Range(0, enemy_DB.EnemyData.Length);
        int level = Calculation.GetLevelByDifficulty(difficulty);
        float exp = Calculation.GetExperienceByDifficultyAndLevel(difficulty, level);
        EnemyData copy = Instantiate(enemy_DB.EnemyData[index]);
        copy.E_Status.SetLevel(level);
        copy.E_Status.SetExperience(exp);
        return copy;
    }

    /// <summary>
    /// 敵の生成処理
    /// </summary>
    /// <param name="pos">出現位置</param>
    /// <param name="data">敵のデータ</param>
    GameObject Spawn(Vector2 pos, EnemyData data)
    {
        GameObject enemy = Instantiate(enemy_Prefab, pos, Quaternion.identity);
        enemy.GetComponent<EnemyController>().EnemyInitialize(data);
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