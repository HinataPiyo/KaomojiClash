using UnityEngine;
using Constants;
using Constants.Global;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Character/EnemyData")]
public class EnemyData : CharacterData
{
    [Header("基本情報")]
    [SerializeField] KAOMOJI kaomoji;
    public KAOMOJI Kaomoji => kaomoji;

    [Header("敵専用設定")]
    [Tooltip("プレイヤーを発見する範囲")]
    [SerializeField, Range(0.1f, 10f)] float findPlayerRadius = 1.5f;
    public float FindPlayerRadius => findPlayerRadius;

    [Header("敵の行動タイミング")]
    [Tooltip("発射までの待機時間（秒）")]
    [SerializeField, Range(0.1f, 5f)] float launchWaitTime = 1.5f;
    public float LaunchWaitTime => launchWaitTime;

    [Tooltip("発射後の硬直時間（秒）")]
    [SerializeField, Range(0.1f, 3f)] float launchCooldown = 1.0f;
    public float LaunchCooldown => launchCooldown;

    [Tooltip("次の発射までの間隔（秒）")]
    [SerializeField, Range(0.5f, 10f)] float launchInterval = 2.5f;
    public float LaunchInterval => launchInterval;

    [SerializeField] Wave wave;
    public Wave Wave => wave;

    public void SetWaveData(Wave w) => wave = w;
}