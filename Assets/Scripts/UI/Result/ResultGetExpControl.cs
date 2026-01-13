using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultGetExpControl : MonoBehaviour
{
    [SerializeField] ResultUIControl resultUI;
    [SerializeField] UIMoveEaseOut moveUI;
    [SerializeField] TextMeshProUGUI t_GetExp;
    
    [SerializeField] GameObject getExp_Prefab;
    [SerializeField] Transform getExp_Parent;
    [SerializeField] float fadeSpeed = 1f;
    CanvasGroup canvasGroup;

    List<ResultGetExp> items = new List<ResultGetExp>();

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void StartEXPRoutine(float totalExp)
    {
        StartCoroutine(ExperienceRoutine(totalExp));
    }

    /// <summary>
    /// 取得経験値を表示するためのコルーチン
    /// </summary>
    /// <param name="totalExp"></param>
    /// <returns></returns>
    IEnumerator ExperienceRoutine(float totalExp)
    {
        t_GetExp.text = "+" + totalExp.ToString("#,###");

        moveUI.MoveLeft(360, 1f);
        
        KaomojiPartData[] parts = Context.I.KaomojiPartDatas();
        foreach(KaomojiPartData part in parts)
        {
            GameObject item = Instantiate(getExp_Prefab, getExp_Parent);
            ResultGetExp getExp = item.GetComponent<ResultGetExp>();
            getExp.Initialize(part, totalExp);
            items.Add(getExp);
        }

        yield return StartCoroutine(FadeIn());
        

        StartCoroutine(EndResultRoutine());
    }

    /// <summary>
    /// リザルトの表示を終了するときの処理
    /// フェードアウト
    /// </summary>
    IEnumerator EndResultRoutine()
    {
        yield return new WaitForSeconds(2f);

        float elapsed = 0f;
        while(elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeSpeed);
            yield return null;
        }

        resultUI.DisablePanel();
        DisablePanel();
    }

    /// <summary>
    /// 取得経験値用パネルをフェードインで表示
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while(elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeSpeed);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 最終処理をここにすべてまとめる
    /// </summary>
    void DisablePanel()
    {
        canvasGroup.alpha = 0f;
        foreach(ResultGetExp item in items)
        {
            item.Remove();
        }
        items.Clear();

        moveUI.Reset();
    }
}