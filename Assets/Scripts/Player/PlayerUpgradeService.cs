using Constants;
using UnityEngine;
using ENUM;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// プレイヤーのアップグレード管理クラス
/// </summary>
public class PlayerUpgradeService : MonoBehaviour
{
    PlayerMovement movement;
    TmpAfterImageTrail afterImageTrail;
    public Status.Params UpgradedStatus { get; private set; } = new Status.Params();
    public List<SkillTag.Stack> TotalTags { get; private set; } = new List<SkillTag.Stack>();
    public List<SkillTag.Stack> ConditionTags { get; private set; } = new List<SkillTag.Stack>();
    PlayerData data;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        afterImageTrail = GetComponent<TmpAfterImageTrail>();
    }

    public void Initialize(PlayerData data, List<SkillTag[]> tags)
    {
        this.data = data;
        TotalTags.Clear();
        TotalTags = SetTags(tags);
        ConditionTags.Clear();
        ConditionTags = SetConditionTags(tags);
        RecalculateUpgrades();
    }

    /// <summary>
    /// アップグレードの再計算
    /// </summary>
    public void RecalculateUpgrades()
    {
        UpgradedStatus = ApplyTagEffects(TotalTags.ToArray());
    }

    /// <summary>
    /// タグをセットする
    /// </summary>
    /// <param name="tags">記号に設定されているTagを取得する</param>
    public static List<SkillTag.Stack> SetTags(List<SkillTag[]> tags)
    {
        List<SkillTag.Stack> total = new List<SkillTag.Stack>();
        for (int ii = 0; ii < tags.Count; ii++)
        {
            SkillTag[] tagList = tags[ii];      // 各記号に設定されているタグ群
            for (int jj = 0; jj < tagList.Length; jj++)
            {
                SkillTag tag = tagList[jj];
                if (tag == null) continue;

                // 既に登録されているか確認
                SkillTag.Stack existingElement = total.Find(e => e.tag == tag);

                if (existingElement != null)
                {
                    // スタック数を増加
                    existingElement.stackCount = Mathf.Min(existingElement.stackCount + 1, 5); // 最大5スタック
                }
                else
                {
                    // 新規登録
                    SkillTag.Stack newElement = new SkillTag.Stack
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

    public static List<SkillTag.Stack> SetConditionTags(List<SkillTag[]> tags)
    {
        List<SkillTag.Stack> conditionTags = new List<SkillTag.Stack>();
        for (int ii = 0; ii < tags.Count; ii++)
        {
            SkillTag[] tagList = tags[ii];      // 各記号に設定されているタグ群
            for (int jj = 0; jj < tagList.Length; jj++)
            {
                SkillTag tag = tagList[jj];
                if (tag == null || !tag.IsCondition) continue;

                // 既に登録されているか確認
                SkillTag.Stack existingElement = conditionTags.Find(e => e.tag == tag);

                if (existingElement != null)
                {
                    // スタック数を増加
                    existingElement.stackCount = Mathf.Min(existingElement.stackCount + 1, 5); // 最大5スタック
                }
                else
                {
                    // 新規登録
                    SkillTag.Stack newElement = new SkillTag.Stack
                    {
                        tag = tag,
                        stackCount = 1
                    };
                    conditionTags.Add(newElement);
                }
            }
        }

        return conditionTags;
    }

    /// <summary>
    /// アップグレードを適用する
    /// </summary>
    /// <param name="baseStatus">基礎ステータス</param>
    /// <param name="upgrades">セット中のSkillTagSOたち</param>
    /// <param name="stackCounts"></param>
    /// <returns></returns>
    public Status.Params ApplyTagEffects(SkillTag.Stack[] elems)
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

            if(upgrade.IsCondition) continue;   // 条件付きスキルタグは除外

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

        ApplyPartsEffects();        // パーツ効果を適用

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
    }

    /// <summary>
    /// 一定時間速度を上げる効果を開始する
    /// </summary>
    /// <param name="duration">継続時間</param>
    /// <param name="speedMultiplier">速度上昇値(%)</param>
    public void StartSpeedUpEffect(float duration, float speedMultiplier)
    {
        StartCoroutine(ApplySpeedUpEffect(duration, speedMultiplier));
    }

    IEnumerator ApplySpeedUpEffect(float duration, float speedMultiplier)
    {
        afterImageTrail.SetActive(true);
        movement.SpeedUp(speedMultiplier);
        yield return new WaitForSeconds(duration);
        movement.ResetSpeedUp();
        afterImageTrail.SetActive(false);
    }
}