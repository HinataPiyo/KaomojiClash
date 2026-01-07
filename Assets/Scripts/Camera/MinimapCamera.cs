using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (!Context.I.Player) return;

        Vector3 pos = Context.I.Player.transform.position;
        transform.position = new Vector3(pos.x, pos.y, -10);
    }
}
