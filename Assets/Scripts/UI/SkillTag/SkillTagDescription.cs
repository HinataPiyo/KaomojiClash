namespace UI
{
    using TMPro;
    using UnityEngine;
    
    public class SkillTagDescription : MonoBehaviour
    {
        [SerializeField] Transform parent;
        [SerializeField] GameObject element;

        public void ShowDescription(string[] descriptions, bool isLevel = false)
        {
            foreach(Transform child in parent)
            {
                Destroy(child.gameObject);
            }
            
            int level = 0;
            foreach(string description in descriptions)
            {
                GameObject obj = Instantiate(element, parent);
                SkillTagDescriptionElement elem = obj.GetComponent<SkillTagDescriptionElement>();
                elem.Initialize(description, level, isLevel);
                level++;
            }

        }
    }
}