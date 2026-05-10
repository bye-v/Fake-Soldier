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

        // W/S로 선택지 탐색, Enter로 확인
        if (Input.GetKeyDown(KeyCode.W))
            SetSelected((selectedIndex - 1 + currentChoices.Length) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.S))
            SetSelected((selectedIndex + 1) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            Confirm(selectedIndex);
    }

    public void Show(ChoiceData[] choices, Action<int> callback, float countdown = 0f)
    {
        currentChoices = choices;
        onChosen = callback;
        selectedIndex = 0;
        active = true;
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool visible = i < choices.Length;
            choiceButtons[i].gameObject.SetActive(visible);
            if (visible) choiceTexts[i].text = $"{i + 1}. {choices[i].text}";
        }
        SetSelected(0);

        hasTimer = countdown > 0;
        if (timerText) timerText.gameObject.SetActive(hasTimer);
        if (hasTimer)
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            timerCoroutine = StartCoroutine(RunTimer(countdown));
        }
    }

    void SetSelected(int idx)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (!choiceButtons[i].gameObject.activeSelf) continue;
            bool sel = (i == idx);

            // 버튼 배경색 (alpha=0이어서 colorBlock으로는 안 보이므로 직접 설정)
            var img = choiceButtons[i].GetComponent<Image>();
            if (img) img.color = sel
                ? new Color(0.9f, 0.75f, 0.1f, 0.28f)
                : new Color(1f, 1f, 1f, 0.05f);

            // 텍스트 색상
            if (i < choiceTexts.Length && choiceTexts[i])
                choiceTexts[i].color = sel ? new Color(1f, 0.92f, 0.3f) : Color.white;
        }
        selectedIndex = idx;
    }

    void Confirm(int idx)
    {
        if (!active) return;
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
        // 시간 초과 → 첫 번째 선택(명령 이행) 강제 선택
        Confirm(0);
    }

    public bool IsActive => active;
}
