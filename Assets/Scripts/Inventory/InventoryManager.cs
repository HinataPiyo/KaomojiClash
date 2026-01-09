using UnityEngine;
using Constants.Global;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I { get; private set;}
    Inventory inv;

    void Awake()
    {
        if(I == null) 
        {
            I = this;
            inv = new Inventory();        // インベントリを生成
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// インベントリに記号データを追加
    /// </summary>
    /// <param name="part">記号データ</param>
    /// <param name="amount">追加する量</param>
    public void AddPart(KaomojiPartData part, int amount)
    {
        inv.AddPart(part, amount);
    }

    public HasKaomojiParts GetPart(KaomojiPartData part)
    {
        return inv.GetPart(part);
    }

    public void AllItemCheck()
    {
        inv.AllItemCheck();
    }
}

