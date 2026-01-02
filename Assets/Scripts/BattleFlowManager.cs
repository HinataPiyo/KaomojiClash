using UnityEngine;

public class BattleFlowManager : MonoBehaviour
{
    public static BattleFlowManager I { get; private set; }

    [SerializeField] WallController wallCtrl;
    [SerializeField] EnemySpawnController enemySpawnCtrl;



    void Awake()
    {
        if(I == null) I = this;
        enemySpawnCtrl.SpawnEnemy(1);
    }


}