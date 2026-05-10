using System.Collections;
using UnityEngine;

public class ControlsHintController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float displayDuration = 5f;
    [SerializeField] float fadeDuration    = 1.5f;

    bool fading;

    void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(AutoFade());
    }

    void Update()
    {
        if (fading) return;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            StartCoroutine(FadeOut());
    }

    IEnumerator AutoFade()
    {
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        if (fading) yield break;
        fading = true;
        float elapsed = 0, start = canvasGroup.alpha;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, elapsed / fadeDuration);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
