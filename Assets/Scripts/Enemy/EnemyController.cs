using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyData data;
    public EnemyData EnemyData => data;

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
}