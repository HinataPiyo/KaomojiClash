using UnityEngine;

public abstract class ArenaItemData : ScriptableObject
{
    [SerializeField] GameObject item_Prefab;    public GameObject Item_Prefab => item_Prefab;
    [SerializeField] Sprite item_Icon;        public Sprite Item_Icon => item_Icon;
    [SerializeField] ENUM.ArenaBuildType item_Type;    public ENUM.ArenaBuildType Item_Type => item_Type;
    [SerializeField] new string name;          public string Name => name;
    [SerializeField] int price = 100;
    [SerializeField] int max_First_UsageCount = 30;    public int Max_First_UsageCount => max_First_UsageCount;

    public ENUM.ArenaItemGradeType GradeType { get; private set; } = ENUM.ArenaItemGradeType.None;
    public Vector2 SetPosition { get; set; } = Vector2.zero;

    public int UsageCount { get; private set; } = 0;

    public abstract string GetDiscription();
    public int GetPrice()
    {
        switch(GradeType)
        {
            case ENUM.ArenaItemGradeType.None:
                return price;
            case ENUM.ArenaItemGradeType.MK_ONE:
                return Mathf.FloorToInt(price * 1.5f);
            case ENUM.ArenaItemGradeType.MK_TWO:
                return Mathf.FloorToInt(price * 2f);
            default:
                return price;
        }
    }

    public void SetGradeType(ENUM.ArenaItemGradeType gradeType)
    {
        GradeType = gradeType;
    }

    public int GetMaxUsageCountByGrade()
    {
        switch (GradeType)
        {
            case ENUM.ArenaItemGradeType.None:
                return max_First_UsageCount;
            case ENUM.ArenaItemGradeType.MK_ONE:
                return Mathf.FloorToInt(max_First_UsageCount * 1.5f);
            case ENUM.ArenaItemGradeType.MK_TWO:
                return max_First_UsageCount * 2;
            default:
                return max_First_UsageCount;
        }
    }
}