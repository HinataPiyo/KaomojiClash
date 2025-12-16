using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop I { get; private set; }

    void Awake()
    {
        if(I == null) I = this;
    }

    public void StartHitStop(float duration = 0.1f)
    {
        StartCoroutine(HitStopRoutine(duration));
    }

    IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}