using UnityEngine.UIElements;
using Constants.Global;
using Constants;
using UnityEngine;

namespace UI.KaomojiBuild.Template
{
    /// <summary>
    /// 記号パーツのステータスを表示するテンプレートのクラス
    /// </summary>
    public class StatusParamater
    {
        public const string TEMP_STATUS_PARAMATER = "Temp_StatusParamater";
        const string BOX_NAME = "box";
        const string TYPE_LABEL_NAME = "type";
        const string GROWTH_RATE_LABEL_NAME = "growthrate";
        const string PROGRESS_CLASS_NAME = "unity-progress-bar__progress";
        const float BASE_MAGNIFICATION = 1f;
        const float PER_TYPE_STEP_MAGNIFICATION = 0.05f;
        static readonly Color32 NEGATIVE_PROGRESS_COLOR = new Color32(140, 140, 140, 255);
        /// <summary>
        /// ステータス要素のクラス
        /// </summary>
        public class Element
        {
            public ENUM.StatusType statusType;
            public ENUM.GrowthRateType growthRateType;
            public ProgressBar progressBar;
            VisualElement progressElement;
            public Label type;
            public Label growthRate;
            Color defaultColor;

            /// <summary>
            /// 初期化
            /// </summary>
            /// <param name="root">ひとまとまりになっているそのRoot</param>
            /// <param name="number">何番目かのIndex</param>
            public void Initialize(VisualElement root, int number)
            {
                statusType = (ENUM.StatusType)number;
                growthRateType = ENUM.GrowthRateType.None;

                progressBar = root.Q<ProgressBar>();

                progressElement = progressBar.Q<VisualElement>(className: PROGRESS_CLASS_NAME);
                defaultColor = Calculation.GetStatusTypeColorByType(statusType);
                if (progressElement != null)
                {
                    progressElement.style.backgroundColor = new StyleColor(defaultColor);
                }

                type = root.Q<Label>(TYPE_LABEL_NAME);
                growthRate = root.Q<Label>(GROWTH_RATE_LABEL_NAME);

                if (type != null)
                {
                    type.text = Calculation.GetStatusTypeNameByType(statusType);
                }

                if (growthRate != null)
                {
                    growthRate.text = Calculation.GetGrowthRateStar(growthRateType);
                }

                SetInitProgress(ENUM.KaomojiPartType.Mouth);
            }

            public void InitProgress(float min, float max)
            {
                if (progressBar == null) return;

                progressBar.lowValue = 0f;
                progressBar.highValue = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
                progressBar.value = 0f;  // ← この行を追加（初期値を0に設定）

                Debug.Log($"[StatusParamater] {statusType} のProgressBar範囲を設定: min={min:F3}, max={max:F3}, highValue={progressBar.highValue:F3}");

                progressElement ??= progressBar.Q(className: PROGRESS_CLASS_NAME);
            }

            /// <summary>
            /// ProgressBarの上限値と下限値を設定する
            /// KaomojiPartTypeの値に応じて、上限値を0.05ずつ加算して拡張する
            /// </summary>
            public void SetInitProgress(ENUM.KaomojiPartType type)
            {
                float highValue = Calculation.GetPartTypeMultiplier(type, statusType);
                Debug.Log($"[StatusParamater] パーツ表示の上限値を適用: partType={type}, statusType={statusType}, 上限値={highValue:F3}");
                InitProgress(-highValue, highValue);
            }

            public void SetInitProgressByEquippedCount(int equippedPartsCount)
            {
                float magnification = GetMagnification(equippedPartsCount);
                Debug.Log($"[StatusParamater] 合計表示の上限倍率を適用: 装備数={equippedPartsCount}, 倍率={magnification:F3}, statusType={statusType}");
                ApplyProgressRange(magnification);
            }

            void ApplyProgressRange(float magnification)
            {
                switch (statusType)
                {
                    case ENUM.StatusType.Speed:
                        InitProgress(KaomojiPart.Speed.MIN_VALUE * magnification, KaomojiPart.Speed.MAX_VALUE * magnification);
                        break;
                    case ENUM.StatusType.Power:
                        InitProgress(KaomojiPart.Power.MIN_VALUE * magnification, KaomojiPart.Power.MAX_VALUE * magnification);
                        break;
                    case ENUM.StatusType.Guard:
                        InitProgress(KaomojiPart.Guard.MIN_VALUE * magnification, KaomojiPart.Guard.MAX_VALUE * magnification);
                        break;
                    case ENUM.StatusType.Stamina:
                        InitProgress(KaomojiPart.Stamina.MIN_VALUE * magnification, KaomojiPart.Stamina.MAX_VALUE * magnification);
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// プログレスバーの値を設定する
            /// </summary>
            public void SetProgress(float value)
            {
                if (progressBar == null || progressElement == null) return;

                // 表示は絶対値
                progressBar.value = Mathf.Abs(value);

                // 負のときだけ灰色
                const float epsilon = 1e-4f;
                bool isNegative = value < -epsilon;

                progressElement.style.backgroundColor = isNegative
                    ? new StyleColor(NEGATIVE_PROGRESS_COLOR)
                    : new StyleColor(defaultColor);
            }

            /// <summary>
            /// 成長率タイプを設定する
            /// </summary>
            public void SetGrowthRateType(ENUM.GrowthRateType growthRateType)
            {
                this.growthRateType = growthRateType;
                if (growthRate != null)
                {
                    growthRate.text = Calculation.GetGrowthRateStar(growthRateType);
                }
            }

            public void DisableGrowthRate()
            {
                if (growthRate != null)
                {
                    growthRate.text = string.Empty;
                }
            }

            public void Reset()
            {
                SetGrowthRateType(ENUM.GrowthRateType.None);
                SetProgress(0f);
            }

            /// <summary>
            /// これはTotalShowStatusで使用する、装備しているパーツの数に応じてProgressBarの上限値を拡張するための関数
            /// </summary>
            /// <param name="equip"></param>
            /// <returns></returns>
            float GetMagnification(int equip)
            {
                int safeIndex = Mathf.Max(0, equip);
                return BASE_MAGNIFICATION + (safeIndex * PER_TYPE_STEP_MAGNIFICATION);
            }
        }

        Element[] elements;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tempRoot">StatusParamaterが使用されているRoot</param>
        public void Initialize(VisualElement moduleRoot)
        {
            VisualElement root = moduleRoot.Q<VisualElement>(TEMP_STATUS_PARAMATER);
            if (root == null)
            {
                elements = System.Array.Empty<Element>();
                return;
            }

            VisualElement[] elems = root.Query<VisualElement>(BOX_NAME).ToList().ToArray();
            elements = new Element[elems.Length];

            for (int i = 0; i < elems.Length; i++)
            {
                elements[i] = new Element();
                elements[i].Initialize(elems[i], i);
            }
        }

        /// <summary>
        /// ステータス種別ごとの値と成長率を一箇所に集約し、表示側の分岐を減らす
        /// </summary>
        static (float value, ENUM.GrowthRateType growthRateType) GetPartStatusData(
            ENUM.StatusType statusType,
            KaomojiPart.Speed speed,
            KaomojiPart.Power power,
            KaomojiPart.Guard guard,
            KaomojiPart.Stamina stamina,
            float multiplier)
        {
            float rawValue;
            ENUM.GrowthRateType growthRateType;

            switch (statusType)
            {
                case ENUM.StatusType.Speed:
                    rawValue = speed.GetParameterByLevel(1);
                    growthRateType = speed.GrowthRateType;
                    break;
                case ENUM.StatusType.Power:
                    rawValue = power.GetParameterByLevel(1);
                    growthRateType = power.GrowthRateType;
                    break;
                case ENUM.StatusType.Guard:
                    rawValue = guard.GetParameterByLevel(1);
                    growthRateType = guard.GrowthRateType;
                    break;
                case ENUM.StatusType.Stamina:
                    rawValue = stamina.GetParameterByLevel(1);
                    growthRateType = stamina.GrowthRateType;
                    break;
                default:
                    return (0f, ENUM.GrowthRateType.None);
            }

            float statusBaseValue = Calculation.GetStatusBaseValue(statusType);
            if (statusBaseValue <= 0f) return (0f, growthRateType);

            float ratio = multiplier / statusBaseValue;
            return (rawValue * ratio, growthRateType);
        }

        /// <summary>
        /// 合計値表示用の値取得を集約し、ループ内の可読性を優先
        /// </summary>
        static float GetTotalStatusValue(
            ENUM.StatusType statusType,
            float speed,
            float power,
            float guard,
            float stamina)
        {
            switch (statusType)
            {
                case ENUM.StatusType.Speed:
                    return speed;
                case ENUM.StatusType.Power:
                    return power;
                case ENUM.StatusType.Guard:
                    return guard;
                case ENUM.StatusType.Stamina:
                    return stamina;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// ステータスを表示する
        /// </summary>
        public void ShowStatus(KaomojiPart.Speed speed, KaomojiPart.Power power,
        KaomojiPart.Guard guard, KaomojiPart.Stamina stamina, ENUM.KaomojiPartType partType)
        {
            if (elements == null || elements.Length == 0) return;

            Debug.Log($"[StatusParamater] ShowStatus開始: partType={partType}, 要素数={elements.Length}");

            for (int i = 0; i < elements.Length; i++)
            {
                Element elem = elements[i];
                elem.SetInitProgress(partType);

                float multiplier = Calculation.GetPartTypeMultiplier(partType, elem.statusType);

                (float value, ENUM.GrowthRateType growthRateType) =
                    GetPartStatusData(elem.statusType, speed, power, guard, stamina, multiplier);

                elem.SetProgress(value);
                elem.SetGrowthRateType(growthRateType);

                Debug.Log($"[StatusParamater] {elem.statusType} の表示値を反映: value={value:F3}, growthRate={growthRateType}, 計算上限値={multiplier:F3}");
            }
        }

        /// <summary>
        /// 合計ステータスを表示する
        /// </summary>
        public void TotalShowStatus(float speed, float power, float guard, float stamina, int equippedPartsCount = 0)
        {
            if (elements == null || elements.Length == 0) return;

            Debug.Log($"[StatusParamater] TotalShowStatus開始: 装備数={equippedPartsCount}, 要素数={elements.Length}");

            for (int i = 0; i < elements.Length; i++)
            {
                Element elem = elements[i];
                elem.DisableGrowthRate();

                elem.SetInitProgressByEquippedCount(equippedPartsCount);
                elem.SetProgress(GetTotalStatusValue(elem.statusType, speed, power, guard, stamina));
            }
        }

        public void Reset()
        {
            if (elements == null || elements.Length == 0) return;

            foreach (Element elem in elements)
            {
                elem.Reset();
            }
        }
    }
}
