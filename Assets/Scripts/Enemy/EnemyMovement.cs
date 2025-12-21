using System.Collections;
using UnityEngine;

public class EnemyMovement : Movement
{
    [SerializeField] EnemyData data;
    

    bool isInput;      // 発射可能フラグ
    bool isLaunch;     // 発射中フラグ

    void Start()
    {
        StartCoroutine(LaunchRoutine());
    }


    IEnumerator LaunchRoutine()
    {
        isInput = true;

        // 発射までの待機時間
        yield return new WaitForSeconds(data.launchDuration);
        isLaunch = true;
    }
    /// <summary>
    /// 待機中の入力処理
    /// </summary>
    protected override void HandleIdleInput()
    {
        if(!isInput) return;
        dragStartWorld = transform.position;    // ドラッグ開始位置を記録
        shootDirectionArrow = WorldCanvasManager.I.CreateShootDirectionArrow(transform.position);   // 照準矢印作成
        state = State.Dragging;     // ドラッグ中状態へ
        isInput = false;
    }

    /// <summary>
    /// ドラッグ中の入力処理
    /// </summary>
    protected override void HandleDraggingInput()
    {
        if(Context.I.Player == null) return;
        // プレイヤーへの方角
        Vector2 directionToPlayer = Context.I.Player.transform.position - transform.position;

        // スリングショットなので、ドラッグベクトルはプレイヤーと逆方向にする
        Vector2 dragVector = -directionToPlayer.normalized * data.Status.maxDragDistance;

        WorldCanvasManager.I.ShowShootDirectionArrow(shootDirectionArrow, transform.position, dragVector, dragVector.magnitude);

        // ボタンを離したら発射
        if (isLaunch)
        {
            if (dragVector.magnitude < data.Status.minLaunchDistance)
            {
                // ほぼ動いてない → 発射キャンセル
                state = State.Idle;
                return;
            }

            // スリングショットなのでドラッグ方向の逆に飛ばす
            Vector2 launchDir = -dragVector.normalized;

            // 距離に応じて速度をスケール
            float powerRate = Mathf.Clamp01(dragVector.magnitude / data.Status.maxDragDistance);
            float launchSpeed = data.Status.launchPower * powerRate;

            CameraZoom.I.ResetZoom();
            shootDirectionArrow.Del();
            Launch(launchDir * launchSpeed);

            isLaunch = false;
        }
    }

    protected override IEnumerator LandingCooldownRoutine()
    {
        state = State.Cooldown;

        yield return new WaitForSeconds(data.Status.landingCooldown);

        // 硬直終了 → Idleへ
        state = State.Idle;

        cooldown = null;
        StartCoroutine(LaunchRoutine());
    }
}