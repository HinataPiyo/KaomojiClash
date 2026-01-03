using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] Vector2 spawnArea;
    public Vector2 SpawnArea => spawnArea;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnArea);
    }
}