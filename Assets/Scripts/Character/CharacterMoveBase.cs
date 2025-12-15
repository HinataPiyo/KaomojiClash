using System.Collections;
using UnityEngine;

public abstract class CharacterMoveBase : MonoBehaviour
{
    Rigidbody2D rb;
    RectTransform rectTF;

    [Header("Debug / Optional")]
    [SerializeField] protected LineRenderer aimLine;             // 照準線（任意）

    protected enum State
    {
        Idle,       // 待機中（ドラッグ開始可能）
        Dragging,   // ドラッグ中（方向決め）
        Launched,   // 発射中（移動中）
        Cooldown    // 着地硬直中（CT）
    }

    [SerializeField] protected State state = State.Idle;
    protected Vector2 dragStartWorld;
    protected Coroutine cooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTF = GetComponentInChildren<RectTransform>();
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                HandleIdleInput();
                break;
            case State.Dragging:
                HandleDraggingInput();
                break;
            case State.Launched:
                if(cooldown != null) break;
                cooldown = StartCoroutine(LandingCooldownRoutine());
                break;
            // Launched / Cooldown 中は入力を無視
        }

        Flip();
    }

    /// <summary>
    /// 向き反転処理
    /// </summary>
    void Flip()
    {
        if(rb.linearVelocity.x < 0)
        {
            rectTF.localScale = new Vector3(1, 1, 1);
        }
        else if (rb.linearVelocity.x > 0)
        {
            rectTF.localScale = new Vector3(-1, 1, 1);
        }
    }

    /// <summary>
    /// Idle中の入力処理
    /// </summary>
    protected abstract void HandleIdleInput();
    
    /// <summary>
    /// ドラッグ中の入力処理
    /// </summary>
    protected abstract void HandleDraggingInput();

    // --- 発射処理 ---

    /// <summary>
    /// 指定速度で発射
    /// </summary>
    /// <param name="velocity"></param>
    protected void Launch(Vector2 velocity)
    {
        state = State.Launched;
        rb.linearVelocity = velocity;
    }

    /// <summary>
    /// 着地硬直ルーチン
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator LandingCooldownRoutine();

    // --- ユーティリティ ---

    /// <summary>
    /// マウスのワールド座標を取得
    /// </summary>
    /// <returns></returns>
    protected Vector2 GetMouseWorldPos()
    {
        var cam = Camera.main;
        if (cam == null) return Vector2.zero;

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = -cam.transform.position.z; // プレイヤーZとの差

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        return new Vector2(worldPos.x, worldPos.y);
    }
}