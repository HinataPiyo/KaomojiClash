using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager I { get; private set; }
    [SerializeField] Volume globalVolume;

    Vignette vignette;

    void Awake()
    {
        if( I == null) I = this;

        globalVolume.profile = Instantiate(globalVolume.profile);

        // Vignette が無ければ追加して初期化
        if (!globalVolume.profile.TryGet(out vignette) || vignette == null)
        {
            vignette = globalVolume.profile.Add<Vignette>(true);
            vignette.intensity.value = 0f;
        }
    }

    /// <summary>
    /// 被弾エフェクトを設定する
    /// </summary>
    public void HitFlashEffect()
    {
        StartCoroutine(FlashEffectRoutine(Color.black));
    }

    /// <summary>
    /// 被弾エフェクトコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator FlashEffectRoutine(Color32 color)
    {
        vignette.color.value = color;
        vignette.intensity.value = 0.4f;

        float duration = 0.5f;
        float elapsed = 0f;
        float initialIntensity = vignette.intensity.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(initialIntensity, 0f, elapsed / duration);
            yield return null;
        }

        vignette.intensity.value = 0f;
    }
}