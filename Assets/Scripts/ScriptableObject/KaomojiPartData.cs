using UnityEngine;
using Constants.Global;

[CreateAssetMenu(fileName = "KaomojiPartData", menuName = "KaomojiPartData", order = 0)]
public class KaomojiPartData : ScriptableObject
{
    [Range(0.01f, 0.8f), SerializeField] float dorpProbability = 0.3f;      public float DropProbability => dorpProbability;
    [SerializeField] ENUM.KaomojiPartType partType;
    [SerializeField] KaomojiPart data;
    public KaomojiPart Data => data;
    public ENUM.KaomojiPartType PartType => partType;
}