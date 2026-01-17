using System.Collections.Generic;
using ENUM;
using Unity.VisualScripting;
using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context I { get; private set; }

    [SerializeField] GameObject player;         public GameObject Player => player;
    PlayerApplyKaomoji playerApply;
    public KaomojiPartData[] KaomojiPartDatas() => playerApply.KaomojiPartDatas.ToArray();
    public BattleStat BattleStat { get; private set; } = BattleStat.None;


    void Awake()
    {
        if(I == null)
        {
            I = this;
        }

        playerApply = player.GetComponent<PlayerApplyKaomoji>();
    }

    /// <summary>
    /// 記号全体のステータスを更新する
    /// </summary>
    public void UpdatePlayerStatus()
    {
        playerApply.UpdateTotalParameter();
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