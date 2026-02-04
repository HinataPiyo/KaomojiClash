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
        int currentAreaIndex = 0;

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
                AreaData areaData = areaDatabase.GetAllAreas()[index].data;
                b.clicked += () => UpdateAreaInformation(areaData);
                b.text = "レベル" + areaData.Build.cultureLevel.ToString();     // 文化圏レベルを表示
                cultureLevelList.Add(cultureEntry);
            }

            UpdateAreaInformation(areaDatabase.GetAllAreas()[0].data);
        }

        /// <summary>
        /// 出撃が押された時の処理
        /// </summary>
        void PlayButtonOnClick()
        {
            AreaManager.I.SetCurrentAreaData(currentAreaIndex);
            SceneChangeManager.I.ChangeScene(ENUM.Scene.Battle);
        }

        /// <summary>
        /// エリア情報の更新
        /// </summary>
        /// <param name="areaData">エリアデータ</param>
        void UpdateAreaInformation(AreaData data)
        {
            currentAreaIndex = data.Build.cultureLevel - 1;
            cultureLevel.text = data.Build.cultureLevel.ToString();     // 文化圏レベルを表示
            average.text = AreaBuild.GetEnemyAverageLevel(data.Build.cultureLevel).ToString();      // 敵の平均レベルを表示
            kamojiDensity.text = (data.Build.kaomojiDensity * 100f).ToString("F1") + "%";

            // 出現する顔文字リストの更新
            spawnKaomojiList.Clear();
            foreach(EnemyData enemy in data.Build.spawnDatabase.GetAllEnemyData())
            {
                VisualElement kaomojiEntry = temp_SapawnKaomoji.Instantiate();
                kaomojiEntry.Q<Label>("value").text = enemy.Kaomoji.BuildKaomoji(enemy.Status.mentalData);
                spawnKaomojiList.Add(kaomojiEntry);
            }
            nature.text = "なし"; // 仮
        }
    }
}