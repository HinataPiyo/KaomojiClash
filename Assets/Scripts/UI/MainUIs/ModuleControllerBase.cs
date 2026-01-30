using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Base
{
    public abstract class ModuleControllerBase : MonoBehaviour
    {
        [SerializeField] protected PlayerData playerData;
        public BackPanelButton BackButton { get; private set; }
        public Main.HasMoney HasMoneyDisplay { get; private set; }
        public PlayerData PlayerData => playerData;
        
        /// <summary>
        /// 各モジュールの初期化を行う
        /// </summary>
        /// <param name="uI">Moduleの初期化に必要なInterface</param>
        /// <param name="name">UIBuilderで設定している名前</param>
        protected void Initialize(IUIModuleHandler uI, string name, VisualElement root)
        {
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

        /// <summary>
        /// 戻るボタンの作成
        /// 必要な時にこの関数を呼び出すだけでOK
        /// </summary>
        /// <param name="root">PanelのRoot</param>
        protected void CreateBackButton(VisualElement root)
        {
            // 前のパネルを設定する
            BackButton = new BackPanelButton(ENUM.Panel.Home, root);
        }
        
        /// <summary>
        /// 所持金表示の作成
        /// 必要な時にこの関数を呼び出すだけでOK
        /// </summary>
        /// <param name="root">PanelのRoot</param>
        protected void CreateHasMoney(VisualElement root)
        {
            HasMoneyDisplay = new Main.HasMoney(root);
        }

        /// <summary>
        /// UIDocumentの再構築: GameObjectを非アクティブ→アクティブにした際
        /// UIDocumentが再構築されるため各モジュールの初期化を行う
        /// </summary>
        protected abstract void Initialize();
        protected virtual void OnEnable() => Initialize();
    }
}