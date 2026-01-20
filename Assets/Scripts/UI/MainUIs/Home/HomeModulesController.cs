using UI.Base;
using UI.Home.Module;
using UnityEngine.UIElements;

namespace UI.Home
{    
    public class HomeModulesController : ModuleControllerBase
    {
        UIDocument uiDocs;
        const string KAOMOJI_PREVIEW_MODULE = "KaomojiPreview";

        public KaomojiPreview module_KP{ get; private set; }

        void Awake()
        {
            uiDocs = GetComponent<UIDocument>();
            module_KP = GetComponent<KaomojiPreview>();
        }

        void Start()
        {
            VisualElement root = uiDocs.rootVisualElement;
            Initialize(module_KP, KAOMOJI_PREVIEW_MODULE, root);
        }
    }
}