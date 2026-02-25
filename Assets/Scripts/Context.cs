using ENUM;
using UI.Battle;
using UI.TotalResult;
using UnityEngine;
using Constants;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class Context : MonoBehaviour
{
    public static Context I { get; private set; }

    [SerializeField] GameObject player;         public GameObject Player => player;
    [SerializeField] PlayerData playerData;     public PlayerData PlayerData => playerData;
    [SerializeField] EnemySpawnController enemySpawnCtrl;
    [SerializeField] BattleModulesController battleModuleCtrl;

    [Header("TotalResult用のデータ")]
    [SerializeField] PlayableDirector totalResult_Director;
    [SerializeField] TimelineAsset[] totalResult_Timelines;
    [SerializeField] TotalResultModulesController totalResultModuleCtrl;
    [SerializeField] ArenaItemSettingData arenaItemSettingData;   public ArenaItemSettingData ArenaItemSettingData => arenaItemSettingData;


    public KaomojiPartData[] KaomojiPartDatas() => PlayerData.Kaomoji.GetAllPartsData();
    public BattleStat BattleStat { get; private set; } = BattleStat.None;
    public bool IsPlayerArive() => player  != null;


    public PlayerApplyKaomoji PlayerKaomojiApply { get; private set; }
    public float GetPlayerStamina() => PlayerKaomojiApply.GetUpgradedStatus().stamina;
    public float GetPlayerPower() => PlayerKaomojiApply.GetUpgradedStatus().power;
    public float GetPlayerSpeed() => PlayerKaomojiApply.GetUpgradedStatus().speed;
    public float GetPlayerGuard() => PlayerKaomojiApply.GetUpgradedStatus().guard;


    /// <summary>
    /// TotalResult用パラメータ
    /// </summary>
    public TotalResultParams TR_Params { get; private set;} = new TotalResultParams();

    public class TotalResultParams
    {
        public int TotalGetMoney { get; private set; }
        public int TotalDecMoney { get; private set; }

        public void ApplyGetMoney(int value) => TotalGetMoney += value;
        public void ApplyDecMoney(int value) => TotalDecMoney += value;
    }


    void Awake()
    {
        if(I == null)
        {
            I = this;
        }
        
        PlayerKaomojiApply = player.GetComponent<PlayerApplyKaomoji>();
    }

    /// <summary>
    /// 記号全体のステータスを更新する
    /// </summary>
    public void RefreshPlayerStatus()
    {
        PlayerKaomojiApply.RefreshUpgradedStatus();
        battleModuleCtrl.UpdateKaomojiCompositions();
    }

    /// <summary>
    /// 戦闘状態の状態遷移
    /// </summary>
    /// <param name="stat"></param>
    public void ChangeStat(BattleStat stat)
    {
        BattleStat = stat;
    }

    /// <summary>
    /// 所持金表示の更新
    /// </summary>
    public void UpdateMoneyDisplay()
    {
        battleModuleCtrl.HasMoneyDisplay.UpdateMoney();
    }

    /// <summary>
    /// ステージクリア時の処理
    /// </summary>
    public void StageClear()
    {
        ChangeStat(BattleStat.StageClear);
        StartCoroutine(WaitStartResult());
        AreaManager.I.CurrentAreaData.ChangeClear();        // ステージクリア済みにする
    }

    /// <summary>
    /// ステージ失敗時の処理
    /// </summary>
    public void StageFailed()
    {
        ChangeStat(BattleStat.StageFailed);
        StartCoroutine(WaitStartResult());
    }

    /// <summary>
    /// 最後の敵を倒した際にResultが流れるので終わるまで待機するため
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitStartResult()
    {
        switch(BattleStat)
        {
            case BattleStat.StageClear:
                yield return new WaitForSeconds(5f);
                AudioManager.I.StopBGM();
                totalResult_Director.playableAsset = totalResult_Timelines[0];
                break;
            case BattleStat.StageFailed:
                yield return new WaitForSeconds(1f);
                totalResult_Director.playableAsset = totalResult_Timelines[1];
                break;
        }

        totalResult_Director.Play();
    }

    /// <summary>
    /// TotalResultPanelの表示しOnEnableで各モジュールのUIを更新する
    /// Dierctorで再生が終了したタイミングで実行
    /// </summary>
    public void RefreshTotalResult()
    {
        totalResultModuleCtrl.EnableResultPanel();

        int cultureLevel = AreaManager.I.CurrentAreaData.Build.cultureLevel;
        int avgLevel = AreaBuild.GetEnemyAverageLevel(cultureLevel);
        float density = AreaManager.I.CurrentAreaData.Build.kaomojiDensity;

        totalResultModuleCtrl.module_FAI.UpdateUI(cultureLevel, avgLevel, density);
        totalResultModuleCtrl.module_KC.UpdateUI(playerData.Kaomoji);
        totalResultModuleCtrl.module_TGADM.UpdateUI(TR_Params.TotalDecMoney, TR_Params.TotalGetMoney);
        totalResultModuleCtrl.module_TGP.CreateUI(InventoryManager.I.inv.GetAllPartsData());
        totalResultModuleCtrl.module_AI.CreateUI(arenaItemSettingData.GetArenaItemDatas());

    }

    /// <summary>
    /// プレイヤーを次の敵に移動させる処理を開始する
    /// </summary>
    public void StartMovePlayerToNextEnemy(int nextStageIndex)
    {
        Vector2 nextEnemyPos = GetNextEnemyPosition(nextStageIndex);
        player.GetComponent<PlayerMovement>().StartMoveToNextEnemy(nextEnemyPos);
    }

    Vector2 GetNextEnemyPosition(int nextStageIndex)
    {
        if(enemySpawnCtrl.FirstSpawnEnemies.Count == 0)
        {
            Debug.LogError("敵が存在しません。");
            return Vector2.zero;
        }

        GameObject nextEnemy = enemySpawnCtrl.FirstSpawnEnemies[nextStageIndex];
        return nextEnemy.transform.position;
    }
}