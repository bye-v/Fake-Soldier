using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Button   creditButton;
    [SerializeField] Button   retryButton;

    [Header("Ending Content")]
    [SerializeField] EndingType endingType;
    [TextArea(3, 8)]
    [SerializeField] string endingText;

    // Bad Ending 전용
    [Header("Bad Ending Only")]
    [SerializeField] TMP_Text victimCountText;
    [SerializeField] int      victimCount = 166;

    [Header("Audio")]
    [SerializeField] AudioClip endingBGM;

    void Start()
    {
        if (GameManager.Instance != null)
            endingType = GameManager.Instance.GetEnding();

        if (endingBGM != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(endingBGM);

        SetupUI();

        creditButton?.onClick.AddListener(() => SceneManager.LoadScene("Credit"));
        retryButton?.onClick.AddListener(OnRetry);
    }

    void SetupUI()
    {
        if (scoreText && GameManager.Instance != null)
            scoreText.text = $"양심 점수: {GameManager.Instance.ConscienceScore}";

        switch (endingType)
        {
            case EndingType.Bad:
                if (titleText) titleText.text = "복종 엔딩";
                if (bodyText)  bodyText.text  = string.IsNullOrEmpty(endingText)
                    ? "당신은 명령을 따랐다.\n표창장이 수여되었다.\n\n그 여름, 광주에서 무슨 일이 있었는지\n당신은 알고 있었다."
                    : endingText;
                if (victimCountText) victimCountText.text = $"5.18 민주화운동 희생자 수: {victimCount}명";
                break;

            case EndingType.Normal:
                if (titleText) titleText.text = "방관 엔딩";
                if (bodyText)  bodyText.text  = string.IsNullOrEmpty(endingText)
                    ? "당신은 명령을 거부하지 않았다.\n하지만 온전히 따르지도 않았다.\n\n군법회의에 회부되었다.\n무엇이 옳았는지, 평생 물음만 남았다."
                    : endingText;
                break;

            case EndingType.True:
                if (titleText) titleText.text = "거부 엔딩";
                if (bodyText)  bodyText.text  = string.IsNullOrEmpty(endingText)
                    ? "당신은 총을 내려놓았다.\n탈영병이라 불렸다.\n\n하지만 시민들 사이에 섞여 걸어가는 당신의 뒷모습은\n처음으로 진짜 군인처럼 보였다."
                    : endingText;
                break;
        }
    }

    void OnRetry()
    {
        if (GameManager.Instance) GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MainMenu");
    }
}
