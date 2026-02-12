using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Constants;
using ENUM;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// EnemyDataを動的に生成するヘルパークラス
/// </summary>
public static class EnemyDataGenerator
{
    /// <summary>
    /// 文化圏レベルと難易度に基づいてランダムな敵を生成
    /// </summary>
    public static EnemyData GenerateRandomEnemy(int cultureLevel, Difficulty difficulty, List<KaomojiPartType> excludeTypes = null)
    {
        // 利用可能なパーツを取得
        var allParts = GetAvailablePartsForEnemy(cultureLevel, difficulty, excludeTypes);
        
        if (allParts.Count == 0)
        {
            Debug.LogWarning($"利用可能なパーツがありません (Culture Lv{cultureLevel}, {difficulty})");
            return null;
        }
        
        // EnemyDataを作成
        EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
        
        if (enemy == null)
        {
            Debug.LogError("CreateInstance<EnemyData>() failed!");
            return null;
        }
        
        // ランダムにパーツを選択して敵を構築
        bool success = BuildEnemyKaomoji(enemy, allParts, cultureLevel, difficulty);
        
        if (!success)
        {
            Debug.LogWarning("敵の構築に失敗しました");
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ScriptableObject.DestroyImmediate(enemy);
            }
            else
            #endif
            {
                ScriptableObject.Destroy(enemy);
            }
            return null;
        }
        
        return enemy;
    }

    /// <summary>
    /// 文化圏レベルと難易度に応じた利用可能なパーツを取得
    /// </summary>
    private static List<KaomojiPartData> GetAvailablePartsForEnemy(int cultureLevel, Difficulty difficulty, List<KaomojiPartType> excludeTypes)
    {
        var allParts = new List<KaomojiPartData>();
        
        #if UNITY_EDITOR
        // エディタモードの場合
        if (!Application.isPlaying)
        {
            try
            {
                allParts = KaomojiPartsManagerEditor.GetAllPartsInEditor().ToList();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"パーツ取得エラー: {e.Message}");
                return allParts;
            }
        }
        else
        #endif
        {
            // ランタイムの場合
            if (KaomojiPartsManager.Instance != null)
            {
                allParts = KaomojiPartsManager.Instance.GetAllParts().ToList();
            }
        }
        
        if (allParts.Count == 0)
        {
            Debug.LogWarning("パーツが見つかりません。Resources/KaomojiParts フォルダにパーツを配置してください。");
            return allParts;
        }
        
        // 除外タイプをフィルタ
        if (excludeTypes != null && excludeTypes.Count > 0)
        {
            allParts = allParts.Where(p => !excludeTypes.Contains(p.PartType)).ToList();
        }
        
        Debug.Log($"利用可能なパーツ数: {allParts.Count}");
        
        return allParts;
    }

    /// <summary>
    /// 敵の顔文字を構築
    /// </summary>
    private static bool BuildEnemyKaomoji(EnemyData enemy, List<KaomojiPartData> availableParts, int cultureLevel, Difficulty difficulty)
    {
        if (enemy == null)
        {
            Debug.LogError("EnemyData is null!");
            return false;
        }

        // 各部位からランダムに選択
        var eyesParts = availableParts.Where(p => p != null && p.PartType == KaomojiPartType.Eyes).ToList();
        var mouthParts = availableParts.Where(p => p != null && p.PartType == KaomojiPartType.Mouth).ToList();
        var handsParts = availableParts.Where(p => p != null && p.PartType == KaomojiPartType.Hands).ToList();
        var deco1Parts = availableParts.Where(p => p != null && p.PartType == KaomojiPartType.Decoration_First).ToList();
        var deco2Parts = availableParts.Where(p => p != null && p.PartType == KaomojiPartType.Decoration_Second).ToList();
        
        Debug.Log($"パーツ内訳 - Eyes:{eyesParts.Count}, Mouth:{mouthParts.Count}, Hands:{handsParts.Count}, Deco1:{deco1Parts.Count}, Deco2:{deco2Parts.Count}");
        
        #if UNITY_EDITOR
        // エディタモードではSerializedObjectを使用
        SerializedObject serializedEnemy = null;
        
        try
        {
            serializedEnemy = new SerializedObject(enemy);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SerializedObject creation failed: {e.Message}");
            return false;
        }
        
        if (serializedEnemy == null)
        {
            Debug.LogError("SerializedObject is null!");
            return false;
        }
        
        // 小文字の"kaomoji"フィールドを検索
        SerializedProperty kaomojiProp = serializedEnemy.FindProperty("kaomoji");
        
        if (kaomojiProp == null)
        {
            Debug.LogError("'kaomoji' property not found in EnemyData!");
            Debug.LogError("EnemyDataの全プロパティを確認:");
            
            // デバッグ用: 全プロパティを列挙
            SerializedProperty iterator = serializedEnemy.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    Debug.Log($"  - {iterator.name} ({iterator.propertyType})");
                } while (iterator.NextVisible(false));
            }
            
            return false;
        }
        
        bool anyPartAssigned = false;
        
        // Eyes（必須）
        if (eyesParts.Count > 0)
        {
            var selectedEyes = eyesParts[Random.Range(0, eyesParts.Count)];
            SerializedProperty eyesProp = kaomojiProp.FindPropertyRelative("eyes");
            if (eyesProp != null)
            {
                eyesProp.objectReferenceValue = selectedEyes;
                anyPartAssigned = true;
                Debug.Log($"Eyes assigned: {selectedEyes.Data.partName}");
            }
            else
            {
                Debug.LogWarning("'eyes' property not found in kaomoji");
            }
        }
        else
        {
            Debug.LogWarning("Eyes パーツがありません！");
        }
        
        // Mouth（必須）
        if (mouthParts.Count > 0)
        {
            var selectedMouth = mouthParts[Random.Range(0, mouthParts.Count)];
            SerializedProperty mouthProp = kaomojiProp.FindPropertyRelative("mouth");
            if (mouthProp != null)
            {
                mouthProp.objectReferenceValue = selectedMouth;
                anyPartAssigned = true;
                Debug.Log($"Mouth assigned: {selectedMouth.Data.partName}");
            }
            else
            {
                Debug.LogWarning("'mouth' property not found in kaomoji");
            }
        }
        else
        {
            Debug.LogWarning("Mouth パー��がありません！");
        }
        
        // Hands（オプション：50%）
        if (handsParts.Count > 0 && Random.value > 0.5f)
        {
            var selectedHands = handsParts[Random.Range(0, handsParts.Count)];
            SerializedProperty handsProp = kaomojiProp.FindPropertyRelative("hands");
            if (handsProp != null)
            {
                handsProp.objectReferenceValue = selectedHands;
                Debug.Log($"Hands assigned: {selectedHands.Data.partName}");
            }
        }
        
        // Decoration First（オプション：30%）
        if (deco1Parts.Count > 0 && Random.value > 0.7f)
        {
            var selectedDeco1 = deco1Parts[Random.Range(0, deco1Parts.Count)];
            SerializedProperty deco1Prop = kaomojiProp.FindPropertyRelative("decorationFirst");
            if (deco1Prop != null)
            {
                deco1Prop.objectReferenceValue = selectedDeco1;
                Debug.Log($"Decoration First assigned: {selectedDeco1.Data.partName}");
            }
        }
        
        // Decoration Second（オプション：30%）
        if (deco2Parts.Count > 0 && Random.value > 0.7f)
        {
            var selectedDeco2 = deco2Parts[Random.Range(0, deco2Parts.Count)];
            SerializedProperty deco2Prop = kaomojiProp.FindPropertyRelative("decorationSecond");
            if (deco2Prop != null)
            {
                deco2Prop.objectReferenceValue = selectedDeco2;
                Debug.Log($"Decoration Second assigned: {selectedDeco2.Data.partName}");
            }
        }
        
        try
        {
            serializedEnemy.ApplyModifiedProperties();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ApplyModifiedProperties failed: {e.Message}");
            return false;
        }
        
        return anyPartAssigned;
        
        #else
        // ランタイムでは直接設定（Setterメソッドが必要）
        bool anyPartAssigned = false;
        
        if (eyesParts.Count > 0)
        {
            enemy.Kaomoji.SetEyes(eyesParts[Random.Range(0, eyesParts.Count)]);
            anyPartAssigned = true;
        }
        
        if (mouthParts.Count > 0)
        {
            enemy.Kaomoji.SetMouth(mouthParts[Random.Range(0, mouthParts.Count)]);
            anyPartAssigned = true;
        }
        
        if (handsParts.Count > 0 && Random.value > 0.5f)
        {
            enemy.Kaomoji.SetHands(handsParts[Random.Range(0, handsParts.Count)]);
        }
        
        if (deco1Parts.Count > 0 && Random.value > 0.7f)
        {
            enemy.Kaomoji.SetDecorationFirst(deco1Parts[Random.Range(0, deco1Parts.Count)]);
        }
        
        if (deco2Parts.Count > 0 && Random.value > 0.7f)
        {
            enemy.Kaomoji.SetDecorationSecond(deco2Parts[Random.Range(0, deco2Parts.Count)]);
        }
        
        return anyPartAssigned;
        #endif
    }

    /// <summary>
    /// 固定敵と自動生成敵を組み合わせて敵���ストを作成
    /// </summary>
    public static List<EnemyData> GenerateEnemyList(EnemySpawnConfig config, int cultureLevel, Difficulty difficulty, int count)
    {
        var enemies = new List<EnemyData>();
        
        // 固定敵を追加
        int fixedCount = 0;
        if (config.fixedEnemies != null && config.fixedEnemies.Count > 0)
        {
            foreach (var fixedEnemy in config.fixedEnemies)
            {
                if (fixedEnemy != null && fixedCount < count)
                {
                    enemies.Add(fixedEnemy);
                    fixedCount++;
                }
            }
        }
        
        // 残りを自動生成
        int remainingCount = count - fixedCount;
        for (int i = 0; i < remainingCount; i++)
        {
            var enemy = GenerateRandomEnemy(cultureLevel, difficulty, config.excludeTypes);
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
            else
            {
                Debug.LogWarning($"敵の生成に失敗しました ({i + 1}/{remainingCount})");
            }
        }
        
        Debug.Log($"Generated {enemies.Count} enemies for {difficulty} (requested: {count})");
        
        return enemies;
    }
}