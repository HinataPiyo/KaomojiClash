namespace UI.Battle
{
    using Constants;

    using UI.Base;
    using UI.TotalResult.Module;
    using UnityEngine.UIElements;

    public class BattleModulesController : ModuleControllerBase
    {
        UIDocument uiDoc;
        const string MODULE_KAOMOJI_COMPOSITIONS = "KaomojiCompositions";
        const string MODULE_STAGE_PROGRESS = "StageProgress";
        const string MODULE_NEXT_STAGE_INFO = "NextStageInfo";
        public KaomojiCompositions module_KC { get; private set; }
        public StageProgress module_SP { get; private set; }
        public NextStageInfo module_NSI { get; private set; }

        void Awake()
        {
            uiDoc = GetComponent<UIDocument>();
            module_KC = GetComponent<KaomojiCompositions>();
            module_SP = GetComponent<StageProgress>();
            module_NSI = GetComponent<NextStageInfo>();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDoc.rootVisualElement;
            CreateHasMoney(root);

            Initialize(module_KC, MODULE_KAOMOJI_COMPOSITIONS, root);
            Initialize(module_SP, MODULE_STAGE_PROGRESS, root);
            Initialize(module_NSI, MODULE_NEXT_STAGE_INFO, root);

            UpdateKaomojiCompositions();
        }

        public void UpdateKaomojiCompositions()
        {
            module_KC.UpdateUI(Context.I.PlayerData.Kaomoji);
        }
    }
}