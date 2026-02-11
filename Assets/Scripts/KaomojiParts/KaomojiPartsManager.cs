using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ENUM;

/// <summary>
/// KaomojiPartDataを動的に検索・管理するマネージャー
/// ScriptableObjectのDatabaseを使わず、AssetDatabaseから直接取得
/// </summary>
public class KaomojiPartsManager : MonoBehaviour
{
    public static KaomojiPartsManager Instance { get; private set; }
    
    private Dictionary<KaomojiPartType, List<KaomojiPartData>> cachedPartsByType;
    private List<KaomojiPartData> allCachedParts;
    private bool isInitialized = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCache();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 全てのKaomojiPartDataをResourcesフォルダから読み込み、キャッシュする
    /// </summary>
    private void InitializeCache()
    {
        allCachedParts = new List<KaomojiPartData>();
        cachedPartsByType = new Dictionary<KaomojiPartType, List<KaomojiPartData>>();

        // Resourcesフォルダから全てのKaomojiPartDataを読み込む
        KaomojiPartData[] allParts = Resources.LoadAll<KaomojiPartData>("KaomojiParts");
        
        foreach (var part in allParts)
        {
            if (part == null) continue;
            
            allCachedParts.Add(part);
            
            // 部位別に分類
            if (!cachedPartsByType.ContainsKey(part.PartType))
            {
                cachedPartsByType[part.PartType] = new List<KaomojiPartData>();
            }
            cachedPartsByType[part.PartType].Add(part);
        }

        isInitialized = true;
        Debug.Log($"KaomojiPartsManager: {allCachedParts.Count} parts loaded.");
    }

    /// <summary>
    /// 全てのパーツを取得
    /// </summary>
    public KaomojiPartData[] GetAllParts()
    {
        if (!isInitialized) InitializeCache();
        return allCachedParts.ToArray();
    }

    /// <summary>
    /// 特定の部位のパーツを取得
    /// </summary>
    public KaomojiPartData[] GetPartsByType(KaomojiPartType type)
    {
        if (!isInitialized) InitializeCache();
        
        if (cachedPartsByType.ContainsKey(type))
        {
            return cachedPartsByType[type].ToArray();
        }
        
        return new KaomojiPartData[0];
    }

    /// <summary>
    /// パーツ名で検索
    /// </summary>
    public KaomojiPartData GetPartByName(string partName)
    {
        if (!isInitialized) InitializeCache();
        return allCachedParts.FirstOrDefault(p => p.Data.partName == partName);
    }

    /// <summary>
    /// ランダムにパーツを取得
    /// </summary>
    public KaomojiPartData GetRandomPart()
    {
        if (!isInitialized) InitializeCache();
        if (allCachedParts.Count == 0) return null;
        return allCachedParts[Random.Range(0, allCachedParts.Count)];
    }

    /// <summary>
    /// 特定の部位からランダムにパーツを取得
    /// </summary>
    public KaomojiPartData GetRandomPartByType(KaomojiPartType type)
    {
        var parts = GetPartsByType(type);
        if (parts.Length == 0) return null;
        return parts[Random.Range(0, parts.Length)];
    }

    /// <summary>
    /// キャッシュをリロード（エディタ用）
    /// </summary>
    public void ReloadCache()
    {
        allCachedParts?.Clear();
        cachedPartsByType?.Clear();
        isInitialized = false;
        InitializeCache();
    }
}