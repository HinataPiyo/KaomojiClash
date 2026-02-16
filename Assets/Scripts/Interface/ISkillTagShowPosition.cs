using UnityEngine.UIElements;
interface ISkillTagShowPosition
{
    public void RegisterTagAndCreateDescription(VisualElement elem, SkillTag tag);
    void CreateTagAtPosition(VisualElement elem, SkillTag tag);
}