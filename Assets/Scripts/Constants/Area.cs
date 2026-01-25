namespace Constants
{
    using ENUM;

    [System.Serializable]
    public class AreaBuild
    {
        public string name;                 // アリーナ名
        public Difficulty difficulty;       // 難易度
        public EnemyDatabase spawnDatabase; // 出現する敵のデータベース
        public int cultureLevel;            // 文化レベル
        public float kaomojiDensity;        // 顔文字密度
    }
}