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

    [Header("Bad Ending Only")]
    [SerializeField] TMP_Text victimCountText;
    [SerializeField] int      victimCount = 166;

    [Header("Audio")]
    [SerializeField] AudioClip endingBGM;

    [Header("Typewriter")]
    [SerializeField] float typeSpeed = 0.045f;

    bool skipTyping;
    bool typingDone;

    void Start()
    {
        if (GameManager.Instance != null)
            endingType = GameManager.Instance.GetEnding();

        if (endingBGM != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayBGMOnce(endingBGM);

        // 텍스트 영역 높이 확장 + 자동 폰트 크기 조정
        if (bodyText)
        {
            var rt = bodyText.GetComponent<UnityEngine.RectTransform>();
            if (rt) rt.sizeDelta = new Vector2(rt.sizeDelta.x, 380f);
            bodyText.enableAutoSizing = true;
            bodyText.fontSizeMin = 16f;
            bodyText.fontSizeMax = 28f;
        }

        string body = BuildUI();
        StartCoroutine(TypeBodyText(body));

        creditButton?.onClick.AddListener(() => SceneManager.LoadScene("Credit"));
        retryButton?.onClick.AddListener(OnRetry);
    }

    void Update()
    {
        // 클릭 또는 스페이스바로 타이핑 스킵
        if (!typingDone && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            skipTyping = true;
    }

    // UI를 구성하고 body 텍스트 문자열을 반환
    string BuildUI()
    {
        if (scoreText && GameManager.Instance != null)
        {
            int score = GameManager.Instance.ConscienceScore;
            string label = score >= 7 ? "양심을 지켰다" : score >= 3 ? "흔들렸다" : "명령에 복종했다";
            scoreText.text = label;
        }

        bool refusedFiring = GameManager.Instance != null && GameManager.Instance.HasRefusedFiring;
        string body = "";

        switch (endingType)
        {
            case EndingType.Bad:
                if (titleText) titleText.text = "복종 (服從)";
                body = string.IsNullOrEmpty(endingText)
                    ? "당신은 명령을 따랐다.\n상부에서 표창장이 수여되었다.\n'임무 완수'라는 말이 적혀 있었다.\n\n전역 후, 광주 뉴스가 나올 때마다\n당신은 조용히 채널을 돌렸다.\n\n그 여름, 광주에서 무슨 일이 있었는지—\n당신은 끝까지 알고 있었다.\n아무에게도 말하지 못한 채."
                    : endingText;
                if (victimCountText) victimCountText.text =
                    $"5·18 민주화운동 공식 희생자\n사망자 {victimCount}명  |  행방불명 54명  |  부상자 3,139명";
                break;

            case EndingType.Normal:
                if (titleText) titleText.text = "방관 (傍觀)";
                body = string.IsNullOrEmpty(endingText)
                    ? "당신은 명령과 양심 사이에서 줄타기를 했다.\n손을 더럽히지도, 그렇다고 막아서지도 않았다.\n\n살아서 돌아왔다. 전역도 했다.\n아이도 낳고, 평범한 이름으로 평범한 삶을 살았다.\n\n하지만 매년 오월이 오면\n식탁에 앉은 채 멍하니 창밖을 바라보았다.\n\n그날, 당신이 아무것도 하지 않는 동안\n누군가는 죽었다.\n\n방관도 선택이다.\n침묵도 역사다.\n그 사실은 당신을 평생 따라다녔다."
                    : endingText;
                if (victimCountText) victimCountText.gameObject.SetActive(false);
                break;

            case EndingType.True:
                if (titleText) titleText.text = "거부 (拒否)";
                if (!string.IsNullOrEmpty(endingText))
                    body = endingText;
                else if (refusedFiring)
                    body = "당신은 총을 땅에 내려놓았다.\n탈영병이라 불렸다. 겁쟁이라고도 했다.\n\n하지만 그날, 전남도청 앞에서\n당신의 총구는 시민을 향하지 않았다.\n\n훗날 5·18이 역사가 되고, 진실이 밝혀졌을 때\n당신은 처음으로 입을 열었다.\n\n\"나는 그날, 처음으로 진짜 군인이 되었습니다.\"";
                else
                    body = "당신은 명령에 등을 돌렸다.\n탈영병이라 불렸다. 겁쟁이라고도 했다.\n\n하지만 당신의 손에는 시민의 피가 없었다.\n\n훗날 5·18이 역사가 되었을 때\n당신은 처음으로 그날의 이야기를 꺼냈다.\n\n\"나는 그날, 사람이기로 했습니다.\"";
                if (victimCountText) victimCountText.gameObject.SetActive(false);
                break;
        }

        return body;
    }

    IEnumerator TypeBodyText(string text)
    {
        if (!bodyText) { typingDone = true; yield break; }

        // 전체 텍스트를 먼저 설정해 TMP가 폰트 크기와 레이아웃을 계산하게 함
        bodyText.text = text;
        bodyText.maxVisibleCharacters = 0;

        // 한 프레임 대기 후 TMP 텍스트 정보 확정
        yield return null;
        int total = bodyText.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            if (skipTyping) { bodyText.maxVisibleCharacters = total; break; }
            bodyText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeSpeed);
        }

        bodyText.maxVisibleCharacters = total;
        typingDone = true;

        // True 엔딩: 타이핑 완료 후 7초 뒤 자동으로 크레딧으로 전환
        if (endingType == EndingType.True)
        {
            yield return new WaitForSeconds(7f);
            SceneManager.LoadScene("Credit");
        }
    }

    void OnRetry()
    {
        if (GameManager.Instance) GameManager.Instance.ResetGame();
        SceneManager.LoadScene("MainMenu");
    }
}
