namespace UI.Battle
{
    using Constants;

    using UI.Base;
    using UnityEngine.UIElements;

    public class BattleModulesController : ModuleControllerBase
    {
        UIDocument uiDoc;
        const string MODULE_EQUIPMENT_KAOMOJI_PARTS = "EquipKaomojiParts";
        public EquipKaomojiParts module_EKP { get; private set; }

        void Awake()
        {
            uiDoc = GetComponent<UIDocument>();
            module_EKP = GetComponent<EquipKaomojiParts>();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDoc.rootVisualElement;
            CreateHasMoney(root);
            Initialize(module_EKP, MODULE_EQUIPMENT_KAOMOJI_PARTS, root);

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