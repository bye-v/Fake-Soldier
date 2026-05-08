using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 0.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(transform.root.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
        if (fadeImage) fadeImage.color = Color.black;
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
