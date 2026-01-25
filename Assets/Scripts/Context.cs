using ENUM;
using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context I { get; private set; }

    [SerializeField] GameObject player;         public GameObject Player => player;
    [SerializeField] PlayerData playerData;     public PlayerData PlayerData => playerData;
    public KaomojiPartData[] KaomojiPartDatas() => PlayerData.Kaomoji.GetAllPartsData();
    public BattleStat BattleStat { get; private set; } = BattleStat.None;


    void Awake()
    {
        if(I == null)
        {
            I = this;
        }
    }

    /// <summary>
    /// 記号全体のステータスを更新する
    /// </summary>
    public void UpdatePlayerStatus()
    {
        playerData.Kaomoji.UpdateTotalParameter();
    }

    /// <summary>
    /// 戦闘状態の状態遷移
    /// </summary>
    /// <param name="stat"></param>
    public void ChangeStat(BattleStat stat)
    {
        BattleStat = stat;
    }
}