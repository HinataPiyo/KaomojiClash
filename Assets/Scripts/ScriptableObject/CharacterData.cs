using UnityEngine;
using Constants;

public class CharacterData : ScriptableObject
{
    [SerializeField]CharacterStatus status;
    public CharacterStatus Status => status;
}