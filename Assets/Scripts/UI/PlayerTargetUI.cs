using UnityEngine;

public class PlayerTargetUI : MonoBehaviour 
{
    Transform player;
    static readonly Vector2 offset = new Vector2(0, 0.3f);

    void Start()
    {
        player = Context.I.Player.transform;
    }

    void Update()
    {
        Vector2 pos = player.position + (Vector3)offset;
        transform.position = pos;
    }
}