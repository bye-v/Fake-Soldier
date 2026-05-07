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

        // 숫자키 1/2/3
        for (int i = 0; i < currentChoices.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            { Confirm(i); return; }
        }

        // 방향키 탐색
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SetSelected((selectedIndex - 1 + currentChoices.Length) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            SetSelected((selectedIndex + 1) % currentChoices.Length);
        if (Input.GetKeyDown(KeyCode.Return))
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
            var colors = choiceButtons[i].colors;
            colors.normalColor = i == idx ? highlightColor : normalColor;
            choiceButtons[i].colors = colors;
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
            if (timerText) timerText.text = $"{Mathf.CeilToInt(timeLeft)}";
            yield return null;
        }
        // 시간 초과 → 첫 번째 선택(명령 이행) 강제 선택
        Confirm(0);
    }

    public bool IsActive => active;
}
