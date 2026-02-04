namespace UI.Battle
{
    using UI.Base;
    using UnityEngine.UIElements;

    public class BattleModulesController : ModuleControllerBase
    {
        UIDocument uiDoc;

        void Awake()
        {
            uiDoc = GetComponent<UIDocument>();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDoc.rootVisualElement;
            CreateHasMoney(root);
        }
    }
}