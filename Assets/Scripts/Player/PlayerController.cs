using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerData data;    public PlayerData PlayerData => data;

    void Awake()
    {
        // Mentalの初期化はPlayerApplyKaomoji内で実行している
        
        Combo combo = GetComponent<Combo>();
        PlayerMovement movement = GetComponent<PlayerMovement>();
        PlayerApplyKaomoji applyKaomoji = GetComponent<PlayerApplyKaomoji>();
        applyKaomoji.Initialize(data);
        combo.Initialize(data);
        movement.Initialize(data);
        
        IInitialize[] init = GetComponents<IInitialize>();

        foreach (var item in init)
        {
            item.Initialize(data);
        }
    }
}