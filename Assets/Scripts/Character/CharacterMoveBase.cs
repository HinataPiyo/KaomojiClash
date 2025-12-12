using System.Collections;
using UnityEngine;

public abstract class CharacterMoveBase : MonoBehaviour
{
    Rigidbody2D rb;
    RectTransform rectTF;

    [Header("Launch")]
    [SerializeField] protected float launchPower = 10f;          // 最大発射パワー
    [SerializeField] protected float maxDragDistance = 3f;       // ドラッグ最大距離
    [SerializeField] protected float minLaunchDistance = 0.1f;   // これ未満なら発射しない

    [Header("Landing / Cooldown")]
    [SerializeField] protected float landingCooldown = 0.25f;    // 着地硬直時間(=CT)

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
    Coroutine cooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTF = GetComponentInChildren<RectTransform>();
    }

    void Update()
    {
        // var hor = Input.GetAxisRaw("Horizontal");
        // rb.linearVelocity = new Vector2(hor, 0);

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
    private IEnumerator LandingCooldownRoutine()
    {
        state = State.Cooldown;

        // 着地直後に速度を止める
        // rb.linearVelocity = Vector2.zero;

        // ここで「着地硬直中の顔文字」に切り替える
        // 例：faceController.SetLandingFace();

        yield return new WaitForSeconds(landingCooldown);

        // 硬直終了 → Idleへ
        state = State.Idle;

        // 顔文字をIdleに戻すなど
        // faceController.SetIdleFace();

        cooldown = null;
    }

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