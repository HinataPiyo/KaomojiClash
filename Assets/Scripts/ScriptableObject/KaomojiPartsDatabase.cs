using UnityEngine;

[CreateAssetMenu(fileName = "KaomojiPartsDatabase", menuName = "KaomojiPartsDatabase")]
public class KaomojiPartsDatabase : ScriptableObject
{
    [SerializeField] KaomojiPartData[] parts;
    public KaomojiPartData[] GetAllParts() => parts;
    public KaomojiPartData[] GetPartsByType(ENUM.KaomojiPartType type)
    {
        return System.Array.FindAll(parts, part => part.PartType == type);
    }
}