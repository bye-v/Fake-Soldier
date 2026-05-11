using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ChoiceData
{
    public string text;
    public int scoreChange;
    public bool isFiringRefusal;
}

public class ChoiceSystem : MonoBehaviour
{
    public static ChoiceSystem Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] GameObject choicePanel;
    [SerializeField] Button[] choiceButtons;
    [SerializeField] TMP_Text[] choiceTexts;
    [SerializeField] TMP_Text timerText;

    [Header("Settings")]
    [SerializeField] Color highlightColor = Color.yellow;
    [SerializeField] Color normalColor    = Color.white;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip hoverSFX;

    ChoiceData[] currentChoices;
    Action<int> onChosen;
    int selectedIndex = 0;
    bool active;

    // 선택지가 열린 직후 입력 무시 (대화창 클릭과의 충돌 방지)
    float shownTime;
    const float inputDelay = 0.18f;

    float timeLeft;
    bool hasTimer;
    Coroutine timerCoroutine;

    bool awaitingConfirm;
    int  confirmTargetIdx;
    ChoiceData[] originalChoices;

    static readonly ChoiceData[] s_ConfirmChoices = new[]
    {
        new ChoiceData { text = "예, 그것이 나의 선택입니다." },
        new ChoiceData { text = "아니오, 다시 생각해보겠습니다." },
    };

    static readonly string[] s_GuiltMessages = new[]
    {
        "정말로 그러하시겠습니까?",
        "이 선택의 무게를 느끼십니까?",
        "이 길을 선택하면 되돌아올 수 없습니다.",
        "당신의 양심은 무엇이라 말합니까?",
    };
    int guiltMsgIndex = 0;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        choicePanel.SetActive(false);
        if (timerText) timerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!active) return;
        // 선택지가 열린 직후 짧은 시간 동안 입력 무시
        if (Time.time - shownTime < inputDelay) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            SetSelected((selectedIndex - 1 + currentChoices.Length) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            SetSelected((selectedIndex + 1) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            Confirm(selectedIndex);
    }

    public void Show(ChoiceData[] choices, Action<int> callback, float countdown = 0f)
    {
        currentChoices  = choices;
        onChosen        = callback;
        selectedIndex   = 0;
        active          = true;
        awaitingConfirm = false;
        shownTime       = Time.time;
        choicePanel.SetActive(true);

        RefreshButtons(choices);
        SetSelected(0, false);

        hasTimer = countdown > 0;
        if (timerText) timerText.gameObject.SetActive(hasTimer);
        if (hasTimer)
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(RunTimer(countdown));
        }
    }

    void RefreshButtons(ChoiceData[] choices)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool visible = i < choices.Length;
            choiceButtons[i].gameObject.SetActive(visible);
            if (visible) choiceTexts[i].text = $"{i + 1}. {choices[i].text}";
        }
    }

    void SetSelected(int idx, bool playSound = true)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (!choiceButtons[i].gameObject.activeSelf) continue;
            bool sel = (i == idx);
            var img = choiceButtons[i].GetComponent<Image>();
            if (img) img.color = sel
                ? new Color(0.9f, 0.75f, 0.1f, 0.28f)
                : new Color(1f, 1f, 1f, 0.05f);
            if (i < choiceTexts.Length && choiceTexts[i])
                choiceTexts[i].color = sel ? new Color(1f, 0.92f, 0.3f) : Color.white;
        }
        if (playSound && idx != selectedIndex && hoverSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(hoverSFX);
        selectedIndex = idx;
    }

    void Confirm(int idx)
    {
        if (!active) return;

        if (awaitingConfirm)
        {
            if (idx == 0)
            {
                awaitingConfirm = false;
                currentChoices  = originalChoices;
                HideGuiltMessage();
                DoConfirm(confirmTargetIdx);
            }
            else
            {
                awaitingConfirm = false;
                currentChoices  = originalChoices;
                HideGuiltMessage();
                RefreshButtons(originalChoices);
                SetSelected(confirmTargetIdx);
            }
            return;
        }

        if (currentChoices[idx].scoreChange < 0)
        {
            ShowGuiltConfirm(idx);
            return;
        }

        DoConfirm(idx);
    }

    void ShowGuiltConfirm(int idx)
    {
        awaitingConfirm  = true;
        confirmTargetIdx = idx;
        originalChoices  = currentChoices;
        currentChoices   = s_ConfirmChoices;
        shownTime        = Time.time; // 양심 팝업 열릴 때도 딜레이 초기화

        if (timerText)
        {
            timerText.gameObject.SetActive(true);
            timerText.text               = s_GuiltMessages[guiltMsgIndex % s_GuiltMessages.Length];
            timerText.color              = new Color(1f, 0.78f, 0.78f, 1f);
            timerText.fontSize           = 22f;
            timerText.alignment          = TextAlignmentOptions.Center;
            timerText.enableWordWrapping = false;
            timerText.overflowMode       = TextOverflowModes.Overflow;
            guiltMsgIndex++;
        }

        RefreshButtons(s_ConfirmChoices);
        SetSelected(1, false);
    }

    void HideGuiltMessage()
    {
        if (timerText && !hasTimer)
            timerText.gameObject.SetActive(false);
    }

    void DoConfirm(int idx)
    {
        active = false;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (timerText) timerText.gameObject.SetActive(false);
        choicePanel.SetActive(false);

        if (clickSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clickSFX);

        var choice = currentChoices[idx];
        GameManager.Instance.AddScore(choice.scoreChange);
        if (choice.isFiringRefusal) GameManager.Instance.SetFiringRefused();

        if (choice.scoreChange != 0)
            StartCoroutine(ShowScoreFeedback(choice.scoreChange));

        onChosen?.Invoke(idx);
    }

    IEnumerator ShowScoreFeedback(int scoreChange)
    {
        var canvas = choicePanel.GetComponentInParent<Canvas>();
        if (!canvas) yield break;

        var go = new GameObject("ScoreFeedback");
        go.transform.SetParent(canvas.transform, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.42f);
        rt.sizeDelta = new Vector2(400f, 80f);
        rt.anchoredPosition = Vector2.zero;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        bool positive = scoreChange > 0;
        tmp.text      = positive ? "양심 +" + scoreChange : "양심 " + scoreChange;
        tmp.fontSize  = 38f;
        tmp.color     = positive ? new Color(0.45f, 1f, 0.55f) : new Color(1f, 0.35f, 0.35f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        float duration = 1.6f;
        float elapsed  = 0f;
        var   startPos = rt.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rt.anchoredPosition = startPos + new Vector2(0f, t * 110f);
            var c = tmp.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            tmp.color = c;
            yield return null;
        }
        Destroy(go);
    }

    IEnumerator RunTimer(float duration)
    {
        timeLeft = duration;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timerText) timerText.text = $"{Mathf.CeilToInt(timeLeft)}초";
            yield return null;
        }
        Confirm(0);
    }

    public bool IsActive => active;
}
