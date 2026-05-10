using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class NPCHintController : MonoBehaviour
{
    [SerializeField] float fadeDuration = 1.2f;

    CanvasGroup cg;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        StartCoroutine(WaitAndShow());
    }

    IEnumerator WaitAndShow()
    {
        // ControlsHintController가 사라질 때까지 대기
        ControlsHintController controls = FindObjectOfType<ControlsHintController>(true);
        if (controls != null)
        {
            while (controls.gameObject.activeInHierarchy)
                yield return null;
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }

        yield return new WaitForSeconds(0.4f);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
    }
}
