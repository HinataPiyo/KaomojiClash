using System.Collections.Generic;
using Constants.Global;
using Constants;
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
        Money.Add(getMoney);
    }

    public void ApplyResultUI(Wave wave, int level)
    {
        resultUI.ApplyResultUI(wave, level);
    }

    /// <summary>
    /// 現在装備中の記号パーツに経験値を付与する
    /// リザルト表示されるタイミングでこの関数が実行される
    /// </summary>
    public void GetExpToMyParts(Wave wave)
    {
        foreach(KaomojiPartData part in Context.I.KaomojiPartDatas())
        {
            if(part == null) continue;
            KaomojiPart.LevelDetail detail = part.Data.levelDetail;

            // レベルアップ前の値を保持しておく
            int befor_level = detail.Level;
            float befor_exp = detail.Exp;
            wave.befor_level.Add(befor_level);
            wave.befor_exp.Add(befor_exp);

            // ! 経験値を反映
            detail.AddExperience(wave.getExp);
        }

        // 記号全体のステータスを更新する
        Context.I.UpdatePlayerStatus();
    }
}