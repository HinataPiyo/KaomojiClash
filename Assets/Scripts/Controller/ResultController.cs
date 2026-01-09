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
}