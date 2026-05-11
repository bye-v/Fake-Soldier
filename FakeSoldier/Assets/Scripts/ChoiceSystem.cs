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

    ChoiceData[] currentChoices;
    Action<int> onChosen;
    int selectedIndex = 0;
    bool active;

    float timeLeft;
    bool hasTimer;
    Coroutine timerCoroutine;

    // 양심 확인 상태
    bool awaitingConfirm;
    int  confirmTargetIdx;
    ChoiceData[] originalChoices;

    static readonly ChoiceData[] s_ConfirmChoices = new[]
    {
        new ChoiceData { text = "예, 그것이 나의 선택입니다." },
        new ChoiceData { text = "아니오, 다시 생각해보겠습니다." },
    };

    // 스테이지별/맥락별 양심 메시지 목록
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

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            SetSelected((selectedIndex - 1 + currentChoices.Length) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            SetSelected((selectedIndex + 1) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            Confirm(selectedIndex);
    }

    public void Show(ChoiceData[] choices, Action<int> callback, float countdown = 0f)
    {
        currentChoices = choices;
        onChosen       = callback;
        selectedIndex  = 0;
        active         = true;
        awaitingConfirm = false;
        choicePanel.SetActive(true);

        RefreshButtons(choices);
        SetSelected(0);

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

    void SetSelected(int idx)
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
        selectedIndex = idx;
    }

    void Confirm(int idx)
    {
        if (!active) return;

        // ── 양심 확인 모드 응답 ──
        if (awaitingConfirm)
        {
            if (idx == 0)
            {
                // "예" → 원래 선택 강행
                awaitingConfirm = false;
                currentChoices  = originalChoices;
                HideGuiltMessage();
                DoConfirm(confirmTargetIdx);
            }
            else
            {
                // "아니오" → 원래 선택지로 복귀
                awaitingConfirm = false;
                currentChoices  = originalChoices;
                HideGuiltMessage();
                RefreshButtons(originalChoices);
                SetSelected(confirmTargetIdx);
            }
            return;
        }

        // ── 부정적 선택 → 양심 확인 발동 ──
        if (currentChoices[idx].scoreChange < 0)
        {
            ShowGuiltConfirm(idx);
            return;
        }

        DoConfirm(idx);
    }

    void ShowGuiltConfirm(int idx)
    {
        awaitingConfirm   = true;
        confirmTargetIdx  = idx;
        originalChoices   = currentChoices;
        currentChoices    = s_ConfirmChoices;

        // timerText를 양심 메시지 표시용으로 전용
        if (timerText)
        {
            timerText.gameObject.SetActive(true);
            timerText.text      = s_GuiltMessages[guiltMsgIndex % s_GuiltMessages.Length];
            timerText.color     = new Color(1f, 0.78f, 0.78f, 1f);
            timerText.fontSize  = 26f;
            timerText.alignment = TextAlignmentOptions.Center;
            guiltMsgIndex++;
        }

        RefreshButtons(s_ConfirmChoices);
        SetSelected(1);  // 기본값: "아니오, 다시 생각해보겠습니다."
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

        var choice = currentChoices[idx];
        GameManager.Instance.AddScore(choice.scoreChange);
        if (choice.isFiringRefusal) GameManager.Instance.SetFiringRefused();

        onChosen?.Invoke(idx);
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
