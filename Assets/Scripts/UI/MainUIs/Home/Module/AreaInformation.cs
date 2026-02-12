namespace UI.Home.Module
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using Constants;

    public class AreaInformation : MonoBehaviour, IUIModuleHandler
    {
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
            
            if (AreaManager.I != null && AreaManager.I.AreaDatas != null)
            {
                for (int ii = 0; ii < AreaManager.I.AreaDatas.Length; ii++)
                {
                    int index = ii;
                    VisualElement cultureEntry = temp_CultureLevelEntry.Instantiate();
                    Button b = cultureEntry.Q<Button>();
                    AreaData areaData = AreaManager.I.AreaDatas[index];
                    b.clicked += () => UpdateAreaInformation(areaData);
                    b.text = "レベル" + areaData.Build.cultureLevel.ToString();
                    cultureLevelList.Add(cultureEntry);
                }

                if (AreaManager.I.AreaDatas.Length > 0)
                {
                    UpdateAreaInformation(AreaManager.I.AreaDatas[0]);
                }
            }
        }

        void PlayButtonOnClick()
        {
            AreaManager.I.SetCurrentAreaData(currentAreaIndex);
            SceneChangeManager.I.ChangeScene(ENUM.Scene.Battle);
        }

        void UpdateAreaInformation(AreaData data)
        {
            currentAreaIndex = data.Build.cultureLevel - 1;
            cultureLevel.text = data.Build.cultureLevel.ToString();
            average.text = AreaBuild.GetEnemyAverageLevel(data.Build.cultureLevel).ToString();
            kamojiDensity.text = (data.Build.kaomojiDensity * 100f).ToString("F1") + "%";

            // 出現する敵の情報を簡潔に表示
            spawnKaomojiList.Clear();
            
            var spawnConfig = data.Build.spawnConfig;
            int totalEnemies = spawnConfig.GetTotalSpawnAmount();
            
            // 各難易度の敵数を表示
            foreach (var amountData in spawnConfig.spawnAmounts)
            {
                if (amountData.amount > 0)
                {
                    VisualElement entry = temp_SapawnKaomoji.Instantiate();
                    string difficultyText = $"[{amountData.difficulty}] × {amountData.amount}体";
                    entry.Q<Label>("value").text = difficultyText;
                    spawnKaomojiList.Add(entry);
                }
            }
            
            nature.text = "なし";
        }
    }
}