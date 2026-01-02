using UnityEngine;
using ENUM;


public class EnemyFindPlayer : MonoBehaviour, IEnemyInitialize
{
    EnemyData data;
    public bool IsEncount { get; private set; }

    public void EnemyInitialize(EnemyData data)
    {
        this.data = data;
    }

    void Update()
    {
        if(data == null) return;
        
        // 現在が戦闘中じゃなければ
        if(Context.I.BattleStat == BattleStat.None)
        {
            int playerMask = LayerMask.GetMask("Player");
            Collider2D player = Physics2D.OverlapCircle((Vector2)transform.position, data.FindPlayerRadius, playerMask);
            IsEncount = player != null;

            if(IsEncount)
            {
                Context.I.StartBattle(transform.position);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsEncount ? Color.green : Color.red;
        float radius = (data != null) ? data.FindPlayerRadius : 0.5f;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
