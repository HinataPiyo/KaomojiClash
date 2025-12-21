
using System.Collections;
using TMPro;
using UnityEngine;

public class ComboUIControl : MonoBehaviour
{
    [SerializeField] Lean.Gui.LeanCircle amplifyerCircle;
    [SerializeField] TextMeshProUGUI comboCountText;
    [SerializeField] Animator anim;

    GameObject[] childs;

    Coroutine fillRoutine;
    int beforeComboCount = 0;

    void Awake()
    {
        childs = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childs[i] = transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// コンボUIを更新する
    /// </summary>
    /// <param name="comboCount">combo数</param>
    /// <param name="fillAmount">Fill</param>
    public void UpdateComboUI(int comboCount, float fillAmount)
    {
        if(fillRoutine != null) StopCoroutine(fillRoutine);
        fillRoutine = StartCoroutine(FillCircleRoutine(fillAmount));
        comboCountText.text = comboCount.ToString("F0");
        if(beforeComboCount != comboCount && comboCount > 0)
        {
            anim.SetTrigger("combo");
            beforeComboCount = comboCount;
        }

        foreach (var child in childs)
        {
            child.SetActive(comboCount > 0);
        }
    }

    IEnumerator FillCircleRoutine(float targetFill)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        float initialFill = amplifyerCircle.Fill;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            amplifyerCircle.Fill = Mathf.Lerp(initialFill, targetFill, elapsed / duration);
            yield return null;
        }

        amplifyerCircle.Fill = targetFill;
        fillRoutine = null;
    }
}