using UnityEngine;

[CreateAssetMenu(fileName = "Genki", menuName = "SkillTags/Genki")]
public class Genki : SkillTag
{
    public override string GetDescription(int stackCount)
    {
        switch(stackCount)
        {
            case 1:
            case 2:
            case 3:
                return $"基礎スタミナを{GetStatusModifier(stackCount).add}上昇させる。";
            case 4:
            case 5:
                return $"スタミナを{GetStatusModifier(stackCount).mul}倍する。基礎スタミナを{GetStatusModifier(stackCount).add}上昇する。";
        }

        return string.Empty;
    }
}