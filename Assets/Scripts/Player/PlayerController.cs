using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerData data;    public PlayerData PlayerData => data;

    void Awake()
    {
        Combo combo = GetComponent<Combo>();
        PlayerMovement movement = GetComponent<PlayerMovement>();
        combo.Initialize(data);
        movement.Initialize(data);
        
        IInitialize[] init = GetComponents<IInitialize>();

        foreach (var item in init)
        {
            item.Initialize(data);
        }
    }
}