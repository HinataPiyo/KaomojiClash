using UnityEngine;
using ENUM;
using System.Collections.Generic;


/// <summary>
/// 戦闘の流れを管理するクラス
/// </summary>
public class BattleFlowManager : MonoBehaviour
{
    public static BattleFlowManager I { get; private set; }

    [SerializeField] WallController wallCtrl;
    [SerializeField] EnemySpawnController enemySpawnCtrl;
    [SerializeField] WaveController waveCtrl;
    [SerializeField] TargetGroupController targetGroupCtrl;

    public List<Transform> BattleEnemies { get; private set; } = new List<Transform>();
    public bool NoneEnemy() => BattleEnemies.Count == 0;



    void Awake()
    {
        if(I == null) I = this;
    }

    /// <summary>
    /// 戦闘開始の際に実行する処理
    /// </summary>
    /// <param name="enemy"></param>
    public void StartBattle(Transform enemy)
    {
        Context.I.ChangeStat(BattleStat.Start);
        BattleEnemies.Add(enemy);
        wallCtrl.CreateWall(Context.I.Player.transform.position, enemy.position);
        CameraZoom.I.InitSetCameraOrthographic(Context.I.BattleStat);
        targetGroupCtrl.AddTarget(enemy);

        waveCtrl.WaveStart(enemy);

        Debug.Log("エンカウントした敵の難易度 : " + enemy.GetComponent<EnemyController>().EnemyData.Wave.difficulty.ToString());
        
        Context.I.ChangeStat(BattleStat.Now);
        AudioManager.I.PlayBGM("StartBattle");
    }

    /// <summary>
    /// 戦闘終了するときに実行する処理
    /// </summary>
    public void EndBattle()
    {
        Context.I.ChangeStat(BattleStat.End);
        wallCtrl.DestroyWall();
        CameraZoom.I.InitSetCameraOrthographic(Context.I.BattleStat);
        AllOutEnemies();

        // AudioManager.I.PlaySE("BattleEnd");
        
        Context.I.ChangeStat(BattleStat.None);
        AudioManager.I.PlayBGM("EndBattle");
    }

    /// <summary>
    /// 敵が死んだときに全てのListに保持されていた自身（敵）の要素を除外する
    /// </summary>
    /// <param name="killedEnemy"></param>
    public void RemoveEnemy(Transform killedEnemy)
    {
        BattleEnemies.Remove(killedEnemy);
        targetGroupCtrl.RemoveTarget(killedEnemy);
        enemySpawnCtrl.CurrentEnemies.Remove(killedEnemy.gameObject);
    }

    /// <summary>
    /// 戦闘中の敵の処理と戦闘外の敵の処理を実行する
    /// </summary>
    public void OnBattleEnemies()
    {
        for(int ii = 0; ii < enemySpawnCtrl.CurrentEnemies.Count; ii++)
        {
            Transform all = enemySpawnCtrl.CurrentEnemies[ii].transform;
            bool isInBattle = false;

            for(int qq = 0; qq < BattleEnemies.Count; qq++)
            {
                if(BattleEnemies[qq] == all)
                {
                    isInBattle = true;
                    break;
                }
            }

            if(isInBattle)
            {
                all.GetComponent<EnemyController>().OnBattle();
            }
            else
            {
                all.GetComponent<EnemyController>().OutBattle();
            }
        }
    }

    /// <summary>
    /// 戦闘終了時、すべての敵を戦闘外の状態に戻す
    /// ここでは敵は不透明、レイヤーを戻す処理をしている
    /// </summary>
    void AllOutEnemies()
    {
        for(int ii = 0; ii < enemySpawnCtrl.CurrentEnemies.Count; ii++)
        {
            enemySpawnCtrl.CurrentEnemies[ii].GetComponent<EnemyController>().BattleEnd();
        }
    }
}