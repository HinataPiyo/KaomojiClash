using UnityEngine.UIElements;
using Constants.Global;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.KaomojiBuild.Template
{
    /// <summary>
    /// 記号パーツのステータスを表示するテンプレートのクラス
    /// </summary>
    public class StatusParamater
    {
        public const string TEMP_STATUS_PARAMATER = "Temp_StatusParamater";
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
                progressElement = progressBar.Q<VisualElement>(className: "unity-progress-bar__progress");
                defaultColor = Calculation.GetStatusTypeColorByType(statusType);
                progressElement.style.backgroundColor = new StyleColor(defaultColor);

                type = root.Q<Label>("type");
                growthRate = root.Q<Label>("growthrate");

                type.text = Calculation.GetStatusTypeNameByType(statusType);
                growthRate.text = Calculation.GetGrowthRateStar(growthRateType);

                SetInitProgress(statusType);
            }

            public void InitProgress(float min, float max)
            {
                // 表示は絶対値なので 0〜最大絶対値 に固定
                progressBar.lowValue = 0f;
                progressBar.highValue = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));

                // 内部要素取得（1回だけ）
                progressElement ??= progressBar.Q(className: "unity-progress-bar__progress");
            }

            public void SetInitProgress(ENUM.StatusType statusType, float magnification = 1f)
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
                    ? new StyleColor(new Color32(140, 140, 140, 255))
                    : new StyleColor(defaultColor);
            }

            /// <summary>
            /// 成長率タイプを設定する
            /// </summary>
            public void SetGrowthRateType(ENUM.GrowthRateType growthRateType)
            {
                this.growthRateType = growthRateType;
                growthRate.text = Calculation.GetGrowthRateStar(growthRateType);
            }

            public void DisableGrowthRate()
            {
                growthRate.text = string.Empty;
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
            // 初期化のロジックをここに実装

            // box内のVisualElementを取得してelements配列を初期化
            VisualElement root = moduleRoot.Q<VisualElement>(TEMP_STATUS_PARAMATER);
            VisualElement[] elems = root.Query<VisualElement>("box").ToList().ToArray();
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
        KaomojiPart.Guard guard, KaomojiPart.Stamina stamina)
        {
            // ステータス表示のロジックをここに実装
            for (int i = 0; i < elements.Length; i++)
            {
                Element elem = elements[i];
                switch (elem.statusType)
                {
                    case ENUM.StatusType.Speed:
                        elem.SetProgress(speed.GetParameterByLevel(1));
                        elem.SetGrowthRateType(speed.GrowthRateType);
                        break;
                    case ENUM.StatusType.Power:
                        elem.SetProgress(power.GetParameterByLevel(1));
                        elem.SetGrowthRateType(power.GrowthRateType);
                        break;
                    case ENUM.StatusType.Guard:
                        elem.SetProgress(guard.GetParameterByLevel(1));
                        elem.SetGrowthRateType(guard.GrowthRateType);
                        break;
                    case ENUM.StatusType.Stamina:
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
            foreach (Element elem in elements)
            {
                elem.Reset();
            }
        }
    }
}