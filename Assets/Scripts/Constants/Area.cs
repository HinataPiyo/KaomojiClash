namespace Constants
{
    using ENUM;

    [System.Serializable]
    public class AreaBuild
    {
        public EnemyDatabase spawnDatabase; // 出現する敵のデータベース
        public int cultureLevel;            // 文化圏レベル
        public float kaomojiDensity;        // 顔文字密度(%表記)
    }
}