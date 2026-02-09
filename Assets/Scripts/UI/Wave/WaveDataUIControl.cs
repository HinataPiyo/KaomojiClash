using Constants.Global;
using TMPro;
using UnityEngine;

public class WaveDataUIControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dif;
    [SerializeField] TextMeshProUGUI averageLevel;
    [SerializeField] TextMeshProUGUI waveCount;
    [SerializeField] TextMeshProUGUI enemyCount;

    /// <summary>
    /// WaveDataをUIに反映する
    /// </summary>
    /// <param name="wave">WaveData</param>
    /// <param name="cultureLevel">文化圏レベル</param>
    /// <param name="cultureMultiplier">ステータスを強化する倍率</param>
    /// <param name="avgLevel">平均レベル</param>
    public void SetWaveData(Wave wave, int avgLevel)
    {
        SetDifficultyText(wave.difficulty);     // 難易度
        SetLevelText(avgLevel);                 // 文化圏レベルに応じた敵の平均レベルを表示
        SetWaveCountText(1);                    // Wave数
        SetEnemyCountText(BattleFlowManager.I.BattleEnemies.Count); // 敵の数
    }

    public void DisablePanel()
    {
        Destroy(gameObject);
    }

    public void SetDifficultyText(ENUM.Difficulty difficulty)
    {
        // Uppercaseを用いて小文字を全て大文字に変換する
        dif.text = difficulty.ToString().ToUpper();
        Color color = Calculation.GetColorByDifficulty(difficulty);
        dif.color = new Color(color.r, color.g, color.b, 0.3f);
    }

    public void SetLevelText(int level)
    {
        averageLevel.text = "平均Lv : " + level.ToString();
    }

    public void SetWaveCountText(int count)
    {
        waveCount.text = "wave数 : " + (count + 1).ToString();
    }
    
    public void SetEnemyCountText(int count)
    {
        enemyCount.text = "残り敵数 : " + count.ToString();
    }
}