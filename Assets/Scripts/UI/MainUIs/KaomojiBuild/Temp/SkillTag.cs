using UnityEngine.UIElements;

namespace UI.KaomojiBuild.Template
{
    public class SkillTagUI
    {
        Button button;
        Label name;
        Label stackCount;

        public SkillTagUI(VisualElement tempRoot, SkillTag tag, int stack = 1)
        {
            name = tempRoot.Q<Label>("name");
            stackCount = tempRoot.Q<Label>("stack-count");
            button = tempRoot.Q<Button>();

            stackCount.style.display = DisplayStyle.None; // 初期状態では非表示/記号のステータス表示の時は非表示
            name.text = tag.Name;

            if(stack > 1)
            {
                stackCount.style.display = DisplayStyle.Flex;
                stackCount.text = $"Lv{stack}";
            }
        }
    }
}