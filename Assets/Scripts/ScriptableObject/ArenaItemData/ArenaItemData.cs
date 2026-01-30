using UnityEngine;

public abstract class ArenaItemData : ScriptableObject
{
    [SerializeField] GameObject item_Prefab;    public GameObject Item_Prefab => item_Prefab;
    [SerializeField] Sprite item_Icon;        public Sprite Item_Icon => item_Icon;
    [SerializeField] ENUM.ArenaBuildType item_Type;    public ENUM.ArenaBuildType Item_Type => item_Type;
    [SerializeField] new string name;          public string Name => name;
    [SerializeField] int price = 100;

    public ENUM.ArenaItemGradeType GradeType { get; private set; } = ENUM.ArenaItemGradeType.None;
    public Vector2 SetPosition { get; set; } = Vector2.zero;

    public abstract string GetDiscription();
    public float GetPrice()
    {
        switch(GradeType)
        {
            case ENUM.ArenaItemGradeType.None:
                return price;
            case ENUM.ArenaItemGradeType.MK_ONE:
                return price * 1.5f;
            case ENUM.ArenaItemGradeType.MK_TWO:
                return price * 2f;
            default:
                return price;
        }
    }

    public void SetGradeType(ENUM.ArenaItemGradeType gradeType)
    {
        GradeType = gradeType;
    }
}