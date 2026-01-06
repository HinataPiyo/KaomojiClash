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
        SetWaveCountText(wave.elements.Count);
        SetEnemyCountText(BattleFlowManager.I.BattleEnemies.Count);
    }

    public void SetDifficultyText(ENUM.Difficulty difficulty)
    {
        this.difficulty.text = difficulty.ToString();
    }

    public void SetLevelText(int level)
    {
        this.level.text = level.ToString();
    }

    public void SetWaveCountText(int waveCount)
    {
        this.waveCount.text = waveCount.ToString();
    }
    
    public void SetEnemyCountText(int enemyCount)
    {
        this.enemyCount.text = enemyCount.ToString();
    }
}