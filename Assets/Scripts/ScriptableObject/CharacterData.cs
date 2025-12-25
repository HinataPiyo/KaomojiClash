using UnityEngine;
using Constants.Global;

public class CharacterData : ScriptableObject
{
    [SerializeField]CharacterStatus status;
    [SerializeField] KAOMOJI kaomoji;
    public CharacterStatus Status => status;
    public KAOMOJI Kaomoji => kaomoji;
}