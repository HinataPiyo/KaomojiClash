using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild.Module
{
    public class SelectKaomojiParts : MonoBehaviour, IUIModuleHandler, IUIPartHandler
    {
        // [SerializeField] KaomojiPartsDatabase partsDatabase; // 削除
        [SerializeField] VisualTreeAsset temp_SelectKaomojiParts;

        KaomojiBuildModulesController modulesCtrl;
        VisualElement moduleRoot;
        ScrollView list_view;

        void Awake()
        {
            modulesCtrl = GetComponent<KaomojiBuildModulesController>();
        }

        public void Initialize(VisualElement moduleRoot)
        {
            this.moduleRoot = moduleRoot;
            list_view = moduleRoot.Q<ScrollView>();
            Reset();

            SortByType(ENUM.KaomojiPartType.Mouth);
        }

        public void AssignPart(KaomojiPartData part)
        {
            VisualElement elem = temp_SelectKaomojiParts.Instantiate();
            Button button = elem.Q<Button>();
            Label body = elem.Q<Label>();
            ProgressBar progress = elem.Q<ProgressBar>();

            body.text = part.Data.part;
            button.clicked += () =>
            {
                Debug.Log($"選択されたパーツ: {part.Data.part}");
                modulesCtrl.module_SKP_StatusParamater.AssignPart(part);
                modulesCtrl.module_SD.AssignPart(part);
            };

            bool isMaxDup = part.Data.IsMaxDup();

            if(part.Data.GetIsInitDisplayUI())
            {
                isMaxDup = true;
                progress.highValue = 1;
                progress.value = 1;
            }
            else
            {
                progress.highValue = part.Data.MaxDup;
                progress.value = part.Data.CurrentDup;
            }

            elem.SetEnabled(isMaxDup);
            
            if(isMaxDup) progress.title = $"{part.Data.CurrentDup}";
            else progress.title = $"{part.Data.CurrentDup} / {part.Data.MaxDup}";

            list_view.Add(elem);
        }

        public void SortByType(ENUM.KaomojiPartType type)
        {
            list_view.Clear();
            
            // KaomojiPartsManagerから取得
            var parts = KaomojiPartsManager.Instance.GetPartsByType(type).ToList();
            
            // Enabled=Trueのものを前に、Falseのものを後ろにソート
            parts.Sort((a, b) =>
            {
                bool aEnabled = a.Data.IsMaxDup() || a.Data.GetIsInitDisplayUI();
                bool bEnabled = b.Data.IsMaxDup() || b.Data.GetIsInitDisplayUI();
                return bEnabled.CompareTo(aEnabled);
            });
            
            foreach (var part in parts)
            {
                AssignPart(part);
            }
        }

        public void Reset()
        {
            list_view.Clear();
        }
    }
}