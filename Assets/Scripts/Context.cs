using ENUM;
using UI.Battle;
using UI.TotalResult;
using UnityEngine;
using Constants.Global;
using UI.TotalResult.Module;
using System.Collections.Generic;
using Constants;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Context : MonoBehaviour
{
    public static Context I { get; private set; }

    [SerializeField] GameObject player;         public GameObject Player => player;
    [SerializeField] PlayerData playerData;     public PlayerData PlayerData => playerData;
    [SerializeField] BattleModulesController battleModuleCtrl;

    [Header("TotalResult用のデータ")]
    [SerializeField] PlayableDirector totalResult_Director;
    [SerializeField] TimelineAsset[] totalResult_Timelines;
    [SerializeField] TotalResultModulesController totalResultModuleCtrl;
    [SerializeField] ArenaItemSettingData arenaItemSettingData;   public ArenaItemSettingData ArenaItemSettingData => arenaItemSettingData;
    public KaomojiPartData[] KaomojiPartDatas() => PlayerData.Kaomoji.GetAllPartsData();
    public BattleStat BattleStat { get; private set; } = BattleStat.None;
    public bool IsPlayerArive() => player  != null;

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
        
        UpdatePlayerStatus();
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

    public void UpdateMoneyDisplay()
    {
        battleModuleCtrl.HasMoneyDisplay.UpdateMoney();
    }

    public void StageClear()
    {
        ChangeStat(BattleStat.TotalResult);
        totalResult_Director.playableAsset = totalResult_Timelines[0];
        totalResult_Director.Play();
    }

    public void StageFailed()
    {
        ChangeStat(BattleStat.StageFailed);
        totalResult_Director.playableAsset = totalResult_Timelines[1];
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
}