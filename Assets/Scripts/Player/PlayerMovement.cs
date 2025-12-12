using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    RectTransform rectTF;

    [Header("Launch")]
    [SerializeField] float launchPower = 10f;          // 最大発射パワー
    [SerializeField] float maxDragDistance = 3f;       // ドラッグ最大距離
    [SerializeField] float minLaunchDistance = 0.1f;   // これ未満なら発射しない

    [Header("Landing / Cooldown")]
    [SerializeField] float landingCooldown = 0.25f;    // 着地硬直時間(=CT)

    [Header("Debug / Optional")]
    [SerializeField] LineRenderer aimLine;             // 照準線（任意）

    private enum State
    {
        Idle,       // 待機中（ドラッグ開始可能）
        Dragging,   // ドラッグ中（方向決め）
        Launched,   // 発射中（移動中）
        Cooldown    // 着地硬直中（CT）
    }

    [SerializeField] State state = State.Idle;
    Vector2 dragStartWorld;
    Coroutine cooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTF = GetComponentInChildren<RectTransform>();
    }

    void Start()
    {
        
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

    void HandleIdleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartWorld = GetMouseWorldPos();
            state = State.Dragging;

            if (aimLine != null)
            {
                aimLine.positionCount = 2;
            }
        }
    }

    void HandleDraggingInput()
    {
        Vector2 currentWorld = GetMouseWorldPos();
        Vector2 dragVector = currentWorld - dragStartWorld;

        // 最大ドラッグ距離を制限
        if (dragVector.magnitude > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }

        // LineRendererで方向表示（スリングショットっぽく逆向きに伸ばす）
        if (aimLine != null)
        {
            Vector3 start = dragStartWorld;
            Vector3 end = dragStartWorld + dragVector;
            aimLine.SetPosition(0, start);
            aimLine.SetPosition(1, end);
        }

        // ボタンを離したら発射
        if (Input.GetMouseButtonUp(0))
        {
            if (aimLine != null)
            {
                aimLine.positionCount = 0;
            }

            if (dragVector.magnitude < minLaunchDistance)
            {
                // ほぼ動いてない → 発射キャンセル
                state = State.Idle;
                return;
            }

            // スリングショットなのでドラッグ方向の逆に飛ばす
            Vector2 launchDir = -dragVector.normalized;

            // 距離に応じて速度をスケール
            float powerRate = Mathf.Clamp01(dragVector.magnitude / maxDragDistance);
            float launchSpeed = launchPower * powerRate;

            Launch(launchDir * launchSpeed);
        }
    }

    // --- 発射処理 ---

    private void Launch(Vector2 velocity)
    {
        state = State.Launched;
        rb.linearVelocity = velocity;
    }

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

    private Vector2 GetMouseWorldPos()
    {
        var cam = Camera.main;
        if (cam == null) return Vector2.zero;

        Vector3 screenPos = Input.mousePosition;
        screenPos.z = -cam.transform.position.z; // プレイヤーZとの差

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        return new Vector2(worldPos.x, worldPos.y);
    }
}
