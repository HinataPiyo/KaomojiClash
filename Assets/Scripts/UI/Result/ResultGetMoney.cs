using TMPro;
using UnityEngine;

public class ResultGetMoney : MonoBehaviour
{
    Animator anim;
    [SerializeField] TextMeshProUGUI t_GetMoney;

    void Awake()
    {
        anim = GetComponent<Animator>();
        Disable();
    }

    public void Initialize(int getMoney)
    {
        // 1,000の様な表示の仕方にする
        t_GetMoney.text = getMoney.ToString("#,###") + "円";
        gameObject.SetActive(true);
        transform.SetAsLastSibling();       // 子オブジェクトの最後尾へ移動
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}