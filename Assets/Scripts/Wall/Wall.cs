using UnityEngine;

public class Wall : MonoBehaviour
{
    // 壁の内側に生成できる範囲
    [SerializeField] Vector2 spawnArea;
    public Vector2 SpawnArea => spawnArea;
    public Vector2 CenterPosition => transform.position;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnArea);
    }
}