using UnityEngine;

public class UIMoveEaseOut : MonoBehaviour
{
    [SerializeField] private RectTransform target;

    private Vector2 startPos;
    private Vector2 endPos;
    private float duration;
    private float time;
    private bool moving;

    // =========================
    // 外部API
    // =========================
    public void MoveLeft(float distance, float durationSec)
    {
        StartMove(Vector2.left * distance, durationSec);
    }

    public void Move(Vector2 offset, float durationSec)
    {
        StartMove(offset, durationSec);
    }

    public void Stop()
    {
        moving = false;
    }

    public void Reset()
    {
        target.anchoredPosition = startPos;
        moving = false;
    }

    // =========================
    // 内部処理
    // =========================
    private void StartMove(Vector2 offset, float durationSec)
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        startPos = target.anchoredPosition;
        endPos = startPos + offset;
        duration = Mathf.Max(0.01f, durationSec);
        time = 0f;
        moving = true;
    }

    private void Update()
    {
        if (!moving) return;

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration);

        float eased = EaseOutCubic(t);

        target.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);

        if (t >= 1f)
            moving = false;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
