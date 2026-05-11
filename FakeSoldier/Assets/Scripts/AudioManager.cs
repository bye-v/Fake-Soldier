using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource voiceSource;
    [SerializeField] [Range(0f, 1f)] float bgmVolume   = 0.6f;
    [SerializeField] [Range(0f, 1f)] float sfxVolume   = 1.0f;
    [SerializeField] [Range(0f, 1f)] float voiceVolume = 1.0f;

    Coroutine voiceStopCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!bgmSource)   bgmSource   = gameObject.AddComponent<AudioSource>();
        if (!sfxSource)   sfxSource   = gameObject.AddComponent<AudioSource>();
        if (!voiceSource) voiceSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop   = true;
        bgmSource.volume = bgmVolume;
        sfxSource.loop   = false;
        sfxSource.volume = sfxVolume;
        voiceSource.loop   = false;
        voiceSource.volume = voiceVolume;
    }

    public void PlayBGM(AudioClip clip, float fadeTime = 0.5f)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.loop = true;
        StopAllCoroutines();
        StartCoroutine(CrossFadeBGM(clip, fadeTime));
    }

    // 루프 없이 한 번만 재생 (엔딩·크레딧용)
    public void PlayBGMOnce(AudioClip clip, float fadeTime = 0.5f)
    {
        if (clip == null) return;
        bgmSource.loop = false;
        PlayBGM(clip, fadeTime);
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

    // NPC 목소리: 최대 maxDuration초 재생 후 자동 페이드 아웃
    // volumeScale > 1 을 사용해 실제 출력 볼륨을 높임
    public void PlayVoice(AudioClip clip, float maxDuration = 2f)
    {
        if (clip == null) return;
        if (voiceStopCoroutine != null) StopCoroutine(voiceStopCoroutine);
        voiceSource.volume = voiceVolume;
        voiceSource.PlayOneShot(clip, 3f); // 3× scale로 음량 증폭
        voiceStopCoroutine = StartCoroutine(FadeOutVoiceAfter(maxDuration));
    }

    public void StopVoice()
    {
        if (voiceStopCoroutine != null) { StopCoroutine(voiceStopCoroutine); voiceStopCoroutine = null; }
        StartCoroutine(FadeVoiceTo(0f, 0.08f, true));
    }

    IEnumerator FadeOutVoiceAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return FadeVoiceTo(0f, 0.12f, true);
    }

    IEnumerator FadeVoiceTo(float targetVol, float duration, bool restoreAfter)
    {
        float start = voiceSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            voiceSource.volume = Mathf.Lerp(start, targetVol, t / duration);
            yield return null;
        }
        voiceSource.volume = targetVol;
        if (restoreAfter)
        {
            yield return null;
            voiceSource.volume = voiceVolume;
        }
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
