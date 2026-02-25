namespace UI.Battle
{
    using UI.Base;
    using UI.TotalResult.Module;
    using UnityEngine.UIElements;

    public class BattleModulesController : ModuleControllerBase
    {
        UIDocument uiDoc;

        const string KAOMOJI_COMPOSITIONS = "KaomojiCompositions";
        public KaomojiCompositions module_KC { get; private set; }

        void Awake()
        {
            uiDoc = GetComponent<UIDocument>();
            module_KC = GetComponent<KaomojiCompositions>();
        }

        void Start()
        {
            UpdateKaomojiCompositions();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDoc.rootVisualElement;
            CreateHasMoney(root);

            Initialize(module_KC, KAOMOJI_COMPOSITIONS, root);
        }

        public void UpdateKaomojiCompositions() => module_KC.UpdateUI(Context.I.PlayerData.Kaomoji);
    }
}