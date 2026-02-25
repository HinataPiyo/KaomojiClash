using UnityEngine;
using System.Collections.Generic;
using Constants;
using ENUM;
using System.Linq;
using UI.Battle;

public class EnemySpawnController : MonoBehaviour
{
    const float SPAWN_WIDTH = 25f;     // 敵のスポーン間隔
    [Header("敵の元Prefab"), SerializeField] GameObject enemy_Prefab;
    [Header("INFO")]
    [Tooltip("エリアの大きさ"), SerializeField] Vector2 fieldAreaSize;

    [Header("戦闘時の囲いを制御するスクリプト"), SerializeField] WallController wallCtrl;
    [Header("Waveを管理するスクリプト"), SerializeField] WaveController waveCtrl;
    [Header("CameraのTargetingを制御するスクリプト"), SerializeField] TargetGroupController targetGroupCtrl;
    [SerializeField] BattleModulesController battleModulesCtrl;

    public List<GameObject> CurrentEnemies { get; private set; } = new List<GameObject>();

    // 現在のエリアデータから敵リストを取得
    private List<EnemyData> currentAreaEnemies = new List<EnemyData>();
    public List<EnemyData> FirstEnemiesData { get; private set; } = new List<EnemyData>();

    public bool IsAllEnemyDefeated() => CurrentEnemies.Count == 0;

    void Start()
    {
        GenerateAreaEnemies();
        FirstSpawnEnemy();
    }

    /// <summary>
    /// 現在のエリアデータから敵を生成
    /// </summary>
    private void GenerateAreaEnemies()
    {
        var areaData = AreaManager.I.CurrentAreaData;
        var spawnConfig = areaData.Build.spawnConfig;
        int cultureLevel = areaData.Build.cultureLevel;

        currentAreaEnemies.Clear();

        // 敵リストから有効な敵を取得
        var validEnemies = spawnConfig.fixedEnemies.Where(e => e != null).ToList();

        if (validEnemies.Count == 0)
        {
            Debug.LogError("有効な敵が設定されていません！AreaDataに敵を設定してください。");
            return;
        }

        // 難易度別に敵を生成（リストからランダムに選択）
        foreach (var amountData in spawnConfig.spawnAmounts)
        {
            if (amountData.amount <= 0) continue;

            for (int i = 0; i < amountData.amount; i++)
            {
                // 敵リストからランダムに1体選択
                EnemyData selectedEnemy = validEnemies[Random.Range(0, validEnemies.Count)];
                currentAreaEnemies.Add(selectedEnemy);
            }
        }
    }

    /// <summary>
    /// 壁の内側に敵を生成させる
    /// </summary>
    public void SpawnEnemyInWall(int waveCount, EnemyData firstEnemyData, Difficulty dif)
    {
        Wall wall = wallCtrl.GetWall();
        Vector2 spawnAreaSize = wall.SpawnArea;

        foreach (EnemyData data in firstEnemyData.Wave.elements[waveCount].datas)
        {
            GameObject e = Spawn(RandomPosition(wall.CenterPosition, spawnAreaSize), data, dif);
            BattleFlowManager.I.BattleEnemies.Add(e.transform);
            targetGroupCtrl.AddTarget(e.transform);
        }

        BattleFlowManager.I.OnBattleEnemies();
    }

    /// <summary>
    /// 一番最初の敵を生成する関数
    /// </summary>
    public void FirstSpawnEnemy()
    {
        EnemySpawnConfig spawnConfig = AreaManager.I.CurrentAreaData.Build.spawnConfig;
        int cultureLevel = AreaManager.I.CurrentAreaData.Build.cultureLevel;

        // StageProgress の「最後判定」に使うため、先に全体の生成数を集計しておく
        int totalSpawnCount = 0;
        foreach (DifficultySpawnAmount amountData in spawnConfig.spawnAmounts)
        {
            totalSpawnCount += amountData.amount;
        }

        int spawnCount = 0;
        // 難易度別に敵を配置
        foreach (DifficultySpawnAmount amountData in spawnConfig.spawnAmounts)
        {
            int spawnAmount = amountData.amount;
            Difficulty dif = amountData.difficulty;
            Debug.Log($"SpawnEnemyInWall: {spawnAmount} enemies for difficulty {dif}");

            for (int i = 0; i < spawnAmount; i++)
            {
                EnemyData enemyData = SelectEnemyData();
                if (enemyData == null) continue;
                FirstEnemiesData.Add(enemyData);     // 最初の敵のデータを保存

                // 最後の1体だけ true。終端アイコン生成時は line を伸ばさないために使う
                bool isLastSpawn = spawnCount == totalSpawnCount - 1;
                battleModulesCtrl.module_SP.CreateStageProgressIcon(isLastSpawn, spawnCount, dif);     // ステージ進行UIにアイコンを追加

                Vector2 spawnPos = new Vector2(SPAWN_WIDTH * (spawnCount + 1), 0);       // 直線状に設置されるように修正
                GameObject e = Spawn(spawnPos, enemyData, dif);
                EnemyController eCtrl = e.GetComponent<EnemyController>();
                waveCtrl.CreateWaveData(eCtrl.EnemyData, dif);

                int avgLevel = AreaBuild.GetEnemyAverageLevelByWaveDifficulty(cultureLevel, dif);
                eCtrl.SetEnemyWorldUI(avgLevel, dif);
                spawnCount++;
            }
        }
    }

    /// <summary>
    /// ランダムな位置を取得
    /// </summary>
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
    /// 全敵からランダム選択（互換性のため残す）
    /// </summary>
    public EnemyData SelectEnemyData()
    {
        if (currentAreaEnemies.Count == 0) return null;

        int index = Random.Range(0, currentAreaEnemies.Count);
        EnemyData copy = Instantiate(currentAreaEnemies[index]);
        return copy;
    }

    /// <summary>
    /// 敵の生成処理
    /// </summary>
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