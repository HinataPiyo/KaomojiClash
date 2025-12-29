using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemyData data;
    public EnemyData EnemyData => data;

    void Awake()
    {
        EnemyMovement movement = GetComponent<EnemyMovement>();
        EnemyApplyKaomoji applyKaomoji = GetComponent<EnemyApplyKaomoji>();
        EnemyMental mental = GetComponent<EnemyMental>();
        EnemyReflect reflect = GetComponent<EnemyReflect>();
        reflect.EnemyInitialize(data);
        mental.EnemyInitialize(data);
        applyKaomoji.Initialize(data);
        movement.Initialize(data);

        IInitialize[] init = GetComponents<IInitialize>();

        foreach (var item in init)
        {
            item.Initialize(data);
        }
    }
}