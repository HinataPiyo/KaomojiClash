using Constants;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Home.Module
{
    
    public class AreaInformation : MonoBehaviour, IUIModuleHandler
    {
        const string PLAY_BUTTON = "PlayButton";
        Label areaName;
        Label difficulty;
        Label cultureLevel;
        Label kaomojiDensity;


        public void Initialize(VisualElement root)
        {
            Button play = root.Q<VisualElement>(PLAY_BUTTON).Q<Button>();
            play.clicked += PlayButtonOnClick;

            areaName = root.Q<VisualElement>("title-box").Q<Label>("value");
            difficulty = root.Q<VisualElement>("difficulty-box").Q<Label>("value");
            cultureLevel = root.Q<VisualElement>("culture-level-box").Q<Label>("value");
            kaomojiDensity = root.Q<VisualElement>("kaomoji-density-box").Q<Label>("value");

            AreaBuild area = AreaManager.I.CurrentAreaData.AreaBuild;
            areaName.text = area.name;
            difficulty.text = area.difficulty.ToString().ToUpper();
            cultureLevel.text = area.cultureLevel.ToString();
            kaomojiDensity.text = "+" + (area.kaomojiDensity * 100) + "%";
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