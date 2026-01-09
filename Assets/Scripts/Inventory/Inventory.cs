using System.Collections.Generic;
using Constants.Global;
using UnityEngine;

public sealed class Inventory
{
    List<HasKaomojiParts> parts = new List<HasKaomojiParts>();

    public void AllItemCheck()
    {
        foreach(HasKaomojiParts hasPart in parts)
        {
            Debug.Log($" {hasPart.part.Data.partName} : {hasPart.amount}");
        }
    }

    /// <summary>
    /// インベントリの中に記号データを追加
    /// </summary>
    /// <param name="part">記号データ</param>
    /// <param name="amount">追加する量</param>
    public void AddPart(KaomojiPartData part, int amount)
    {
        foreach(HasKaomojiParts hasPart in parts)
        {
            if(hasPart.part == part)
            {
                hasPart.amount += amount;
                return;
            }
        }

        parts.Add(new HasKaomojiParts { amount = amount, part = part });
    }

    public HasKaomojiParts GetPart(KaomojiPartData part)
    {
        foreach(HasKaomojiParts hasPart in parts)
        {
            if(hasPart.part == part)
            {
                return hasPart;
            }
        }
        
        return null;
    }


    /// <summary>
    /// インベントリの中にある記号データを所持数を減らす
    /// </summary>
    /// <param name="part">記号データ</param>
    /// <param name="amount">減らす量</param>
    public void RemovePart(KaomojiPartData part, int amount)
    {
        foreach(HasKaomojiParts hasPart in parts)
        {
            if(hasPart.part == part)
            {
                hasPart.amount -= amount;
                if(hasPart.amount <= 0)
                {
                    parts.Remove(hasPart);
                }
                return;
            }
        }
    }
}