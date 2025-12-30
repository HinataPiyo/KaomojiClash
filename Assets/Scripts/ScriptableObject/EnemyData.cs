using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : CharacterData 
{
    // 本体の顔文字   
    [SerializeField] string kaomoji_Body;       public string Kaomoji_Body => kaomoji_Body;
    public float launchDuration = 0.5f;
    [SerializeField] float findPlayerRadius = 2f;    public float FindPlayerRadius => findPlayerRadius;
    [SerializeField] EnemyStatus e_Status;      public EnemyStatus E_Status => e_Status;


    [Serializable]
    public class EnemyStatus
    {
        [Range(-2f, 2f)] public float speed;
        [Range(-3f, 3f)] public float power;
        [Range(-0.5f, 0.5f)] public float guard;
        [Range(-1f, 1f)] public float stamina;
    }

}