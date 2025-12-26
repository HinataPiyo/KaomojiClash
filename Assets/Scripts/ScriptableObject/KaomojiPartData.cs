using UnityEngine;
using Constants.Global;

[CreateAssetMenu(fileName = "KaomojiPartData", menuName = "KaomojiPartData", order = 0)]
public class KaomojiPartData : ScriptableObject
{
    [SerializeField] ENUM.KaomojiPartType partType;
    [SerializeField] KaomojiPart data;
    public KaomojiPart Data => data;
    public ENUM.KaomojiPartType PartType => partType;
}