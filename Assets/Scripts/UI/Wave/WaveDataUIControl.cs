using Constants.Global;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveDataUIControl : MonoBehaviour
{
    UIDocument uiDonc;

    VisualElement panel;

    Label difficulty;
    Label level;
    Label waveCount;
    Label enemyCount;


    void Awake()
    {
        uiDonc = GetComponent<UIDocument>();
        panel = uiDonc.rootVisualElement.Q("wave-data-panel");
        difficulty = panel.Q("difficulty-box").Q<Label>("value");
        level = panel.Q("level-box").Q<Label>("value");
        waveCount = panel.Q("current-wave-count-box").Q<Label>("value");
        enemyCount = panel.Q("current-enemy-amount-box").Q<Label>("value");

        panel.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// WaveDataをUIに反映する
    /// </summary>
    /// <param name="wave">WaveData</param>
    /// <param name="cultureLevel">文化圏レベル</param>
    /// <param name="cultureMultiplier">ステータスを強化する倍率</param>
    /// <param name="avgLevel">平均レベル</param>
    public void SetWaveData(Wave wave, int cultureLevel, float cultureMultiplier, int avgLevel)
    {
        panel.style.display = DisplayStyle.Flex;
        SetDifficultyText(wave.difficulty);     // 難易度
        SetLevelText(avgLevel);                 // 文化圏レベルに応じた敵の平均レベルを表示
        SetWaveCountText(1);                    // Wave数
        SetEnemyCountText(BattleFlowManager.I.BattleEnemies.Count); // 敵の数
    }

    public void DisablePanel()
    {
        panel.style.display = DisplayStyle.None;
    }

    public void SetDifficultyText(ENUM.Difficulty difficulty)
    {
        // Uppercaseを用いて小文字を全て大文字に変換する
        this.difficulty.text = difficulty.ToString().ToUpper();;
        this.difficulty.style.color = Calculation.GetColorByDifficulty(difficulty);
    }

    public void SetLevelText(int level)
    {
        this.level.text = level.ToString();
    }

    public void SetWaveCountText(int waveCount)
    {
        this.waveCount.text = (waveCount + 1).ToString();
    }
    
    public void SetEnemyCountText(int enemyCount)
    {
        this.enemyCount.text = enemyCount.ToString();
    }
}