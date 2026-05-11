using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour
{
    [SerializeField] RectTransform creditContent;
    [SerializeField] float scrollSpeed = 60f;
    [SerializeField] Button skipButton;
    [SerializeField] AudioClip creditBGM;

    void Start()
    {
        if (creditBGM != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayBGMOnce(creditBGM);

        skipButton?.onClick.AddListener(GoTitle);
        StartCoroutine(ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        if (!creditContent) yield break;
        float startY = creditContent.anchoredPosition.y;
        float endY = startY + creditContent.rect.height + Screen.height;

        while (creditContent.anchoredPosition.y < endY)
        {
            var pos = creditContent.anchoredPosition;
            pos.y += scrollSpeed * Time.deltaTime;
            creditContent.anchoredPosition = pos;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        GoTitle();
    }

    void GoTitle() => SceneManager.LoadScene("MainMenu");
}
