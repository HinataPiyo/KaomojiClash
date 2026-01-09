using System.Collections.Generic;
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
}