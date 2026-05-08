using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] [Range(0f, 1f)] float bgmVolume = 0.6f;
    [SerializeField] [Range(0f, 1f)] float sfxVolume = 1.0f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!bgmSource) bgmSource = gameObject.AddComponent<AudioSource>();
        if (!sfxSource) sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }

    public void PlayBGM(AudioClip clip, float fadeTime = 0.5f)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        StopAllCoroutines();
        StartCoroutine(CrossFadeBGM(clip, fadeTime));
    }

    public void StopBGM(float fadeTime = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetBGMVolume(float v)
    {
        bgmVolume = Mathf.Clamp01(v);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        sfxSource.volume = sfxVolume;
    }

    IEnumerator CrossFadeBGM(AudioClip newClip, float fadeTime)
    {
        if (bgmSource.isPlaying && fadeTime > 0f)
        {
            float t = 0f;
            float startVol = bgmSource.volume;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
                yield return null;
            }
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        if (fadeTime > 0f)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / fadeTime);
                yield return null;
            }
        }
        bgmSource.volume = bgmVolume;
    }

    IEnumerator FadeOutBGM(float fadeTime)
    {
        if (!bgmSource.isPlaying) yield break;
        float t = 0f;
        float startVol = bgmSource.volume;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = bgmVolume;
    }
}
