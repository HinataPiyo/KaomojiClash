using UnityEngine;
using Constants.Global;
using System.Collections;


public class WaveController : MonoBehaviour
{
    public static readonly float EnemyAmountIncreaseValue = 0.6f;
    [SerializeField] EnemySpawnController enemySpawn;
    [SerializeField] WaveDataUIControl waveDataCtrl;

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
        int maxWaveCount = encountEnemy.Wave.elements.Count;
        waveDataCtrl.SetWaveData(encountEnemy.Wave, encountEnemy.E_Status.GetLevel());

        for(int ii = 0; ii < maxWaveCount; ii++)
        {
            waveCount = ii;
            waveDataCtrl.SetWaveCountText(waveCount);
            enemySpawn.SpawnEnemyInWall(waveCount, encountEnemy);       // 敵を生成したのち
            waveDataCtrl.SetEnemyCountText(BattleFlowManager.I.BattleEnemies.Count);    // UIに反映
            yield return new WaitUntil(() => BattleFlowManager.I.NoneEnemy());
        }

        BattleFlowManager.I.EndBattle();        // 戦闘終了
        waveCount = 0;
        IsWaving = false;       // Wave終了
    }

    /// <summary>
    /// WaveDataを作成しEnemyDataに保持させておく
    /// ※ EnemyDataは敵生成時にCopyされている(元のSOには干渉しない)
    /// </summary>
    /// <param name="firstEnemy">エンカウントした敵の情報</param>
    /// <param name="firstEnemyLevel">(現)敵のレベル（難易度）は生成時に決まるので引数で受け取る</param>
    public void CreateWaveData(EnemyData firstEnemy, ENUM.Difficulty difficulty)
    {
        Wave w = new Wave();
        w.difficulty = difficulty;
        int waveCount = 0;
        int dif_Amount = GetOneWaveEnemyAmount(w.difficulty);     // 難易度に応じて敵の量を設定

        // 現在 3Wave分作成
        for(int ii = 0; ii < 3; ii++)
        {
            w.elements.Add(new Wave.Element());
        }

        // 作成したWaveDataからさらに、敵情報を盛り込んでいく
        foreach(Wave.Element elem in w.elements)
        {
            // 1Waveごとの設定
            float increase = EnemyAmountIncreaseValue * waveCount;
            int enemyAmount = Mathf.CeilToInt(dif_Amount * (increase + 1));
            for(int ii = 0; ii < enemyAmount; ii++)
            {
                // 難易度に応じて敵のレベルが決まる
                EnemyData select = enemySpawn.SelectEnemyData(w.difficulty);
                elem.datas.Add(select);              
            }

            waveCount++;
        }

        // WaveDataを作成し終わったらエンカウントした敵にWaveDataを再度Setする
        firstEnemy.SetWaveData(w);
    }

    /// <summary>
    /// 1Waveごとの敵の数
    /// </summary>
    /// <param name="difficulty">難易度</param>
    int GetOneWaveEnemyAmount(ENUM.Difficulty difficulty)
    {
        bool isMany = Random.Range(0, 2) == 1;
        switch(difficulty)
        {
            case ENUM.Difficulty.Easy:
                return isMany ? 1 : 2;
            case ENUM.Difficulty.Normal:
                return isMany ? 2 : 3;
            case ENUM.Difficulty.Hard:
                return isMany ? 3 : 4;;
            case ENUM.Difficulty.Extreme:
                return isMany ? 4 : 5;
            default:
                return 0;
        }
    }
}