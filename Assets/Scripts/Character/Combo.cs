using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class Combo : MonoBehaviour
{
    [SerializeField] PlayerData data;
    [SerializeField] ComboUIControl comboUI;
    [SerializeField] int comboCount = 0;
    [SerializeField] float comboTimer = 3f;     // コンボ継続時間
    [SerializeField] int amplifyer_MaxCombo = 10;        // 最大コンボ数

    float currentTimer = 0f;
    public float CurrentAmplifyer { get; private set; } = 0f;

    void Awake()
    {
        Reset();
    }

    void Update()
    {
        if(comboCount > 0)
        {
            currentTimer -= Time.deltaTime;
            if(currentTimer <= 0f)
            {
                Reset();    // コンボリセット
            }
        }

        // 現在の攻撃力増幅率を計算
        CurrentAmplifyer = Mathf.Lerp(data.StrengthMinAmplifyer, data.StrengthMaxAmplifyer, (float)comboCount / amplifyer_MaxCombo);
        // StrengthMin..StrengthMax の範囲を 0..1 に正規化して UI に渡す
        float normalizedAmplifyer = Mathf.InverseLerp(data.StrengthMinAmplifyer, data.StrengthMaxAmplifyer, CurrentAmplifyer);
        comboUI.UpdateComboUI(comboCount, normalizedAmplifyer);
    }

    /// <summary>
    /// コンボ数を増加する
    /// </summary>
    public void IncreaseCombo()
    {
        comboCount++;
        currentTimer = comboTimer;
    }

    void Reset()
    {
        comboCount = 0;
        currentTimer = 0f;
    }
}