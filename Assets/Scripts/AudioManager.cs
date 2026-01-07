using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }
    [SerializeField] AudioDatas data;
    [SerializeField] AudioDatas bgmData;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] float bgmFadeDuration = 1f; // フェード時間（秒）
    
    private Coroutine bgmFadeCoroutine;


    void Awake()
    {
        if(I == null) { I = this; }
    }

    public void PlaySEReflect()
    {
        AudioClip clip = data.GetAudioClip("Reflect_0");
        AudioClip clip1 = data.GetAudioClip("Reflect_1");
        AudioClip clip2 = data.GetAudioClip("Reflect_2");
        // AudioClip clip3 = data.GetSEClip("Reflect_3");

        AudioClip[] clips = { clip, clip1, clip2, };
        int index = Random.Range(0, clips.Length);
        seSource.PlayOneShot(clips[index]);
    }

    public void PlaySE(string tag)
    {
        seSource.PlayOneShot(data.GetAudioClip(tag));
    }

    public void PlayBGM(string tag)
    {
        // 既存のフェード処理を停止
        if(bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        // 前の BGM をフェードアウト後、新しい BGM をフェードイン
        bgmFadeCoroutine = StartCoroutine(BGMFadeTransition(tag));
    }

    private IEnumerator BGMFadeTransition(string tag)
    {
        // 前の BGM がある場合はフェードアウト
        if(bgmSource.clip != null && bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut(bgmSource, bgmFadeDuration));
            bgmSource.Stop();
        }

        if(tag == string.Empty) yield break;

        // 新しい BGM をセット
        bgmSource.clip = bgmData.GetAudioClip(tag);
        if(bgmSource.clip != null)
        {
            bgmSource.Play();
            // フェードイン
            yield return StartCoroutine(FadeIn(bgmSource, bgmFadeDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        source.volume = 0f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        source.volume = 1f;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.volume = 0f;
    }


}