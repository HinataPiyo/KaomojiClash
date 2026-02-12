using UnityEngine;
using System.Collections.Generic;
using Constants;
using ENUM;

public class EnemySpawnController : MonoBehaviour
{
    [Header("敵の元Prefab"), SerializeField] GameObject enemy_Prefab;
    [Header("INFO")]
    [Tooltip("エリアの大きさ"), SerializeField] Vector2 fieldAreaSize;

    [Header("戦闘時の囲いを制御するスクリプト"), SerializeField] WallController wallCtrl;
    [Header("Waveを管理するスクリプト"), SerializeField] WaveController waveCtrl;
    [Header("CameraのTargetingを制御するスクリプト"), SerializeField] TargetGroupController targetGroupCtrl;

    public List<GameObject> CurrentEnemies { get; private set; } = new List<GameObject>();

    // 現在のエリアデータから敵リストを取得
    private List<EnemyData> currentAreaEnemies = new List<EnemyData>();

    public bool IsAllEnemyDefeated() => CurrentEnemies.Count == 0;

    void Start()
    {
        GenerateAreaEnemies();

        if (currentAreaEnemies.Count == 0)
        {
            Debug.LogError("敵が生成されていません!AreaDataまたはKaomojiPartsを確認してください。");
            return;
        }

        FirstSpawnEnemy();
    }

    /// <summary>
    /// 現在のエリアデータから敵を生成
    /// </summary>
    private void GenerateAreaEnemies()
    {
        if (AreaManager.I == null)
        {
            Debug.LogError("AreaManager.I is null! シーンにAreaManagerが配置されていません。");
            return;
        }

        if (AreaManager.I.CurrentAreaData == null)
        {
            Debug.LogError("CurrentAreaData is null! エリアが設定されていません。");
            return;
        }

        var areaData = AreaManager.I.CurrentAreaData;
        var spawnConfig = areaData.Build.spawnConfig;
        var partUnlockManager = areaData.Build.partUnlockManager; // 部位解放設定を取得
        int cultureLevel = areaData.Build.cultureLevel;

        Debug.Log($"=== 敵生成開始 ===");
        Debug.Log($"エリア: {areaData.AreaName}");
        Debug.Log($"文化圏レベル: {cultureLevel}");
        Debug.Log($"生成モード: {spawnConfig.mode}");

        currentAreaEnemies.Clear();

        // 難易度別に敵を生成
        foreach (var amountData in spawnConfig.spawnAmounts)
        {
            if (amountData.amount <= 0) continue;

            Debug.Log($"難易度 {amountData.difficulty} の敵を {amountData.amount} 体生成中...");

            var enemies = EnemyDataGenerator.GenerateEnemyList(
                spawnConfig,
                cultureLevel,
                amountData.difficulty,
                amountData.amount,
                partUnlockManager  // 部位解放設定を渡す
            );

            currentAreaEnemies.AddRange(enemies);

            Debug.Log($"  → {enemies.Count} 体生成完了");
        }

        Debug.Log($"=== 合計 {currentAreaEnemies.Count} 体の敵を生成 ===");
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
        if (currentAreaEnemies.Count == 0)
        {
            Debug.LogError("No enemies to spawn!");
            return;
        }

        var spawnConfig = AreaManager.I.CurrentAreaData.Build.spawnConfig;
        int cultureLevel = AreaManager.I.CurrentAreaData.Build.cultureLevel;

        // 難易度別に敵を配置
        foreach (var amountData in spawnConfig.spawnAmounts)
        {
            int spawnAmount = amountData.amount;
            Difficulty dif = amountData.difficulty;

            for (int i = 0; i < spawnAmount; i++)
            {
                EnemyData enemyData = SelectEnemyData(dif);
                if (enemyData == null) continue;

                GameObject e = Spawn(RandomPosition(Vector2.zero, fieldAreaSize), enemyData, dif);
                EnemyController eCtrl = e.GetComponent<EnemyController>();
                waveCtrl.CreateWaveData(eCtrl.EnemyData, dif);

                int avgLevel = AreaBuild.GetEnemyAverageLevelByWaveDifficulty(cultureLevel, dif);
                eCtrl.SetEnemyWorldUI(avgLevel, dif);
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
    /// 特定難易度の敵をランダム選択
    /// </summary>
    public EnemyData SelectEnemyData(Difficulty difficulty)
    {
        var availableEnemies = currentAreaEnemies.FindAll(e =>
        {
            // 難易度に応じた敵を選択（簡易実装）
            // TODO: より詳細な判定ロジックを追加
            return true;
        });

        if (availableEnemies.Count == 0)
        {
            Debug.LogWarning($"No enemies available for difficulty {difficulty}");
            return null;
        }

        int index = Random.Range(0, availableEnemies.Count);
        EnemyData copy = Instantiate(availableEnemies[index]);
        return copy;
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