using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : CharacterData 
{
    [SerializeField] string kaomoji_Body;     // 本体の顔文字
    public float launchDuration = 0.5f;
    public string Kaomoji_Body => kaomoji_Body;
    [SerializeField] EnemyStatus e_Status;
    public EnemyStatus E_Status => e_Status;


    [Serializable]
    public class EnemyStatus
    {
        [Range(-2f, 2f)] public float speed;
        [Range(-3f, 3f)] public float power;
        [Range(-0.5f, 0.5f)] public float guard;
        [Range(-1f, 1f)] public float stamina;
    }

}