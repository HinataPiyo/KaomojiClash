namespace UI.TotalResult.Module
{
    using UnityEngine.UIElements;
    using UnityEngine;
    using Constants.Global;

    public class TotalGetParts : MonoBehaviour, IUIModuleHandler
    {
        ScrollView getPartsList;

        [SerializeField] VisualTreeAsset temp_GetPart;

        public void Initialize(VisualElement moduleRoot)
        {
            getPartsList = moduleRoot.Q<ScrollView>();
            ListClear();
        }

        /// <summary>
        /// UI更新
        /// </summary>
        /// <param name="elems">UIにSettingするにのに必要なData</param>
        public void CreateUI(HasKaomojiParts[] parts)
        {
            foreach (var part in parts)
            {
                VisualElement tempRoot = temp_GetPart.Instantiate();
                new Temp.GetPart(tempRoot, part);
                getPartsList.Add(tempRoot);
            }
        }

        void ListClear()
        {
            getPartsList.Clear();
        }
    }
}