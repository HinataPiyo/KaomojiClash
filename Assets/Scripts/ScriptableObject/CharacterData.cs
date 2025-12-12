using UnityEngine;
using Constants.Global;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData", order = 0)]
public class CharacterData : ScriptableObject
{
    [SerializeField]CharacterStatus status;
    public CharacterStatus Status => status;
}