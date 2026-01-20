using UnityEngine;
using UnityEngine.UIElements;
using Constants.Global;

namespace UI.KaomojiBuild.Module
{
    public class FaceLine : MonoBehaviour, IUIModuleHandler
    {
        KaomojiBuildModulesController modulesCtrl;
        void Awake()
        {
            modulesCtrl = GetComponent<KaomojiBuildModulesController>();
        }

        /// <summary>
        /// Managerクラスからこの関数が実行されRoot要素が渡される
        /// </summary>
        public void Initialize(VisualElement moduleRoot)
        {
            VisualElement info = moduleRoot.Q<VisualElement>("info");
            Label icon = info.Q<Label>("icon-value");
            Label name = info.Q<Label>("name-value");
            Label description = info.Q<Label>("description-value");

            CharacterStatus.MentalData mentalData = modulesCtrl.PlayerData.Status.mentalData;
            icon.text = mentalData.faceline;
            name.text = mentalData.name;
            description.text = mentalData.GetEffectDescription();

            VisualElement[] condition = moduleRoot.Query<VisualElement>("condition").ToList().ToArray();

            for (int ii = 0; ii < condition.Length; ii++)
            {
                int index = ii;   // クロージャ対策
                Label level = condition[index].Q<Label>("level");
                Label body = condition[index].Q<Label>("body");

                level.text = $"レベル {index + 1}";
                body.text = CharacterStatus.MentalData.GetConditionBodyByLevel(index + 1);
                condition[index].style.backgroundColor = Color.gray;
            }
        }
    }
}