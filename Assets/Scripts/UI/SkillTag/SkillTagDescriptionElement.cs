namespace UI
{
    using TMPro;
    using UnityEngine;

    public class SkillTagDescriptionElement : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] TextMeshProUGUI levelText;

        public void Initialize(string desc, int level, bool isLevelDisplay = false)
        {
            description.text = desc;
            levelText.text = "Lv" + (level + 1).ToString();
        }
    }
}