using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerData data;    public PlayerData PlayerData => data;
    Combo combo;
    PlayerMovement movement;
    PlayerApplyKaomoji applyKaomoji;

    void Awake()
    {
        // Mentalの初期化はPlayerApplyKaomoji内で実行している
        
        combo = GetComponent<Combo>();
        movement = GetComponent<PlayerMovement>();
        applyKaomoji = GetComponent<PlayerApplyKaomoji>();
    }

    void Start()
    {
        applyKaomoji.Initialize(data);
        combo.Initialize(data);
        movement.Initialize(data);
        
        ICharacterInitialize[] init = GetComponents<ICharacterInitialize>();

        foreach (var item in init)
        {
            item.Initialize(data);
        }
    }
}