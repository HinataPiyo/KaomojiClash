using ENUM;
using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context I { get; private set; }

    [SerializeField] GameObject player;         public GameObject Player => player;

    [SerializeField] WallController wallCtrl;
    public BattleStat BattleStat { get; private set; } = BattleStat.None;


    void Awake()
    {
        if(I == null)
        {
            I = this;
        }
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