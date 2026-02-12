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

                SetInitProgress(statusType);
            }

            public void InitProgress(float min, float max)
            {
                if (progressBar == null) return;

                progressBar.lowValue = 0f;
                progressBar.highValue = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
                progressBar.value = 0f;  // ← この行を追加（初期値を0に設定）

                progressElement ??= progressBar.Q(className: PROGRESS_CLASS_NAME);
            }

            public void SetInitProgress(ENUM.StatusType statusType, float magnification = 1f, float partTypeMultiplier = 1f)
            {
                switch (statusType)
                {
                    case ENUM.StatusType.Speed:
                        InitProgress(KaomojiPart.Speed.MIN_VALUE * magnification * partTypeMultiplier, KaomojiPart.Speed.MAX_VALUE * magnification * partTypeMultiplier);
                        break;
                    case ENUM.StatusType.Power:
                        InitProgress(KaomojiPart.Power.MIN_VALUE * magnification * partTypeMultiplier, KaomojiPart.Power.MAX_VALUE * magnification * partTypeMultiplier);
                        break;
                    case ENUM.StatusType.Guard:
                        InitProgress(KaomojiPart.Guard.MIN_VALUE * magnification * partTypeMultiplier, KaomojiPart.Guard.MAX_VALUE * magnification * partTypeMultiplier);
                        break;
                    case ENUM.StatusType.Stamina:
                        InitProgress(KaomojiPart.Stamina.MIN_VALUE * magnification * partTypeMultiplier, KaomojiPart.Stamina.MAX_VALUE * magnification * partTypeMultiplier);
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
                    ? new StyleColor(new Color32(140, 140, 140, 255))
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
        }

        Element[] elements;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="tempRoot">StatusParamaterが使用されているRoot</param>
        public void Initialize(VisualElement moduleRoot)
        {
            // box内のVisualElementを取得してelements配列を初期化
            VisualElement root = moduleRoot.Q<VisualElement>(TEMP_STATUS_PARAMATER);

            VisualElement[] elems = root.Query<VisualElement>(BOX_NAME).ToList().ToArray();
            elements = new Element[elems.Length];       // 配列のサイズを設定

            for (int i = 0; i < elems.Length; i++)
            {
                int index = i; // ローカルコピーを作成
                elements[i] = new Element();        // Elementインスタンスを作成
                elements[i].Initialize(elems[index], index);
            }
        }

        /// <summary>
        /// ステータスを表示する
        /// </summary>
        public void ShowStatus(KaomojiPart.Speed speed,KaomojiPart.Power power,
        KaomojiPart.Guard guard, KaomojiPart.Stamina stamina, ENUM.KaomojiPartType partType)
        {
            // ステータス表示のロジックをここに実装
            if (elements == null || elements.Length == 0) return;

            float partTypeMultiplier = Calculation.GetPartTypeMultiplier(partType);

            for (int i = 0; i < elements.Length; i++)
            {
                Element elem = elements[i];
                switch (elem.statusType)
                {
                    case ENUM.StatusType.Speed:
                        elem.SetInitProgress(ENUM.StatusType.Speed, 1f, partTypeMultiplier);
                        elem.SetProgress(speed.GetParameterByLevel(1));
                        elem.SetGrowthRateType(speed.GrowthRateType);
                        break;
                    case ENUM.StatusType.Power:
                        elem.SetInitProgress(ENUM.StatusType.Power, 1f, partTypeMultiplier);
                        elem.SetProgress(power.GetParameterByLevel(1));
                        elem.SetGrowthRateType(power.GrowthRateType);
                        break;
                    case ENUM.StatusType.Guard:
                        elem.SetInitProgress(ENUM.StatusType.Guard, 1f, partTypeMultiplier);
                        elem.SetProgress(guard.GetParameterByLevel(1));
                        elem.SetGrowthRateType(guard.GrowthRateType);
                        break;
                    case ENUM.StatusType.Stamina:
                        elem.SetInitProgress(ENUM.StatusType.Stamina, 1f, partTypeMultiplier);
                        elem.SetProgress(stamina.GetParameterByLevel(1));
                        elem.SetGrowthRateType(stamina.GrowthRateType);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 合計ステータスを表示する
        /// </summary>
        public void TotalShowStatus(float speed, float power, float guard, float stamina, int equippedPartsCount = 0)
        {
            if (elements == null || elements.Length == 0) return;

            for (int i = 0; i < elements.Length; i++)
            {
                Element elem = elements[i];
                elem.DisableGrowthRate();
                switch (elem.statusType)
                {
                    case ENUM.StatusType.Speed:
                        elem.SetInitProgress(ENUM.StatusType.Speed, equippedPartsCount);
                        elem.SetProgress(speed);
                        break;
                    case ENUM.StatusType.Power:
                        elem.SetInitProgress(ENUM.StatusType.Power, equippedPartsCount);
                        elem.SetProgress(power);
                        break;
                    case ENUM.StatusType.Guard:
                        elem.SetInitProgress(ENUM.StatusType.Guard, equippedPartsCount);
                        elem.SetProgress(guard);
                        break;
                    case ENUM.StatusType.Stamina:
                        elem.SetInitProgress(ENUM.StatusType.Stamina, equippedPartsCount);
                        elem.SetProgress(stamina);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Reset()
        {
            // ステータスリセットのロジックをここに実装
            if (elements == null || elements.Length == 0) return;

            foreach (Element elem in elements)
            {
                elem.Reset();
            }
        }
    }
}
