namespace Constants
{
    using Constants.Global;
    using UnityEngine;
    using ENUM;
    using System.Collections.Generic;

    [System.Serializable]
    public sealed class CharacterStatus
    {
        [Header("初期ステータス")]
        [Tooltip("速さ"), Range(1f, 20f)] public float speed = 10f;
        [Tooltip("体力")] public float health = 10f;
        [Tooltip("攻撃力")] public float power = 1f;
        [Tooltip("防御力")] public float guard = 1f;

        [Header("Movement")]
        public float maxDragDistance = 3f;       // ドラッグ最大距離
        public float minLaunchDistance = 0.1f;   // これ未満なら発射しない

        [Header("Landing / Cooldown")]
        public float landingCooldown = 0.25f;    // 着地硬直時間(=CT)

        [Header("Reflect")]
        public float reflectPower = 1f;         // 反射時の速度倍率

        [Header("MentalPower")]
        public MentalData mentalData;
    }

    [System.Serializable]
    public class Status
    {
        public class Params
        {
            // 総合ステータス
            public float speed;
            public float power;
            public float guard;
            public float stamina;
        }

        public Params m_params { get; private set; } = new Params();

        /// <summary>
        /// 記号全体のステータスを更新する
        /// レベルに応じた値を加算していく
        /// 記号ステータスのパーセント値を基礎ステータスに乗算して値を算出する
        /// </summary>
        public Params UpdateTotalPartsParameter(KaomojiPartData[] datas)
        {
            m_params.speed = m_params.power = m_params.guard = m_params.stamina = 0f;
            foreach (var part in datas)
            {
                // 記号が割り当てられてなければスキップ
                if (part == null || part.Data == null) continue;

                int level = part.Data.levelDetail.Level;
                m_params.speed += part.Data.speed.GetParameterByLevel(level);
                m_params.power += part.Data.power.GetParameterByLevel(level);
                m_params.guard += part.Data.guard.GetParameterByLevel(level);
                m_params.stamina += part.Data.stamina.GetParameterByLevel(level);
            }

            return m_params;
        }
    }



    [System.Serializable]
    public sealed class KAOMOJI
    {
        public const float ColiderXSize = 0.1f;      // コライダーの当たり判定の幅を調整
        public KaomojiPartData mouth;
        public KaomojiPartData eyes;
        public KaomojiPartData hands;
        public KaomojiPartData decoration_first;
        public KaomojiPartData decoration_second;

        public KaomojiPartData[] GetAllPartsData() => new KaomojiPartData[]
        { mouth, eyes, hands, decoration_first, decoration_second };

        public void AllResetPartsLevel()
        {
            KaomojiPartData[] parts = GetAllPartsData();
            foreach (var part in parts)
            {
                part?.Data.levelDetail.ResetLevel();
            }
        }

        public List<SkillTag[]> GetAllSkillTags()
        {
            List<SkillTag[]> allTags = new List<SkillTag[]>();
            KaomojiPartData[] parts = GetAllPartsData();
            foreach (var part in parts)
            {
                if (part != null)
                {
                    allTags.Add(part.Data.SkillTags);
                }
            }

            return allTags;
        }

        public int GetEquippedPartsCount()
        {
            int count = 0;
            KaomojiPartData[] parts = GetAllPartsData();
            foreach (var part in parts)
            {
                if (part != null)
                {
                    count++;
                }
            }
            return count;
        }

        public void SetPartDataByType(KaomojiPartData partData)
        {
            switch (partData.PartType)
            {
                case KaomojiPartType.Eyes:
                    eyes = partData;
                    break;
                case KaomojiPartType.Mouth:
                    mouth = partData;
                    break;
                case KaomojiPartType.Hands:
                    hands = partData;
                    break;
                case KaomojiPartType.Decoration_First:
                    decoration_first = partData;
                    break;
                case KaomojiPartType.Decoration_Second:
                    decoration_second = partData;
                    break;
            }
        }

        /// <summary>
        /// 顔文字の組み立て
        /// </summary>
        /// <returns>設定された顔文字のPartsを合体させたもの</returns>
        public string BuildKaomoji(MentalData mentalData)
        {
            string _left_faceline = SeparatePart(mentalData?.faceline, 0);
            string _right_faceline = SeparatePart(mentalData?.faceline, 1);
            string _left_eye = SeparatePart(eyes?.Data.part, 0);
            string _right_eye = SeparatePart(eyes?.Data.part, 1);
            string _mouth = mouth?.Data.part;
            // string left_hands = SeparatePart(K.hands.Data.part, 0);
            // string right_hands = SeparatePart(K.hands.Data.part, 1);

            string merged = _left_faceline + _left_eye + _mouth + _right_eye + _right_faceline;
            return merged;
        }

        /// <summary>
        /// 各ステータスの初期パラメータを取得
        /// </summary>
        public float GetInitialParam(StatusType type)
        {
            float value = 0f;
            foreach (var part in GetAllPartsData())
            {
                // 記号が割り当てられてなければスキップ
                if (part == null) continue;
                switch (type)
                {
                    case StatusType.Speed:
                        value += part.Data.speed.GetInitialParam();
                        break;
                    case StatusType.Power:
                        value += part.Data.power.GetInitialParam();
                        break;
                    case StatusType.Guard:
                        value += part.Data.guard.GetInitialParam();
                        break;
                    case StatusType.Stamina:
                        value += part.Data.stamina.GetInitialParam();
                        break;
                }
            }

            return value;
        }

        public float GetTotalParamByType(StatusType type)
        {
            float value = 0f;
            foreach (var part in GetAllPartsData())
            {
                // 記号が割り当てられてなければスキップ
                if (part == null) continue;
                int level = part.Data.levelDetail.Level;
                switch (type)
                {
                    case StatusType.Speed:
                        value += part.Data.speed.GetParameterByLevel(level);
                        break;
                    case StatusType.Power:
                        value += part.Data.power.GetParameterByLevel(level);
                        break;
                    case StatusType.Guard:
                        value += part.Data.guard.GetParameterByLevel(level);
                        break;
                    case StatusType.Stamina:
                        value += part.Data.stamina.GetParameterByLevel(level);
                        break;
                }
            }

            return value;
        }

        /// <summary>
        /// パーツの左右分割
        /// </summary>
        /// <param name="part">SOで設定した記号</param>
        /// <param name="index">左右どちらか(0=左側、1=右側)</param>
        /// <returns></returns>
        public string SeparatePart(string part, int index)
        {
            // index: 0=左側、1=右側
            // partがnullまたは空文字、indexが範囲外の場合は空文字を返す
            if (string.IsNullOrEmpty(part) || part.Length <= index)
            {
                return "";
            }

            return part[index].ToString();
        }

        // 既存のKAOMOJI構造体内に以下を追加

        /// <summary>
        /// 目パーツを設定
        /// </summary>
        public void SetEyes(KaomojiPartData part)
        {
            eyes = part;
        }

        /// <summary>
        /// 口パーツを設定
        /// </summary>
        public void SetMouth(KaomojiPartData part)
        {
            mouth = part;
        }

        /// <summary>
        /// 手パーツを設定
        /// </summary>
        public void SetHands(KaomojiPartData part)
        {
            hands = part;
        }

        /// <summary>
        /// 装飾1パーツを設定
        /// </summary>
        public void SetDecorationFirst(KaomojiPartData part)
        {
            decoration_first = part;
        }

        /// <summary>
        /// 装飾2パーツを設定
        /// </summary>
        public void SetDecorationSecond(KaomojiPartData part)
        {
            decoration_second = part;
        }
    }

    [System.Serializable]
    public sealed class KaomojiPart
    {
        /// <summary>
        /// 顔文字のパーツが持つステータスはこんだけ
        /// </summary>
        public string partName;
        public string part;
        public LevelDetail levelDetail;

        // %で計算(例: 0.1なら10%UP)
        [Tooltip("移動速度に影響")] public Speed speed;
        [Tooltip("攻撃力に影響")] public Power power;
        [Tooltip("防御力に影響")] public Guard guard;
        [Tooltip("体力に影響")] public Stamina stamina;
        [SerializeField] int maxDup = 50;               // 最大重複数
        [SerializeField] bool isInitDisplay = false;    // UIに表示するかどうか
        [SerializeField] SkillTag[] skillTag;           // このパーツに対応するスキルタグ
        public SkillTag[] SkillTags => skillTag;


        /// <summary>
        /// 初期表示するかどうか
        /// </summary>
        public bool GetIsInitDisplayUI() => isInitDisplay;
        public int CurrentDup { get; private set; } = 0;  // 現在の重複数
        public int MaxDup => maxDup;
        public void AddDup(int amount)
        {
            CurrentDup += amount;
            // if(CurrentDup > maxDup)
            // {
            //     CurrentDup = maxDup;
            // }
        }

        /// <summary>
        /// 最大重複数に達しているかどうか
        /// </summary>
        public bool IsMaxDup() => CurrentDup >= maxDup;

        [System.Serializable]
        public class LevelDetail
        {
            [SerializeField] GrowthRateType growthRateType;
            public int Level { get; private set; } = 1;
            public float Exp { get; private set; } = 0f;

            public void AddExperience(float exp)
            {
                Exp += exp;
                CheckLevelUp(); // 引数は不要
            }

            // 複数レベルアップ対応
            public void CheckLevelUp()
            {
                // 無限ループ防止：必要経験値が0以下を返す設計は不可
                int safety = 1000;

                /*
                    経験値テーブルが壊れていて needExp = 0 を返した場合
                        → while (Exp >= need) が永久ループする
                        → ゲームがフリーズ
                */
                while (safety-- > 0)
                {
                    float need = GetNeedExpBorder(Level);

                    if (need <= 0f)
                    {
                        // テーブル不正。ここは例外でもログでも良い
                        break;
                    }

                    if (Exp < need) break;

                    Exp -= need;  // ここが重要：getExpではなく必要量を引く
                    Level++;
                }
            }

            public float GetNeedExpBorder(int level)
            {
                return Calculation.GetNeedExpBorderByLevelAndGrowthRateType(level, growthRateType);
            }

            public void ResetLevel()
            {
                Level = 1;
                Exp = 0f;
            }
        }


        // レベルごとの効果値
        [System.Serializable]
        public sealed class Speed : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.2f;
            public const float MAX_VALUE = 0.2f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType; public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if (level <= 1) return value;
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }

            public float GetInitialParam() => value;
        }

        [System.Serializable]
        public sealed class Power : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.7f;
            public const float MAX_VALUE = 0.7f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType; public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if (level <= 1) return value;
                // まず増加量を計算
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }

            public float GetInitialParam() => value;
        }

        [System.Serializable]
        public sealed class Guard : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.05f;
            public const float MAX_VALUE = 0.05f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType; public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if (level <= 1) return value;
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }

            public float GetInitialParam() => value;
        }

        [System.Serializable]
        public sealed class Stamina : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.2f;
            public const float MAX_VALUE = 0.2f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType; public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if (level <= 1) return value;
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }

            public float GetInitialParam() => value;
        }

    }


}