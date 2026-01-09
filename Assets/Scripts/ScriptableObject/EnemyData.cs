using System;
using Constants.Global;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : CharacterData 
{
    // 本体の顔文字   
    [SerializeField] string kaomoji_Body;       public string Kaomoji_Body => kaomoji_Body;
    [SerializeField] float launchDuration = 0.5f;    public float LaunchDuration => launchDuration;
    [SerializeField] float findPlayerRadius = 2f;    public float FindPlayerRadius => findPlayerRadius;
    [SerializeField] EnemyStatus e_Status;      public EnemyStatus E_Status => e_Status;
    [SerializeField] KaomojiPartData[] parts;   public KaomojiPartData[] Parts => parts;

    public Wave Wave { get; private set; } = new Wave();
    public void SetWaveData(Wave w) => Wave = w;

    [Serializable]
    public class EnemyStatus
    {
        [Header("ステータスの強さ")] 
        int level = 1;

        [Header("デフォルトステータス")]
        [Range(-2f, 2f)] public float speed;
        [Range(-3f, 3f)] public float power;
        [Range(-0.5f, 0.5f)] public float guard;
        [Range(-1f, 1f)] public float stamina;

        public int GetLevel() => level;
        public void SetLevel(int value) 
        {
            if(value > 0) level = value;
            else level = 1;     // 下限を1にする
        }
    }

}