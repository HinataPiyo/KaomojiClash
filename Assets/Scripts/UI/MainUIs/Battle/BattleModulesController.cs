namespace UI.Battle
{
    using Constants;

    using UI.Base;
    using UnityEngine.UIElements;

    public class BattleModulesController : ModuleControllerBase
    {
        UIDocument uiDoc;
        const string MODULE_EQUIPMENT_KAOMOJI_PARTS = "EquipKaomojiParts";
        const string MODULE_STAGE_PROGRESS = "StageProgress";
        public EquipKaomojiParts module_EKP { get; private set; }
        public StageProgress module_SP { get; private set; }


        void Awake()
        {
            uiDoc = GetComponent<UIDocument>();
            module_EKP = GetComponent<EquipKaomojiParts>();
            module_SP = GetComponent<StageProgress>();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDoc.rootVisualElement;
            CreateHasMoney(root);
            Initialize(module_EKP, MODULE_EQUIPMENT_KAOMOJI_PARTS, root);
            Initialize(module_SP, MODULE_STAGE_PROGRESS, root);

            UpdateEquipKaomojiParts();
        }

        /// <summary>
        /// 装備している顔文字パーツの情報を更新して表示する
        /// </summary>
        public void UpdateEquipKaomojiParts()
        {
            KAOMOJI kaomoji = Context.I.PlayerData.Kaomoji;
            module_EKP.UpdateUI(kaomoji);
        }
    }
}