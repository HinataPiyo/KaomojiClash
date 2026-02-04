using System.Collections;
using Constants.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Constants;

public class ResultGetItem : MonoBehaviour
{
    const float UPDATE_UI_SPEED = 0.05f;
    [SerializeField] TextMeshProUGUI part;
    [SerializeField] TextMeshProUGUI partName;
    [SerializeField] Slider maxHasItemSlider;
    [SerializeField] TextMeshProUGUI dropCount;

    HasKaomojiParts dropItem;
    int currentHasAmount;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(HasKaomojiParts dropItem)
    {
        this.dropItem = dropItem;
        KaomojiPart data = dropItem.part.Data;
        part.text = data.part;
        partName.text = data.partName;
        maxHasItemSlider.maxValue = 100;        // ! テストで最大所持数を100に設定
        currentHasAmount = InventoryManager.I.GetPart(dropItem.part).amount;
        maxHasItemSlider.value = currentHasAmount;
        dropCount.text = "+0";
    }

    public void StartUIRoutine()
    {
        StartCoroutine(CountUpRoutine(dropItem));
    }

    /// <summary>
    /// ドロップ品のカウントやスライダーを演出として一カウントずつ上昇させる処理
    /// </summary>
    IEnumerator CountUpRoutine(HasKaomojiParts dropItem)
    {
        int count = dropItem.amount;

        for(int ii = 0; ii < count; ii++)
        {
            int increase = ii + 1;
            dropCount.text = "+" + increase.ToString();
            currentHasAmount++;
            maxHasItemSlider.value = currentHasAmount;
            yield return new WaitForSeconds(UPDATE_UI_SPEED);
        }

        AudioManager.I.PlaySE("ShowGetItem");
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}