namespace Constants
{
    using Constants.Global;
    using UnityEngine;
    using ENUM;
    [System.Serializable]
    public sealed class CharacterStatus
    {
        [Header("初期ステータス")]
        [Header("速さ"), Range(1f, 20f)] public float speed = 10f;
        [Header("体力")] public float health = 10f;
        [Header("攻撃力")] public float power = 1f;
        // Gurdは記号の合計値で求めるので初期ステータスを決める段階では不要

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
    public sealed class KAOMOJI
    {
        public const float ColiderXSize = 0.1f;      // コライダーの当たり判定の幅を調整
        public KaomojiPartData eyes;
        public KaomojiPartData mouth;
        public KaomojiPartData hands;
        public KaomojiPartData decoration_first;
        public KaomojiPartData decoration_second;


        // 総合ステータス
        public float Speed { get; private set; }
        public float Power { get; private set; }
        public float Guard { get; private set; }
        public float Stamina { get; private set; }

        public KaomojiPartData[] GetAllPartsData() => new KaomojiPartData[]
        { eyes, mouth, hands, decoration_first, decoration_second };

        public int GetEquippedPartsCount()
        {
            int count = 0;
            KaomojiPartData[] parts = GetAllPartsData();
            foreach(var part in parts)
            {
                if(part != null)
                {
                    count++;
                }
            }
            return count;
        }

        public void SetPartDataByType(KaomojiPartData partData)
        {
            switch(partData.PartType)
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
        /// 記号全体のステータスを更新する
        /// </summary>
        public void UpdateTotalParameter()
        {
            KaomojiPartData[] datas = new KaomojiPartData[]
            { eyes, mouth, hands, decoration_first, decoration_second};

            Speed = Power = Guard = Stamina = 0f;
            foreach(var part in datas)
            {
                // 記号が割り当てられてなければスキップ
                if (part == null || part.Data == null) continue;
                
                int level = part.Data.levelDetail.Level;
                Speed += part.Data.speed.GetParameterByLevel(level);
                Power += part.Data.power.GetParameterByLevel(level);
                Guard += part.Data.guard.GetParameterByLevel(level);
                Stamina += part.Data.stamina.GetParameterByLevel(level);
            }
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
            if(string.IsNullOrEmpty(part) || part.Length <= index)
            {
                return "";
            }

            return part[index].ToString();
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
        }
        

        // レベルごとの効果値
        [System.Serializable]
        public sealed class Speed : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.5f;
            public const float MAX_VALUE = 0.5f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }
        }

        [System.Serializable]
        public sealed class Power : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.7f;
            public const float MAX_VALUE = 0.7f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                // まず増加量を計算
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }
        }

        [System.Serializable]
        public sealed class Guard : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.05f;
            public const float MAX_VALUE = 0.05f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }
        }

        [System.Serializable]
        public sealed class Stamina : IKaomojiPartParameter
        {
            public const float MIN_VALUE = -0.2f;
            public const float MAX_VALUE = 0.2f;
            [Range(MIN_VALUE, MAX_VALUE), SerializeField] float value;
            [SerializeField] GrowthRateType growthRateType;     public GrowthRateType GrowthRateType => growthRateType;
            public float GetParameterByLevel(int level)
            {
                if(level <= 1) return value;
                return value * level + (Calculation.GetGrowthRate(growthRateType) * value);
            }
        }
        
    }
}