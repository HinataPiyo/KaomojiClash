using System.Collections.Generic;
using UnityEngine;
using ENUM;

public abstract class SkillTag : ScriptableObject
{
    public class Stack
    {
        public SkillTag tag;
        public int stackCount;
    }

    [System.Serializable]
    public class StatusModifier
    {
        [Header("対象のステータスの種類")]
        public StatusType stat;

        [Header("加算値(基礎ステータスを上昇する)")]
        public float add;
        [Header("乗算値(基礎ステータスに乗算される)")]
        public float mul = 1f;

        public float ApplyAddition(float baseValue)
        {
            return baseValue + add;
        }

        public float ApplyMultiplication(float baseValue)
        {
            return baseValue * mul;
        }

        /// <summary>
        /// 最初に乗算され、その後加算される
        /// </summary>
        public float ApplyModifier(float baseValue)
        {
            float modifiedValue = ApplyMultiplication(baseValue);
            modifiedValue = ApplyAddition(modifiedValue);
            return modifiedValue;
        }
    }

    [Header("このスキルタグが条件付きかどうか")]
    [SerializeField] bool isCondition = false;      public bool IsCondition => isCondition;
    [Header("スキルタグの表示名")]
    [SerializeField] string displayName;
    [Header("最大スタック数")]
    [SerializeField] int maxStacks = 5;

    // 1スタック目〜Nスタック目の効果（画像の 1〜5 に相当）
    [SerializeField] protected List<StatusModifier> stackModifiers = new();

    public string Name => displayName;
    public string[] GetDescriptionsArray()
    {
        string[] descriptions = new string[maxStacks];
        for (int i = 0; i < maxStacks; i++)
        {
            descriptions[i] = GetDescription(i + 1);
        }
        return descriptions;
    }

    /// <summary>
    /// 指定されたスタック数に対応する上昇値を取得する
    /// </summary>
    public StatusModifier GetStatusModifier(int stackCount)
    {
        if (stackCount < 1 || stackCount > maxStacks)
        {
            Debug.LogWarning($"Invalid stack count: {stackCount}. Must be between 1 and {maxStacks}.");
            return default;
        }
        return stackModifiers[stackCount - 1];
    }

    public abstract string GetDescription(int stackCount);
}
