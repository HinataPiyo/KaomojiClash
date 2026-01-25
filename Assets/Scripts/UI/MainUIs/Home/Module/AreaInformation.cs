using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Home.Module
{
    
    public class AreaInformation : MonoBehaviour, IUIModuleHandler
    {
        const string PLAY_BUTTON = "PlayButton";
        public void Initialize(VisualElement root)
        {
            Button play = root.Q<VisualElement>(PLAY_BUTTON).Q<Button>();
            play.clicked += PlayButtonOnClick;
        }

        /// <summary>
        /// 出撃が押された時の処理
        /// </summary>
        void PlayButtonOnClick()
        {
            SceneChangeManager.I.ChangeScene(ENUM.Scene.Battle);
        }
    }
}