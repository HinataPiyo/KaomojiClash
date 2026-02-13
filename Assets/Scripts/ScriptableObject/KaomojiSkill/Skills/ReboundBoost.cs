using UnityEngine;

[CreateAssetMenu(fileName = "ReboundBoost", menuName = "SkillTags/ReboundBoost")]
public class ReboundBoost : SkillTag
{
    [Header("速度上昇する時間（秒）")]
    [SerializeField] float speedUpDuration = 2f;
    [Header("速度上昇する倍率（％）")]
    [SerializeField] float speedUpMultiplier = 0.4f;

    public override string GetDescription(int stackCount)
    {
        float totalMultiplier = speedUpMultiplier * stackCount;
        float duration = GetDurationByLevel(stackCount);
        return $"壁に触れてから {duration} 秒間、速度が {totalMultiplier * 100}% 上昇する。";
    }

    public float GetDurationByLevel(int stackCount)
    {
        switch (stackCount)
        {
            case 1:
                return speedUpDuration;
            case 2:
                return speedUpDuration * 0.95f;
            case 3:
                return speedUpDuration * 0.9f;
            default:
                return 0f;
        }
    }

    public float GetSpeedUpMultiplierByLevel(int stackCount)
    {
        return speedUpMultiplier * stackCount;
    }
}