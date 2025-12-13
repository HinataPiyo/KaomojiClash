using UnityEngine;
using Constants.Global;

public class CharacterData : ScriptableObject
{
    [SerializeField]CharacterStatus status;
    public CharacterStatus Status => status;
}