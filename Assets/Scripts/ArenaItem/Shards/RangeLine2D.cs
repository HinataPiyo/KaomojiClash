using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeLine2D : MonoBehaviour
{
    [SerializeField] float radius = 3f;
    [SerializeField] float lineWidth = 0.1f;
    [SerializeField] int segments = 64;
    [SerializeField] Color color = Color.cyan;

    LineRenderer lr;

    float prevRadius;
    float prevWidth;
    int prevSegments;
    Color prevColor;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.loop = true;
        lr.useWorldSpace = false;
        lr.alignment = LineAlignment.TransformZ;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="r"></param>
    public void Initialize(float r)
    {
        ApplyColor();     // 初期色
        SetRadius(r);
        UpdateCircle();   // 初期形状
    }

    void LateUpdate()
    {
        if (radius != prevRadius || lineWidth != prevWidth || segments != prevSegments)
            UpdateCircle();

        if (color != prevColor)
            ApplyColor();
    }

    //============================
    // 円形状更新
    //============================
    void UpdateCircle()
    {
        prevRadius = radius;
        prevWidth = lineWidth;
        prevSegments = segments;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.positionCount = segments;

        float step = 2f * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float a = step * i;
            Vector3 pos = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * radius;
            lr.SetPosition(i, pos);
        }
    }

    //============================
    // 色更新
    //============================
    void ApplyColor()
    {
        prevColor = color;

        lr.startColor = color;
        lr.endColor = color;
    }

    //============================
    // 外部API
    //============================
    public void SetRadius(float r)
    {
        radius = Mathf.Max(0f, r);
    }

    public void SetWidth(float w)
    {
        lineWidth = Mathf.Max(0f, w);
    }

    public void SetColor(Color c)
    {
        color = c;
    }

    public void ResetColor()
    {
        color = Color.cyan;
    }
}
