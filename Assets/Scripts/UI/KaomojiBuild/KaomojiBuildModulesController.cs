using UI.KaomojiBuild.Module;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.KaomojiBuild
{
    public class KaomojiBuildModulesController : MonoBehaviour
    {
        /// <summary>
        /// moduleの名前
        /// </summary>
        const string MODULE_SELECT_KAOMOJI_PARTS = "SelectKaomojiParts";

        /// <summary>
        /// キャッシュ
        /// </summary>
        UIDocument uiDocument;
        SelectKaomojiParts selectKaomojiPartsModule;

        void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            VisualElement root = uiDocument.rootVisualElement;

            selectKaomojiPartsModule = GetComponent<SelectKaomojiParts>();

            Initialize(selectKaomojiPartsModule, MODULE_SELECT_KAOMOJI_PARTS);
        }

        /// <summary>
        /// 各モジュールの初期化を行う
        /// </summary>
        /// <param name="uI">Moduleの初期化に必要なInterface</param>
        /// <param name="name">UIBuilderで設定している名前</param>
        void Initialize(IUIModuleHandler uI, string name)
        {
            VisualElement root = uiDocument.rootVisualElement;
            VisualElement moduleRoot = root.Q(name);

            if (moduleRoot != null)
            {
                uI.Initialize(moduleRoot);
            }
            else
            {
                Debug.LogError($"[ {name} ] モジュールのルート要素が見つかりません。");
            }
        }
    }
}