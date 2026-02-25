namespace UI.Battle
{
    using Constants;

    using UI.Base;
    using UI.TotalResult.Module;
    using UnityEngine.UIElements;

    public class BattleModulesController : ModuleControllerBase
    {
        UIDocument uiDoc;
        const string MODULE_EQUIPMENT_KAOMOJI_PARTS = "EquipKaomojiParts";
        const string MODULE_STAGE_PROGRESS = "StageProgress";
        const string MODULE_NEXT_STAGE_INFO = "NextStageInfo";
        public EquipKaomojiParts module_EKP { get; private set; }
        public StageProgress module_SP { get; private set; }
        public NextStageInfo module_NSI { get; private set; }


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