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

    public void SetWaveData(Wave wave, int level)
    {
        panel.style.display = DisplayStyle.Flex;
        SetDifficultyText(wave.difficulty);
        SetLevelText(level);
        SetWaveCountText(1);
        SetEnemyCountText(BattleFlowManager.I.BattleEnemies.Count);
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