using UnityEngine;

[CreateAssetMenu(fileName = "KaomojiPartsDatabase", menuName = "Database/KaomojiPartsDatabase")]
public class KaomojiPartsDatabase : ScriptableObject
{
    [SerializeField] KaomojiPartData[] parts;
    public KaomojiPartData[] GetAllParts() => parts;
    public KaomojiPartData[] GetPartsByType(ENUM.KaomojiPartType type)
    {
        return System.Array.FindAll(parts, part => part.PartType == type);
    }
}