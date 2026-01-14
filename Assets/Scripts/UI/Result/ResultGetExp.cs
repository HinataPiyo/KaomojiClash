using System.Collections;
using Constants.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultGetExp : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI t_Level;
    [SerializeField] TextMeshProUGUI t_MaxExpAndCurrentExp;
    [SerializeField] TextMeshProUGUI t_Icon;
    [SerializeField] TextMeshProUGUI t_PartName;
    [SerializeField] TextMeshProUGUI t_KaomojiPartType;

    [Header("Animation (distance-based)")]
    [SerializeField] float fastSpeed = 2.5f;   // 前半速度（slider.maxValue/秒 の係数）
    [SerializeField] float slowSpeed = 0.6f;   // 終盤速度
    [Tooltip("target に近いと判定する残りExp量。レベル幅が大きく変動するなら割合指定に切替推奨。")]
    [SerializeField] float slowDownRange = 20f;
    [SerializeField] AnimationCurve slowCurve = null; // nullなら内蔵カーブを使用

    [Header("Option")]
    [SerializeField] float levelUpPause = 0.05f;

    KaomojiPartData part;

    int currentLevel;
    float expInLevel;

    void Awake()
    {
        if (slowCurve == null)
        {
            // 0→1 で「だんだん減速」を作る無難なカーブ
            slowCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="part"></param>
    /// <param name="totalExp">今回加算する経験値</param>
    /// <param name="befor_level">加算前レベル</param>
    /// <param name="befor_exp">加算前のレベル内経験値</param>
    public void Initialize(KaomojiPartData part, float totalExp, int befor_level, float befor_exp)
    {
        this.part = part;

        currentLevel = befor_level;
        expInLevel = befor_exp;

        t_Icon.text = part.Data.part;
        t_PartName.text = part.Data.partName;
        t_KaomojiPartType.text = Calculation.GetKaomojiPartTypeName(part.PartType);

        ApplyLevelUI(); // maxValue/value/text を同期

        StartCoroutine(AnimateGainExp(totalExp));
    }

    IEnumerator AnimateGainExp(float gainExp)
    {
        while (gainExp > 0f)
        {
            float needExp = part.Data.levelDetail.GetNeedExpBorder(currentLevel);
            float remainToLevelUp = Mathf.Max(0f, needExp - expInLevel);

            // 今レベルで消費する分（レベルアップ境界を跨ぐ場合はここで満タンまで）
            float consume = Mathf.Min(gainExp, remainToLevelUp);

            float target = expInLevel + consume; // このレベル内の目標値

            // 「targetに追いつきそうになったら減速」するアニメーション
            yield return AnimateToTarget(target);

            // 最終保証
            expInLevel = target;
            slider.value = expInLevel;
            UpdateText();

            gainExp -= consume;

            // レベルアップ境界に到達した場合のみ次レベルへ
            if (expInLevel >= needExp - 0.0001f)
            {
                currentLevel++;
                expInLevel = 0f;

                ApplyLevelUI();

                if (levelUpPause > 0f)
                    yield return new WaitForSeconds(levelUpPause);
            }
        }
    }

    /// <summary>
    /// 「スライダー終盤」ではなく「targetに近づいた終盤」で減速する
    /// </summary>
    IEnumerator AnimateToTarget(float target)
    {
        target = Mathf.Clamp(target, 0f, slider.maxValue);

        while (slider.value < target - 0.0001f)
        {
            float current = slider.value;
            float remaining = target - current;

            // 残りが slowDownRange 以内に入ったら 0→1 に遷移して減速へ
            float k = 1f - Mathf.Clamp01(remaining / Mathf.Max(0.0001f, slowDownRange));
            float easedK = slowCurve.Evaluate(k);

            float speed = Mathf.Lerp(fastSpeed, slowSpeed, easedK);

            // レベル幅が変わっても体感が崩れにくいよう、maxValue基準で移動量を作る
            float step = speed * Time.deltaTime * slider.maxValue;

            float next = Mathf.MoveTowards(current, target, step);
            slider.value = next;
            expInLevel = next;

            UpdateText();
            yield return null;
        }

        slider.value = target;
        expInLevel = target;
        UpdateText();
    }

    void ApplyLevelUI()
    {
        float needExp = part.Data.levelDetail.GetNeedExpBorder(currentLevel);
        slider.maxValue = Mathf.Max(1f, needExp); // 0割/無効値対策
        slider.value = Mathf.Clamp(expInLevel, 0f, slider.maxValue);
        UpdateText();
    }

    void UpdateText()
    {
        t_MaxExpAndCurrentExp.text =
            slider.value.ToString("#,###") + "/" + slider.maxValue.ToString("#,###");
        t_Level.text = "Lv" + currentLevel.ToString();
    }

    /// <summary>
    /// リザルト表示が終了したらこの処理を実行する
    /// </summary>
    public void Remove()
    {
        Destroy(gameObject);
    }
}
