using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField] Image    fadeImage;
    [SerializeField] TMP_Text titleLabel;
    [SerializeField] float    fadeDuration      = 0.5f;
    [SerializeField] float    titleFadeDuration = 0.4f;

    void Awake()
    {
        if (Instance != null) { Destroy(transform.root.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
        if (fadeImage) fadeImage.color = Color.black;
        if (titleLabel) titleLabel.gameObject.SetActive(false);
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName, Action onMidpoint = null)
        => StartCoroutine(DoFadeToScene(sceneName, onMidpoint));

    IEnumerator DoFadeToScene(string sceneName, Action onMidpoint)
    {
        yield return FadeOut();
        onMidpoint?.Invoke();
        if (sceneName != null) GameManager.Instance.LoadScene(sceneName);
        yield return FadeIn();
    }

    // 장면 전환 없이 화면 위에 날짜/장소 타이틀을 잠시 표시
    // fadeImage를 반투명 배경으로 활용해 가독성 확보
    public IEnumerator ShowTitle(string text, float holdTime = 1.8f)
    {
        if (!titleLabel || string.IsNullOrEmpty(text)) yield break;

        const float bgAlpha = 0.72f;

        titleLabel.text = text;
        titleLabel.color = Color.clear;
        titleLabel.gameObject.SetActive(true);

        float t = 0;
        while (t < titleFadeDuration)
        {
            t += Time.deltaTime;
            float pct = t / titleFadeDuration;
            titleLabel.color = new Color(1f, 1f, 1f, pct);
            if (fadeImage) { var c = fadeImage.color; c.a = pct * bgAlpha; fadeImage.color = c; }
            yield return null;
        }
        titleLabel.color = Color.white;
        if (fadeImage) { var c = fadeImage.color; c.a = bgAlpha; fadeImage.color = c; }

        yield return new WaitForSeconds(holdTime);

        t = 0;
        while (t < titleFadeDuration)
        {
            t += Time.deltaTime;
            float pct = t / titleFadeDuration;
            titleLabel.color = new Color(1f, 1f, 1f, 1f - pct);
            if (fadeImage) { var c = fadeImage.color; c.a = (1f - pct) * bgAlpha; fadeImage.color = c; }
            yield return null;
        }
        titleLabel.gameObject.SetActive(false);
        if (fadeImage) { var c = fadeImage.color; c.a = 0f; fadeImage.color = c; }
    }

    public IEnumerator FadeIn()  => DoFade(1f, 0f);
    public IEnumerator FadeOut() => DoFade(0f, 1f);

    IEnumerator DoFade(float from, float to)
    {
        if (!fadeImage) yield break;
        float t = 0;
        var c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }
}
