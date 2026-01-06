using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyData data;
    EnemyFindPlayer findPlayer;
    EnemyApplyKaomoji applyKaomoji;
    public EnemyData EnemyData => data;

    void Awake()
    {
        applyKaomoji = GetComponent<EnemyApplyKaomoji>();
        findPlayer = GetComponent<EnemyFindPlayer>();
    }

    public void EnemyInitialize(EnemyData data)
    {
        if(EnemyData == null) this.data = data;
        
        IEnemyInitialize[] enemyInit = GetComponents<IEnemyInitialize>();
        ICharacterInitialize[] charaInit = GetComponents<ICharacterInitialize>();

        foreach(var item in charaInit)
        {
            item.Initialize(data);
        }

        foreach (var item in enemyInit)
        {
            item.EnemyInitialize(data);
        }
    }

    public void SetLevelAndDifficultyText(int level, ENUM.Difficulty difficulty)
    {
        applyKaomoji.SetLevelAndDifficultyText(level, difficulty);
    }

    /// <summary>
    /// 戦闘中になった敵の処理
    /// </summary>
    public void OnBattle()
    {
        findPlayer.DoEncount();
        gameObject.layer = LayerMask.NameToLayer(Layer.BATTLE);
        applyKaomoji.Opaque();
        applyKaomoji.DisableLevelAndDifficulty();
    }

    /// <summary>
    /// 戦闘外の敵の処理
    /// </summary>
    public void OutBattle()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.WORLD);
        applyKaomoji.Translucent();
        applyKaomoji.DisableLevelAndDifficulty();
    }

    /// <summary>
    /// 戦闘終了時に実行する処理
    /// </summary>
    public void BattleEnd()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.WORLD);
        applyKaomoji.Opaque();
        applyKaomoji.EnableLevelAndDifficulty();
    }
        
}