namespace UI.TotalResult.Temp
{
    using UnityEngine.UIElements;
    using Constants.Global;

    public class KaomojiComposition
    {
        public KaomojiComposition(VisualElement tempRoot, ENUM.KaomojiPartType type, int level, string part)
        {
            Label type_value = tempRoot.Q<Label>("type");
            Label level_value = tempRoot.Q<Label>("level");
            Label part_value = tempRoot.Q<Label>("part");

            type_value.text = Calculation.GetKaomojiPartTypeName(type);
            level_value.text = $"Lv{level}";
            part_value.text = part;
        }
    }
}