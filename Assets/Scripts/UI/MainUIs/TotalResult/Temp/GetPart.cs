namespace UI.TotalResult.Temp
{
    using UnityEngine.UIElements;
    using Constants.Global;

    public class GetPart
    {
        public GetPart(VisualElement tempRoot, HasKaomojiParts p)
        {
            Label icon = tempRoot.Q<VisualElement>("icon-box").Q<Label>("value");
            Label name_value = tempRoot.Q<VisualElement>("info-box").Q<Label>("name-value");
            Label count_value = tempRoot.Q<VisualElement>("count-box").Q<Label>("value");

            ProgressBar dupBar = tempRoot.Q<ProgressBar>();

            icon.text = p.part.Data.part;
            name_value.text = p.part.Data.partName;
            count_value.text = $"x{p.amount}";
            dupBar.highValue = p.part.Data.MaxDup;
            dupBar.value = p.part.Data.CurrentDup;
        }
    }
}