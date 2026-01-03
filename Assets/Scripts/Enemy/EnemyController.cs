using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyData data;
    EnemyApplyKaomoji applyKaomoji;
    public EnemyData EnemyData => data;

    void Awake()
    {
        applyKaomoji = GetComponent<EnemyApplyKaomoji>();
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

    public void OnBattle()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.BATTLE);
        applyKaomoji.Opaque();
    }

    public void OutBattle()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.WORLD);
        applyKaomoji.Translucent();
    }

    public void BattleEnd()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.WORLD);
        applyKaomoji.Opaque();
    }
        
}