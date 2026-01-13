using System.Collections.Generic;
using Constants.Global;
using UnityEngine;

public class DropController : MonoBehaviour
{
    /// <summary>
    /// 敵一体が所持している記号データたちをドロップ抽選してListで返す
    /// </summary>
    /// <param name="parts"></param>
    /// <returns></returns>
    public List<KaomojiPartData> GetDorpParts(KaomojiPartData[] parts)
    {
        List<KaomojiPartData> dorpParts = new List<KaomojiPartData>();

        foreach(KaomojiPartData part in parts)
        {
            bool isDorp = Random.value < part.DropProbability;
            if(!isDorp) continue;
            dorpParts.Add(part);
        }

        return dorpParts;
    }

    /// <summary>
    /// Wave生成時にドロップ内容も決めておく
    /// </summary>
    public void GetDropParts(List<HasKaomojiParts> dropParts, EnemyData data)
    {
        List<KaomojiPartData> parts = GetDorpParts(data.Parts);

        if(parts.Count == 0) return;

        foreach(KaomojiPartData part in parts)
        {
            HasKaomojiParts existing = dropParts.Find(hp => hp.part == part);
            if(existing != null)
            {
                existing.amount++;
            }
            else
            {
                dropParts.Add(new HasKaomojiParts { amount = 1, part = part });
            }
        }
    }
}