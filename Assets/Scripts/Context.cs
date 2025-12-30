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

    public void StartBattle(Vector2 enemy)
    {
        BattleStat = BattleStat.Start;
        wallCtrl.CreateWall(Player.transform.position, enemy);
    }

    
    
}