namespace UI.Battle
{
    using System.Collections.Generic;
    using Constants.Global;
    using ENUM;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class StageProgress : MonoBehaviour, IUIModuleHandler
    {
        [SerializeField] VisualTreeAsset temp_StageProgress;
        List<VisualElement> icons = new List<VisualElement>();
        // icon-value だけでなく icon-container ごと保持して、UXMLのサイズ構造を崩さない
        List<VisualElement> iconContainers = new List<VisualElement>();
        VisualElement line;
        // レイアウト確定後に実測値へ上書きされる初期値
        float baseLineWidth = 100f;
        float iconSpacing = 60f;
        float iconStartLeft = -30f;
        bool geometryReady;

        public void Initialize(VisualElement root)
        {
            VisualElement icon = root.Q<VisualElement>("icon-value");
            VisualElement iconContainer = root.Q<VisualElement>("icon-container");
            line = root.Q<VisualElement>("line");
            icons.Add(icon);
            iconContainers.Add(iconContainer);

            // 初回のレイアウト確定時に line 幅とアイコン実測値を取得する
            line.RegisterCallback<GeometryChangedEvent>(OnLineGeometryChanged);
        }

        /// <summary>
        /// ステージ進行アイコンを追加する。
        /// 最後の1体(isLastSpawn)では line を伸ばさず、終端で余白が増えないようにする。
        /// </summary>
        public void CreateStageProgressIcon(bool isLastSpawn, int enemySpawnCount, Difficulty dif)
        {
            Color color = Calculation.GetColorByDifficulty(dif);
            if (enemySpawnCount == 0)
            {
                icons[0].style.backgroundColor = color;
                return;
            }
            else
            {
                VisualElement temp = temp_StageProgress.Instantiate();
                VisualElement iconContainer = temp.Q<VisualElement>("icon-container");
                VisualElement icon = iconContainer.Q<VisualElement>("icon-value");
                icon.style.backgroundColor = color;

                // 絶対配置で「先頭left + 間隔 * 通しインデックス」の位置へ並べる
                iconContainer.style.position = Position.Absolute;
                iconContainer.style.left = iconStartLeft + iconSpacing * enemySpawnCount;
                line.Add(iconContainer);

                if (!isLastSpawn)
                {
                    UpdateLineWidth(enemySpawnCount);
                }

                icons.Add(icon);
                iconContainers.Add(iconContainer);
            }
        }

        /// <summary>
        /// line のレイアウト確定後に一度だけ実測値を取り、以降の配置/幅計算の基準にする。
        /// </summary>
        void OnLineGeometryChanged(GeometryChangedEvent evt)
        {
            if (geometryReady) return;

            baseLineWidth = line.resolvedStyle.width;

            if (iconContainers.Count > 0 && iconContainers[0] != null)
            {
                VisualElement firstContainer = iconContainers[0];
                float measuredWidth = firstContainer.resolvedStyle.width;
                float measuredLeft = firstContainer.resolvedStyle.left;

                if (measuredWidth > 0f) iconSpacing = measuredWidth;
                if (!float.IsNaN(measuredLeft)) iconStartLeft = measuredLeft;
            }

            geometryReady = true;
            line.UnregisterCallback<GeometryChangedEvent>(OnLineGeometryChanged);
        }

        /// <summary>
        /// 現在のアイコン数を収めるのに必要な幅を計算して line を更新する。
        /// 基準幅より小さくはしない。
        /// </summary>
        void UpdateLineWidth(int enemySpawnCount)
        {
            if (enemySpawnCount <= 0) return;

            float requiredWidth = iconStartLeft + iconSpacing * enemySpawnCount + iconSpacing;
            float newWidth = Mathf.Max(baseLineWidth, requiredWidth);

            line.style.width = newWidth;
        }

        public void NowStage(int progressCount)
        {
            iconContainers[progressCount].AddToClassList("nowStage");
        }

        public void StageClear(int progressCount)
        {
            iconContainers[progressCount - 1].RemoveFromClassList("nowStage");
            icons[progressCount - 1].style.backgroundColor = Color.cyan;
        }
    }
}