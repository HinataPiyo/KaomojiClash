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
        Debug.Log("ドロップ品がインベントリに格納されました");
        
        InventoryManager.I.AllItemCheck();
    }

    public void ApplyResultUI(Wave wave, int level)
    {
        resultUI.ApplyResultUI(wave, level);
    }
}