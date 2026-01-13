using System.Collections;
using System.Collections.Generic;
using Constants.Global;
using TMPro;
using UnityEngine;

public class ResultUIControl : MonoBehaviour
{
    const float UPDATE_TEXT_SPEED = 0.04f;
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
    
    [SerializeField] ResultGetExpControl getExp;
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
        List<HasKaomojiParts> drop = wave.dropKaomojiParts;
        foreach(HasKaomojiParts hasPart in drop)
        {
            GameObject item = Instantiate(getItem_Prefab, getItem_Parent);
            ResultGetItem getItem = item.GetComponent<ResultGetItem>();
            getItem.Initialize(hasPart);    // 初期化
            items.Add(getItem);
            yield return new WaitForSeconds(0.3f);
            getItem.StartUIRoutine();       // UIを更新
        }

        getMoney.Initialize(wave.getMoney);        // 獲得金額を表示
        yield return new WaitForSeconds(0.8f);
        AudioManager.I.PlaySE("GetMoney");

        yield return new WaitForSeconds(1.5f);
        getExp.StartEXPRoutine(wave.getExp);
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
    /// 表示を終了する処理をまとめた関数
    /// </summary>
    public void DisablePanel()
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