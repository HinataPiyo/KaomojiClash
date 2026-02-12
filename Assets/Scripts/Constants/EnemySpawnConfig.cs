// namespace Constants
// {
//     using UnityEngine;
//     using ENUM;
//     using System.Collections.Generic;

//     // ... AreaBuildクラスの後に定義されています ...

//     /// <summary>
//     /// 敵出現設定
//     /// </summary>
//     [System.Serializable]
//     public class EnemySpawnConfig
//     {
//         [Header("生成モード")]
//         public GenerationMode mode = GenerationMode.SemiAuto;

//         // [Header("固定敵設定（半自動/手動モード）")]
//         [Tooltip("固定で出現させる敵データ")]
//         public List<EnemyData> fixedEnemies = new List<EnemyData>();

//         [Header("除外設定（自動/半自動モード）")]
//         [Tooltip("自動生成時に除外す��部位タイプ")]
//         public List<KaomojiPartType> excludeTypes = new List<KaomojiPartType>();

//         [Header("難易度別出現数")]
//         public DifficultySpawnAmount[] spawnAmounts = new DifficultySpawnAmount[]
//         {
//             new DifficultySpawnAmount { difficulty = Difficulty.Easy, amount = 2 },
//             new DifficultySpawnAmount { difficulty = Difficulty.Normal, amount = 3 },
//             new DifficultySpawnAmount { difficulty = Difficulty.Hard, amount = 2 },
//             new DifficultySpawnAmount { difficulty = Difficulty.Extreme, amount = 1 }
//         };

//         /// <summary>
//         /// 生成された敵データ（ランタイムキャッシュ）
//         /// </summary>
//         [System.NonSerialized]
//         public List<EnemyData> generatedEnemies = new List<EnemyData>();

//         /// <summary>
//         /// 全難易度の合計出現数を取得
//         /// </summary>
//         public int GetTotalSpawnAmount()
//         {
//             int total = 0;
//             foreach (var amount in spawnAmounts)
//             {
//                 total += amount.amount;
//             }
//             return total;
//         }

//         /// <summary>
//         /// 特定難易度の出現数を取得
//         /// </summary>
//         public int GetAmountByDifficulty(Difficulty difficulty)
//         {
//             foreach (var amount in spawnAmounts)
//             {
//                 if (amount.difficulty == difficulty)
//                     return amount.amount;
//             }
//             return 0;
//         }
//     }

//     /// <summary>
//     /// 生成モード
//     /// </summary>
//     public enum GenerationMode
//     {
//         FullAuto,    // 完全自動（文化圏レベルのみ）
//         SemiAuto,    // 半自動（固定敵 + 残りは自動）
//         Manual       // 完全手動（全て指定）
//     }

//     /// <summary>
//     /// 難易度別出現数
//     /// </summary>
//     [System.Serializable]
//     public class DifficultySpawnAmount
//     {
//         public Difficulty difficulty;
//         public int amount;
//     }
// }