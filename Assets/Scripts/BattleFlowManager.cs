using UnityEngine;
using ENUM;
using System.Collections.Generic;

public class BattleFlowManager : MonoBehaviour
{
    public static BattleFlowManager I { get; private set; }

    [SerializeField] WallController wallCtrl;
    [SerializeField] EnemySpawnController enemySpawnCtrl;
    [SerializeField] TargetGroupController targetGroupCtrl;

    public List<Transform> BattleEnemies { get; private set; } = new List<Transform>();


    void Awake()
    {
        if(I == null) I = this;
    }

    public void StartBattle(Transform enemy)
    {
        Context.I.ChangeStat(BattleStat.Start);
        BattleEnemies.Add(enemy);
        wallCtrl.CreateWall(Context.I.Player.transform.position, enemy.position);
        CameraZoom.I.InitSetCameraOrthographic(Context.I.BattleStat);
        targetGroupCtrl.AddTarget(enemy);
        OnBattleEnemies();
        
        Context.I.ChangeStat(BattleStat.Now);
    }

    public void EndBattle()
    {
        Context.I.ChangeStat(BattleStat.End);
        wallCtrl.DestroyWall();
        CameraZoom.I.InitSetCameraOrthographic(Context.I.BattleStat);
        AllOutEnemies();
        
        Context.I.ChangeStat(BattleStat.None);
    }

    public void RemoveEnemy(Transform killedEnemy)
    {
        BattleEnemies.Remove(killedEnemy);
        targetGroupCtrl.RemoveTarget(killedEnemy);
        enemySpawnCtrl.CurrentEnemies.Remove(killedEnemy.gameObject);
    }

    void OnBattleEnemies()
    {
        foreach(Transform joinE in BattleEnemies)
        {
            foreach(Transform outE in enemySpawnCtrl.GetEnemiesTransform())
            {
                if(joinE != outE)
                {
                    outE.GetComponent<EnemyController>().OutBattle();
                    continue;
                }
            }

            joinE.GetComponent<EnemyController>().OnBattle();
        }
    }

    void AllOutEnemies()
    {
        foreach(Transform e in enemySpawnCtrl.GetEnemiesTransform())
        {
            e.GetComponent<EnemyController>().BattleEnd();
        }
    }
}