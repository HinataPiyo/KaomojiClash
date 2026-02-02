using UnityEngine;
using Constants;

[CreateAssetMenu(fileName = "AreaData", menuName = "AreaData")]
public class AreaData : ScriptableObject
{
    [SerializeField] AreaBuild area;       public AreaBuild Build => area;
}