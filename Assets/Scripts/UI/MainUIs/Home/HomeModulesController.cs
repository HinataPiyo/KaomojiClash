using UI.Base;
using UI.Home.Module;
using UnityEngine.UIElements;

namespace UI.Home
{    
    public class HomeModulesController : ModuleControllerBase
    {
        UIDocument uiDocs;
        const string KAOMOJI_PREVIEW_MODULE = "KaomojiPreview";
        const string BUILD_BUTTONS_MODULE = "BuildButtons";
        const string AREA_INFORMATION_MODULE = "AreaInformation";

        public KaomojiPreview module_KP{ get; private set; }
        public BuildButtons module_BB{ get; private set; }
        public AreaInformation module_AI{ get; private set; }


        void Awake()
        {
            uiDocs = GetComponent<UIDocument>();
            module_KP = GetComponent<KaomojiPreview>();
            module_BB = GetComponent<BuildButtons>();
            module_AI = GetComponent<AreaInformation>();
        }

        protected override void Initialize()
        {
            VisualElement root = uiDocs.rootVisualElement;
            CreateHasMoney(root);
            Initialize(module_KP, KAOMOJI_PREVIEW_MODULE, root);
            Initialize(module_BB, BUILD_BUTTONS_MODULE, root);
            Initialize(module_AI, AREA_INFORMATION_MODULE, root);
        }
    }
}