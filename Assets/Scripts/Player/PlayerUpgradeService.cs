using Constants;
using UnityEngine;
using ENUM;
using System.Collections.Generic;

public class PlayerUpgradeService : MonoBehaviour
{
    public class SkillTagElement
    {
        public SkillTag tag;
        public int stackCount;
    }

    public Status.Params UpgradedStatus { get; private set; } = new Status.Params();
    List<SkillTagElement> totalTags = new List<SkillTagElement>();
    PlayerData data;

    public void Initialize(PlayerData data, List<SkillTag[]> tags)
    {
        this.data = data;
        totalTags.Clear();
        totalTags = SetTags(tags);
        RecalculateUpgrades();
    }

    /// <summary>
    /// アップグレードの再計算
    /// </summary>
    public void RecalculateUpgrades()
    {
        UpgradedStatus = ApplyTagEffects(totalTags.ToArray());
    }

    /// <summary>
    /// タグをセットする
    /// </summary>
    /// <param name="tags">記号に設定されているTagを取得する</param>
    public static List<SkillTagElement> SetTags(List<SkillTag[]> tags)
    {
        List<SkillTagElement> total = new List<SkillTagElement>();
        for (int ii = 0; ii < tags.Count; ii++)
        {
            SkillTag[] tagList = tags[ii];
            for (int jj = 0; jj < tagList.Length; jj++)
            {
                SkillTag tag = tagList[jj];
                if (tag == null) continue;

                // 既に登録されているか確認
                SkillTagElement existingElement = total.Find(e => e.tag == tag);
                if (existingElement != null)
                {
                    // スタック数を増加
                    existingElement.stackCount = Mathf.Min(existingElement.stackCount + 1, 5); // 最大5スタック
                }
                else
                {
                    // 新規登録
                    SkillTagElement newElement = new SkillTagElement
                    {
                        tag = tag,
                        stackCount = 1
                    };
                    total.Add(newElement);
                }
            }
        }

        return total;
    }

    /// <summary>
    /// アップグレードを適用する
    /// </summary>
    /// <param name="baseStatus">基礎ステータス</param>
    /// <param name="upgrades">セット中のSkillTagSOたち</param>
    /// <param name="stackCounts"></param>
    /// <returns></returns>
    public Status.Params ApplyTagEffects(SkillTagElement[] elems)
    {
        // 基礎ステータスをコピー
        UpgradedStatus = new Status.Params
        {
            speed = data.Status.speed,
            power = data.Status.power,
            guard = data.Status.guard,
            stamina = data.Status.health
        };

        // 各アップグレードを適用
        for (int i = 0; i < elems.Length; i++)
        {
            SkillTag upgrade = elems[i].tag;
            int stackCount = elems[i].stackCount;
            SkillTag.StatusModifier modifier = upgrade.GetStatusModifier(stackCount);

            switch (modifier.stat)
            {
                case StatusType.Speed:
                    UpgradedStatus.speed = modifier.ApplyModifier(UpgradedStatus.speed);
                    break;
                case StatusType.Power:
                    UpgradedStatus.power = modifier.ApplyModifier(UpgradedStatus.power);
                    break;
                case StatusType.Guard:
                    UpgradedStatus.guard = modifier.ApplyModifier(UpgradedStatus.guard);
                    break;
                case StatusType.Stamina:
                    UpgradedStatus.stamina = modifier.ApplyModifier(UpgradedStatus.stamina);
                    break;
         
            }
        }

        ApplyPartsEffects();

        return UpgradedStatus;
    }

    /// <summary>
    /// パーツ効果を適用する
    /// </summary>
    void ApplyPartsEffects()
    {
        // 各ステータスに効果を適用
        UpgradedStatus.speed += UpgradedStatus.speed * data.Kaomoji.GetTotalParamByType(StatusType.Speed);
        UpgradedStatus.power += UpgradedStatus.power * data.Kaomoji.GetTotalParamByType(StatusType.Power);
        UpgradedStatus.guard += UpgradedStatus.guard * data.Kaomoji.GetTotalParamByType(StatusType.Guard);
        UpgradedStatus.stamina += UpgradedStatus.stamina * data.Kaomoji.GetTotalParamByType(StatusType.Stamina);

        Debug.Log($"アップグレード後のステータス - 速度: {UpgradedStatus.speed}, 力: {UpgradedStatus.power}, ガード: {UpgradedStatus.guard}, 体力: {UpgradedStatus.stamina}");
    }
}