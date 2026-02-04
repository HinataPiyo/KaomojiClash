using ENUM;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyData data;
    EnemyFindPlayer findPlayer;
    EnemyApplyKaomoji applyKaomoji;
    public EnemyData EnemyData => data;
    public Difficulty Difficulty { get; private set; }      // 自身の難易度を保持しておく

    void Awake()
    {
        applyKaomoji = GetComponent<EnemyApplyKaomoji>();
        findPlayer = GetComponent<EnemyFindPlayer>();
    }

    public void EnemyInitialize(EnemyData data, Difficulty dif)
    {
        if(EnemyData == null) this.data = data;
        Difficulty = dif;
        
        IEnemyInitialize[] enemyInit = GetComponents<IEnemyInitialize>();
        ICharacterInitialize[] charaInit = GetComponents<ICharacterInitialize>();

        foreach(var item in charaInit)
        {
            item.Initialize(data);
        }

        foreach (var item in enemyInit)
        {
            item.EnemyInitialize(data, dif);
        }
    }

    public void SetEnemyWorldUI(int level, ENUM.Difficulty difficulty)
    {
        applyKaomoji.ChangeMinimapIconColor(difficulty);
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