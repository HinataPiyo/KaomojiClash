namespace Constants
{
    using UnityEngine;

    public static class DamageCalc
    {
        // defence: ガード値（0以上）
        // K: 逓減の曲線、rMax: 軽減率上限
        public static float CalcTakenDamage(float rawDamage, float defence, float K = 200f, float rMax = 0.60f)
        {
            rawDamage = Mathf.Max(0f, rawDamage);
            defence   = Mathf.Max(0f, defence);

            float r = defence / (defence + K);   // 0..1に近づく逓減
            r = Mathf.Min(r, rMax);              // キャップ

            float taken = rawDamage * (1f - r);
            return Mathf.Max(1f, taken);         // 1ダメ保証（任意：事故対策）
        }

        // 追加軽減(例：スキルや状況)を乗算で重ねる例
        public static float ApplyExtraReductions(float damage, params float[] reductions01)
        {
            float d = damage;
            foreach (var red in reductions01)
            {
                float r = Mathf.Clamp01(red);
                d *= (1f - r);
            }
            return Mathf.Max(1f, d); // 任意
        }
    }
}