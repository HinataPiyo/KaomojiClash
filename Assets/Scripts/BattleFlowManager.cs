using UnityEngine;
using ENUM;
using System.Collections.Generic;
using System.Collections;


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

    [SerializeField] GameObject encountEffect;
    [SerializeField] GameObject startWaveEffect;

    WaveDataUIControl waveDataCtrl;
    public List<Transform> BattleEnemies { get; private set; } = new List<Transform>();
    public bool NoneEnemy() => BattleEnemies.Count == 0;

    static readonly float EncountWaitTime = 1.5f;



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
        Transform player = Context.I.Player.transform;
        Context.I.ChangeStat(BattleStat.Start);
        BattleEnemies.Add(enemy);
        targetGroupCtrl.AddTarget(enemy, true);
        GameObject effect = EncountEffect(player.position, enemy.position);

        Vector2 centerPos = wallCtrl.CreateWall(player.position, enemy.position);
        waveDataCtrl = waveCtrl.WaveStart(enemy, centerPos);

        Debug.Log("エンカウントした敵の難易度 : " + enemy.GetComponent<EnemyController>().EnemyData.Wave.difficulty.ToString());
        AudioManager.I.PlayBGM(string.Empty);       // BGMを止める
        AudioManager.I.PlaySE("Encount");

        StartCoroutine(BattleStatChangeNowFlow(effect));
    }

    IEnumerator BattleStatChangeNowFlow(GameObject encountEffect)
    {
        yield return new WaitForSeconds(EncountWaitTime);

        Destroy(encountEffect);
        startWaveEffect.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        startWaveEffect.SetActive(false);

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

        waveDataCtrl.DisablePanel();
        
        Context.I.ChangeStat(BattleStat.None);
        AudioManager.I.PlayBGM("EndBattle");

        bool isAllEnemyDefeated = enemySpawnCtrl.IsAllEnemyDefeated();      // 全ての敵を倒しているかどうかを確認
        if(isAllEnemyDefeated)
        {
            
            Context.I.StageClear();         // 全ての敵を倒していたらTotalResultを再生
        }
        
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
        
        waveDataCtrl.SetEnemyCountText(BattleEnemies.Count);        // 残り敵数をUIに反映
    }

    /// <summary>
    /// 接敵した際にEffectを表示する
    /// </summary>
    GameObject EncountEffect(Vector2 player, Vector2 enemy)
    {
        Vector2 createPos = Vector2.Lerp(player, enemy, 0.5f);        // プレイヤーと敵との距離の中心を取得
        GameObject effect = Instantiate(encountEffect, createPos, Quaternion.identity, WorldCanvasManager.I.transform);

        // カメラの距離とZoomする時間を設定
        CameraZoom.I.StartEncountZoom(1.5f, EncountWaitTime);
        return effect;
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
        foreach(var enemyObject in enemySpawnCtrl.CurrentEnemies)
        {
            enemyObject.GetComponent<EnemyController>().BattleEnd();
        }
    }
}