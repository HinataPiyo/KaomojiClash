using Constants;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Home.Module
{
    
    public class AreaInformation : MonoBehaviour, IUIModuleHandler
    {
        [SerializeField] AreaDatabase areaDatabase;
        [SerializeField] VisualTreeAsset temp_SapawnKaomoji;
        [SerializeField] VisualTreeAsset temp_CultureLevelEntry;
        const string PLAY_BUTTON = "PlayButton";

        ScrollView cultureLevelList;
        Label cultureLevel;
        Label average;
        Label kamojiDensity;
        ScrollView spawnKaomojiList;
        Label nature;

        public void Initialize(VisualElement root)
        {
            Button play = root.Q<VisualElement>(PLAY_BUTTON).Q<Button>();
            play.clicked += PlayButtonOnClick;

            cultureLevelList = root.Q<VisualElement>("culture-tire-list-box").Q<ScrollView>();
            cultureLevel = root.Q<VisualElement>("culture-level-box").Q<Label>("value");
            average = root.Q<VisualElement>("average-level-box").Q<Label>("value");
            kamojiDensity = root.Q<VisualElement>("kaomoji-density-box").Q<Label>("value");
            spawnKaomojiList = root.Q<VisualElement>("center-box").Q<ScrollView>();
            nature = root.Q<Label>("nature-value");

            cultureLevelList.Clear();
            for(int ii = 0; ii < areaDatabase.GetAllAreas().Length; ii++)
            {
                int index = ii; // クロージャ対策
                VisualElement cultureEntry = temp_CultureLevelEntry.Instantiate();
                Button b = cultureEntry.Q<Button>();
                b.clicked += () => UpdateAreaInformation(areaDatabase.GetAllAreas()[index], areaNumber: index);
                b.text = "レベル" + index.ToString();
                cultureLevelList.Add(cultureEntry);
            }

            UpdateAreaInformation(areaDatabase.GetAllAreas()[0]);
        }

        /// <summary>
        /// 出撃が押された時の処理
        /// </summary>
        void PlayButtonOnClick()
        {
            SceneChangeManager.I.ChangeScene(ENUM.Scene.Battle);
        }

        /// <summary>
        /// エリア情報の更新
        /// </summary>
        /// <param name="areaData">エリアデータ</param>
        void UpdateAreaInformation(AreaData data, int areaNumber = 0)
        {
            cultureLevel.text = areaNumber.ToString();
            average.text = "1" + "5"; // 仮
            kamojiDensity.text = (data.Build.kaomojiDensity * 100f).ToString("F1") + "%";
            spawnKaomojiList.Clear();
            foreach(EnemyData enemy in data.Build.spawnDatabase.GetAllEnemyData())
            {
                VisualElement kaomojiEntry = temp_SapawnKaomoji.Instantiate();
                kaomojiEntry.Q<Label>("value").text = enemy.Kaomoji_Body;
                spawnKaomojiList.Add(kaomojiEntry);
            }
            nature.text = "なし"; // 仮
        }
    }
}