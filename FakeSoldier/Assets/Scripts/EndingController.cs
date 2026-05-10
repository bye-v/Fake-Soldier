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
        {
            int score = GameManager.Instance.ConscienceScore;
            string label = score >= 7 ? "양심을 지켰다" : score >= 3 ? "흔들렸다" : "명령에 복종했다";
            scoreText.text = label;
        }

        bool refusedFiring = GameManager.Instance != null && GameManager.Instance.HasRefusedFiring;

        switch (endingType)
        {
            case EndingType.Bad:
                if (titleText) titleText.text = "복종 (服從)";
                if (bodyText)  bodyText.text  = string.IsNullOrEmpty(endingText)
                    ? "당신은 명령을 따랐다.\n상부에서 표창장이 수여되었다.\n'임무 완수'라는 말이 적혀 있었다.\n\n전역 후, 광주 뉴스가 나올 때마다\n당신은 조용히 채널을 돌렸다.\n\n그 여름, 광주에서 무슨 일이 있었는지—\n당신은 끝까지 알고 있었다.\n아무에게도 말하지 못한 채."
                    : endingText;
                if (victimCountText) victimCountText.text =
                    $"5·18 민주화운동 공식 희생자\n사망자 {victimCount}명  |  행방불명 54명  |  부상자 3,139명";
                break;

            case EndingType.Normal:
                if (titleText) titleText.text = "방관 (傍觀)";
                if (bodyText)  bodyText.text  = string.IsNullOrEmpty(endingText)
                    ? "당신은 명령과 양심 사이에서 줄타기를 했다.\n거부하지도, 온전히 따르지도 않았다.\n\n전역 후 당신은 평범하게 살았다.\n그러나 5월이 되면, 광주라는 이름이 나오면\n밥이 목으로 넘어가지 않았다.\n\n행동하지 않는 것도 하나의 선택이었다.\n그 무게는 평생 어깨 위에 남았다."
                    : endingText;
                if (victimCountText) victimCountText.gameObject.SetActive(false);
                break;

            case EndingType.True:
                if (titleText) titleText.text = "거부 (拒否)";
                if (bodyText)
                {
                    if (!string.IsNullOrEmpty(endingText))
                    {
                        bodyText.text = endingText;
                    }
                    else if (refusedFiring)
                    {
                        bodyText.text =
                            "당신은 총을 땅에 내려놓았다.\n탈영병이라 불렸다. 겁쟁이라고도 했다.\n\n하지만 그날, 전남도청 앞에서\n당신의 총구는 시민을 향하지 않았다.\n\n훗날 5·18이 역사가 되고, 진실이 밝혀졌을 때\n당신은 처음으로 입을 열었다.\n\n\"나는 그날, 처음으로 진짜 군인이 되었습니다.\"";
                    }
                    else
                    {
                        bodyText.text =
                            "당신은 명령에 등을 돌렸다.\n탈영병이라 불렸다. 겁쟁이라고도 했다.\n\n하지만 당신의 손에는 시민의 피가 없었다.\n\n훗날 5·18이 역사가 되었을 때\n당신은 처음으로 그날의 이야기를 꺼냈다.\n\n\"나는 그날, 사람이기로 했습니다.\"";
                    }
                }
                if (victimCountText) victimCountText.gameObject.SetActive(false);
                break;
        }
    }

    void OnRetry()
    {
        if (GameManager.Instance) GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MainMenu");
    }
}
