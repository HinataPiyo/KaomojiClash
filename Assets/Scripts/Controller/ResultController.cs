using System.Collections.Generic;
using Constants.Global;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    [SerializeField] ResultUIControl resultUI;
    /// <summary>
    /// ドロップ品をインベントリに移す関数
    /// </summary>
    /// <param name="drop">Wave初めに作ったDropListを取得する</param>
    public void DropsToInventory(List<HasKaomojiParts> drop)
    {
        foreach(HasKaomojiParts hasPart in drop)
        {
            InventoryManager.I.AddPart(hasPart.part, hasPart.amount);
        }
        InventoryManager.I.AllItemCheck();
    }

    /// <summary>
    /// 取得金額分、所持金に反映させる
    /// </summary>
    /// <param name="getMoney"></param>
    public void GetMoneyToHasMoney(int getMoney)
    {
        MoneyManager.I.AddMoney(getMoney);
    }

    public void ApplyResultUI(Wave wave, int level)
    {
        resultUI.ApplyResultUI(wave, level);
    }

    /// <summary>
    /// 現在装備中の記号パーツに経験値を付与する
    /// </summary>
    public void GetExpToMyParts(float totalExp)
    {
        foreach(KaomojiPartData part in Context.I.KaomojiPartDatas())
        {
            // 経験値を反映させていく
            part.Data.levelDetail.AddExperience(totalExp);
        }

        // 記号全体のステータスを更新する
        Context.I.UpdatePlayerStatus();
    }
}