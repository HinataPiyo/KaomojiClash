using System;
using Constants;
using Constants.Global;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : CharacterData 
{
    public float guard;
    public float launchDuration;    // 発射までの待機時間
    // 本体の顔文字   
    [Tooltip("ここでは顔の構成のみを決める"), SerializeField] KAOMOJI kaomoji;     public KAOMOJI Kaomoji => kaomoji;
    [SerializeField] float findPlayerRadius = 2f;    public float FindPlayerRadius => findPlayerRadius;

    public Wave Wave { get; private set; } = new Wave();
    public void SetWaveData(Wave w) => Wave = w;
}