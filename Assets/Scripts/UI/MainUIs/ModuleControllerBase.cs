using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Base
{
    public class ModuleControllerBase : MonoBehaviour
    {
        [SerializeField] protected PlayerData playerData;
        public PlayerData PlayerData => playerData;
        
        /// <summary>
        /// 各モジュールの初期化を行う
        /// </summary>
        /// <param name="uI">Moduleの初期化に必要なInterface</param>
        /// <param name="name">UIBuilderで設定している名前</param>
        protected virtual void Initialize(IUIModuleHandler uI, string name, VisualElement root)
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
    }
}