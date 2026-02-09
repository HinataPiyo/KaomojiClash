using UnityEngine;

[CreateAssetMenu(fileName = "Teppeki", menuName = "SkillTags/Teppeki")]
public class Teppeki : SkillTag
{
    public override string GetDescription(int stackCount)
    {
        switch(stackCount)
        {
            case 1:
            case 2:
            case 3:
                return $"基礎ガードを{GetStatusModifier(stackCount).add}上昇させる。";
            case 4:
            case 5:
                return $"ガードを{GetStatusModifier(stackCount).mul}倍する。基礎ガードを{GetStatusModifier(stackCount).add}上昇する。";
        }

        return string.Empty;
    }
}