using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyData data;
    public EnemyData EnemyData => data;

    void Awake()
    {
        ICharacterInitialize[] init = GetComponents<ICharacterInitialize>();
        IEnemyInitialize[] enemyInit = GetComponents<IEnemyInitialize>();

        foreach (var item in init)
        {
            item.Initialize(data);
        }

        foreach (var item in enemyInit)
        {
            item.EnemyInitialize(data);
        }
    }
}