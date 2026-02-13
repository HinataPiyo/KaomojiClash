using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TmpAfterImageTrail : MonoBehaviour
{
    [Header("Target TMP (player visual)")]
    [SerializeField] TextMeshPro targetTmp;

    [Header("Ghost TMP Prefab (TMP_Text only, no collider)")]
    [SerializeField] TextMeshPro ghostPrefab;

    [Header("Tuning")]
    [SerializeField] float spawnInterval = 0.03f;
    [SerializeField] float lifeTime = 0.25f;
    [SerializeField, Range(0f, 1f)] float startAlpha = 0.35f;

    [Header("Pool")]
    [SerializeField] int poolSize = 24;

    [Header("Spawn Parent")]
    [SerializeField] Transform ghostParent;

    [Header("Render")]
    [SerializeField] int sortingOrderOffset = 1;

    bool _active;
    float _timer;

    readonly Queue<TextMeshPro> _pool = new();

    void Awake()
    {
        if (targetTmp == null)
            targetTmp = GetComponentInChildren<TextMeshPro>(true);

        if (ghostParent == null)
        {
            var container = new GameObject($"{name}_AfterImagePool");
            ghostParent = container.transform;
            ghostParent.SetParent(null);
        }

        for (int i = 0; i < poolSize; i++)
        {
            var g = Instantiate(ghostPrefab, ghostParent);
            g.gameObject.SetActive(false);
            _pool.Enqueue(g);
        }
    }

    public void SetActive(bool active)
    {
        _active = active;
        _timer = 0f;
    }

    void Update()
    {
        if (!_active) return;
        if (targetTmp == null) return;
        if (_pool.Count == 0) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnGhost();
        }
    }

    void SpawnGhost()
    {
        var g = _pool.Dequeue();
        g.gameObject.SetActive(true);

        CopyVisual(targetTmp, g);

        // アルファ開始
        var c = g.color;
        c.a = startAlpha;
        g.color = c;

        StartCoroutine(FadeAndReturn(g, lifeTime));
    }

    void CopyVisual(TMP_Text src, TMP_Text dst)
    {
        // テキスト内容
        dst.text = src.text;

        // 重要：見た目差が出やすい項目をコピー
        dst.font = src.font;
        dst.fontSize = src.fontSize;
        dst.fontStyle = src.fontStyle;
        dst.alignment = src.alignment;
        dst.textWrappingMode = src.textWrappingMode;
        dst.richText = src.richText;

        // アウトライン/影などの見た目（マテリアル共有に注意）
        dst.fontSharedMaterial = src.fontSharedMaterial;

        // Transform（見た目を一致させる）
        var st = src.transform;
        var dt = dst.transform;

        dt.position = st.position;
        dt.rotation = st.rotation;

        // 残像は移動しない親配下に置くため、ワールドスケールを維持
        dt.localScale = st.lossyScale;

        // ソート（WorldSpace TMPの時に重要）
        if (src is TextMeshPro tmp3dSrc && dst is TextMeshPro tmp3dDst)
        {
            tmp3dDst.sortingLayerID = tmp3dSrc.sortingLayerID;
            tmp3dDst.sortingOrder = tmp3dSrc.sortingOrder + sortingOrderOffset;
        }
    }

    IEnumerator FadeAndReturn(TextMeshPro g, float t)
    {
        float elapsed = 0f;
        Color c = g.color;

        while (elapsed < t)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, 0f, elapsed / t);
            c.a = a;
            g.color = c;
            yield return null;
        }

        g.gameObject.SetActive(false);
        _pool.Enqueue(g);
    }
}
