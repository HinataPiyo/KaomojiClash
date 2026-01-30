using UnityEngine;

[CreateAssetMenu(fileName = "ArenaItemDatabase", menuName = "Database/ArenaItemDatabase")]
public class ArenaItemDatabase : ScriptableObject
{
    [SerializeField] ArenaItemData[] arenaItemDatas;
    public ArenaItemData[] GetAllArenaItemDatas() => arenaItemDatas;
}