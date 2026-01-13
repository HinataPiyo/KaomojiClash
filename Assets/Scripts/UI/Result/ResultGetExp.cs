using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultGetExp : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI t_MaxExpAndCurrentExp;
    [SerializeField] TextMeshProUGUI t_Icon;
    [SerializeField] TextMeshProUGUI t_PartName;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="part"></param>
    /// <param name="totalExp"></param>
    public void Initialize(KaomojiPartData part, float totalExp)
    {
        // ! 経験値は既に反映済みだからこのような計算式になってる
        // ! part.Data.levelDetail.Exp　は目標経験値となる
        slider.maxValue = part.Data.levelDetail.GetNeedExpBorder();
        slider.value = part.Data.levelDetail.Exp - totalExp;
        t_Icon.text = part.Data.part;
        t_PartName.text = part.Data.partName;
        t_MaxExpAndCurrentExp.text = slider.value.ToString("#,###") + "/" + slider.maxValue.ToString("#,###");
        StartCoroutine(UpdateUI(part.Data.levelDetail.Exp));
    }

    /// <summary>
    /// スライダーを滑らかに更新する処理
    /// </summary>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    IEnumerator UpdateUI(float targetValue)
    {
        float start = slider.value;
        float duration = 1.5f; // 秒
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // EaseOut
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            slider.value = Mathf.Lerp(start, targetValue, eased);
            t_MaxExpAndCurrentExp.text = slider.value.ToString("#,###") + "/" + slider.maxValue.ToString("#,###");

            yield return null;
        }

        slider.value = targetValue; // 最終保証
    }

    /// <summary>
    /// リザルト表示が終了したらこの処理を実行する
    /// </summary>
    public void Remove()
    {
        Destroy(gameObject);
    }
}