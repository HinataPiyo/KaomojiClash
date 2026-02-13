namespace Map
{

    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class MapCreator : MonoBehaviour
    {
        [Header("生成するASCIIオブジェクトのテンプレート")]
        [SerializeField] GameObject asciiObjectPrefab;

        [Header("配置候補のASCIIアート")]
        [SerializeField] string[] asciiArts;

        [Header("ASCIIランダム分割設定")]
        [SerializeField] bool useRandomSplit = true;
        [SerializeField, Min(1)] int minSplitLength = 2;

        [Header("生成設定")]
        [SerializeField, Min(1)] int createCount = 10;
        [SerializeField] Vector2 spawnArea = new Vector2(16f, 10f);
        [SerializeField, Min(0.1f)] float objectRadius = 0.5f;
        [SerializeField, Min(1)] int maxTryPerObject = 30;
        [SerializeField] LayerMask blockLayerMask = ~0;
        [SerializeField] bool createOnStart = true;

        readonly List<GameObject> spawnedObjects = new List<GameObject>();
        readonly List<Vector2> usedPositions = new List<Vector2>();

        void Start()
        {
            if (createOnStart)
            {
                CreateMapObjects();
            }
        }

        [ContextMenu("Create Map Objects")]
        public void CreateMapObjects()
        {
            if (asciiObjectPrefab == null)
            {
                Debug.LogWarning("MapCreator: asciiObjectPrefab が未設定です。", this);
                return;
            }

            if (asciiArts == null || asciiArts.Length == 0)
            {
                Debug.LogWarning("MapCreator: asciiArts が未設定です。", this);
                return;
            }

            ClearMapObjects();

            for (int i = 0; i < createCount; i++)
            {
                if (!TryFindSpawnPosition(out Vector2 spawnPos))
                {
                    Debug.LogWarning($"MapCreator: 配置可能な座標が不足したため {i}/{createCount} 個で停止しました。", this);
                    break;
                }

                GameObject obj = Instantiate(asciiObjectPrefab, spawnPos, Quaternion.identity, transform);
                ApplyAsciiText(obj, CreateRandomAsciiFromArray());

                spawnedObjects.Add(obj);
                usedPositions.Add(spawnPos);
            }
        }

        [ContextMenu("Clear Map Objects")]
        public void ClearMapObjects()
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    Destroy(spawnedObjects[i]);
                }
            }

            spawnedObjects.Clear();
            usedPositions.Clear();
        }

        bool TryFindSpawnPosition(out Vector2 spawnPos)
        {
            for (int tryCount = 0; tryCount < maxTryPerObject; tryCount++)
            {
                Vector2 candidate = GetRandomPositionInArea();
                if (IsOverlapping(candidate))
                {
                    continue;
                }

                spawnPos = candidate;
                return true;
            }

            spawnPos = default;
            return false;
        }

        Vector2 GetRandomPositionInArea()
        {
            float x = Random.Range(-spawnArea.x * 0.5f, spawnArea.x * 0.5f);
            float y = Random.Range(-spawnArea.y * 0.5f, spawnArea.y * 0.5f);
            return (Vector2)transform.position + new Vector2(x, y);
        }

        bool IsOverlapping(Vector2 candidate)
        {
            float minDistance = objectRadius * 2f;
            float minDistanceSqr = minDistance * minDistance;

            for (int i = 0; i < usedPositions.Count; i++)
            {
                if ((usedPositions[i] - candidate).sqrMagnitude < minDistanceSqr)
                {
                    return true;
                }
            }

            Collider2D hit = Physics2D.OverlapCircle(candidate, objectRadius, blockLayerMask);
            return hit != null;
        }

        void ApplyAsciiText(GameObject target, string ascii)
        {
            TMP_Text tmp = target.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = ascii;
                return;
            }

            TextMesh textMesh = target.GetComponentInChildren<TextMesh>();
            if (textMesh != null)
            {
                textMesh.text = ascii;
            }
        }

        string CreateRandomAsciiFromArray()
        {
            string baseAscii = asciiArts[Random.Range(0, asciiArts.Length)];
            if (string.IsNullOrEmpty(baseAscii))
            {
                return "#";
            }

            if (!useRandomSplit || baseAscii.Length == 1)
            {
                return baseAscii;
            }

            int safeMin = Mathf.Clamp(minSplitLength, 1, baseAscii.Length);
            int length = Random.Range(safeMin, baseAscii.Length + 1);
            int startIndex = Random.Range(0, baseAscii.Length - length + 1);
            return baseAscii.Substring(startIndex, length);
        }

        /// <summary>
        /// 戦闘中になった敵の処理
        /// </summary>
        public void OnBattle()
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    spawnedObjects[i].GetComponentInChildren<TMP_Text>().color = new Color(0f, 0f, 0f, 0.3f);
                }
            }
        }

        /// <summary>
        /// 戦闘外の敵の処理
        /// </summary>
        public void OutBattle()
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    spawnedObjects[i].GetComponentInChildren<TMP_Text>().color = new Color(0f, 0f, 0f, 0.3f);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, spawnArea.y, 0f));
        }

    }

}