using System.Collections;
using System.Collections.Generic;
using Constants.Global;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIControl : MonoBehaviour
{
    const float UPDATE_TEXT_SPEED = 0.05f;
    const string TITLE_NAME = "RESULT";
    [SerializeField] GameObject getItem_Prefab;
    [SerializeField] Transform getItem_Parent;

    [Space(10)]
    [SerializeField] TextMeshProUGUI t_Title;

    [SerializeField] TextMeshProUGUI t_Difficulty;
    [SerializeField] TextMeshProUGUI t_TextDifficulty;
    [SerializeField] TextMeshProUGUI t_Level;
    [SerializeField] TextMeshProUGUI t_TextLevel;
    [SerializeField] ResultGetMoney getMoney;
    
    [SerializeField] float fadeSpeed = 1f;
    CanvasGroup canvasGroup;

    List<ResultGetItem> items = new List<ResultGetItem>();


    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }


    /// <summary>
    /// UIに反映する
    /// </summary>
    public void ApplyResultUI(Wave wave, int level)
    {
        t_Title.text = "";
        t_Difficulty.text = "";
        t_TextDifficulty.text = "";
        t_Level.text = "";
        t_TextLevel.text = "";

        canvasGroup.alpha = 1f;

        StartCoroutine(ResultTextRoutine(wave.difficulty, level));
        StartCoroutine(ResultGetItemRoutine(wave));
    }

    /// <summary>
    /// ドロップ品のUIを順番に出現させていく処理
    /// </summary>
    IEnumerator ResultGetItemRoutine(Wave wave)
    {
        yield return new WaitForSeconds(0.5f);

        List<HasKaomojiParts> drop = wave.dropKaomojiParts;
        foreach(HasKaomojiParts hasPart in drop)
        {
            GameObject item = Instantiate(getItem_Prefab, getItem_Parent);
            ResultGetItem getItem = item.GetComponent<ResultGetItem>();
            getItem.Initialize(hasPart);    // 初期化
            items.Add(getItem);
            yield return new WaitForSeconds(0.5f);
            getItem.StartUIRoutine();       // UIを更新
        }

        getMoney.Initialize(wave.getMoney);        // 獲得金額を表示
        yield return new WaitForSeconds(1f);
        AudioManager.I.PlaySE("EndResult");

        StartCoroutine(EndResultRoutine());
    }

    /// <summary>
    /// 指定したTextMeshProUGUIに文字を送りで表示する
    /// </summary>
    private IEnumerator DisplayTextWithDelay(TextMeshProUGUI textUI, string text)
    {
        textUI.text = "";
        foreach(char c in text)
        {
            AudioManager.I.PlaySE("TextWriting");
            textUI.text += c;
            yield return new WaitForSeconds(UPDATE_TEXT_SPEED);
        }
    }

    IEnumerator ResultTextRoutine(ENUM.Difficulty difficulty, int level)
    {
        t_Difficulty.color = Calculation.GetColorByDifficulty(difficulty);

        yield return StartCoroutine(DisplayTextWithDelay(t_Title, TITLE_NAME));
        yield return StartCoroutine(DisplayTextWithDelay(t_TextDifficulty, "難易度:"));
        yield return StartCoroutine(DisplayTextWithDelay(t_Difficulty, difficulty.ToString()));
        yield return StartCoroutine(DisplayTextWithDelay(t_TextLevel, "平均レベル:"));
        yield return StartCoroutine(DisplayTextWithDelay(t_Level, level.ToString()));
    }

    /// <summary>
    /// リザルトの表示を終了するときの処理
    /// フェードアウト
    /// </summary>
    IEnumerator EndResultRoutine()
    {
        yield return new WaitForSeconds(5f);

        float elapsed = 0f;
        while(elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeSpeed);
            yield return null;
        }
        DisablePanel();        
    }

    /// <summary>
    /// 表示を終了する処理をまとめた関数
    /// </summary>
    void DisablePanel()
    {
        canvasGroup.alpha = 0f;
        foreach(ResultGetItem item in items)
        {
            item.Remove();
        }
        items.Clear();

        getMoney.Disable();
    }

}