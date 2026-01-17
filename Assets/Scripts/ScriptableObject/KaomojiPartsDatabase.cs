using UnityEngine;

[CreateAssetMenu(fileName = "KaomojiPartsDatabase", menuName = "KaomojiPartsDatabase")]
public class KaomojiPartsDatabase : ScriptableObject
{
    [SerializeField] KaomojiPartData[] parts;
    public KaomojiPartData[] GetAllParts() => parts;
}