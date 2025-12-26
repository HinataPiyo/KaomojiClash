using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }
    [SerializeField] AudioDatas data;
    [SerializeField] AudioSource seSource;

    void Awake()
    {
        if(I == null) { I = this; }
    }

    public void PlaySEReflect()
    {
        AudioClip clip = data.GetSEClip("Reflect_0");
        AudioClip clip1 = data.GetSEClip("Reflect_1");
        AudioClip clip2 = data.GetSEClip("Reflect_2");
        // AudioClip clip3 = data.GetSEClip("Reflect_3");

        AudioClip[] clips = { clip, clip1, clip2, };
        int index = Random.Range(0, clips.Length);
        seSource.PlayOneShot(clips[index]);
    }

    public void PlaySE(string tag)
    {
        seSource.PlayOneShot(data.GetSEClip(tag));
    }
}