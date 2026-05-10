using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 5)]
    public string text;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text speakerText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] GameObject continueIndicator;

    [Header("Settings")]
    [SerializeField] float typeSpeed = 0.04f;

    Queue<DialogueLine> lineQueue = new();
    bool isTyping;
    bool skipRequested;
    Action onComplete;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;
        bool advance = Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0);
        if (!advance) return;
        if (isTyping) skipRequested = true;
        else          NextLine();
    }

    public void StartDialogue(IEnumerable<DialogueLine> lines, Action onDone = null)
    {
        onComplete = onDone;
        lineQueue.Clear();
        foreach (var l in lines) lineQueue.Enqueue(l);
        dialoguePanel.SetActive(true);
        if (continueIndicator) continueIndicator.SetActive(false);
        NextLine();
    }

    // 내면 독백 색상 (하늘색 계열로 일반 대사와 구분)
    static readonly Color InnerVoiceColor   = new Color(0.6f, 0.85f, 1f);
    static readonly Color NormalSpeakerColor = Color.white;

    void NextLine()
    {
        if (lineQueue.Count == 0) { EndDialogue(); return; }
        var line = lineQueue.Dequeue();

        bool isInner = line.speaker.Contains("속");
        if (speakerText)
        {
            speakerText.text  = line.speaker;
            speakerText.color = isInner ? InnerVoiceColor : NormalSpeakerColor;
            speakerText.fontStyle = isInner
                ? TMPro.FontStyles.Italic
                : TMPro.FontStyles.Normal;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine(line.text, isInner));
    }

    IEnumerator TypeLine(string text, bool isInner = false)
    {
        isTyping = true;
        skipRequested = false;
        if (continueIndicator) continueIndicator.SetActive(false);

        bodyText.color     = isInner ? InnerVoiceColor : NormalSpeakerColor;
        bodyText.fontStyle = isInner ? TMPro.FontStyles.Italic : TMPro.FontStyles.Normal;
        bodyText.text = "";

        foreach (char c in text)
        {
            if (skipRequested) { bodyText.text = text; break; }
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        if (continueIndicator) continueIndicator.SetActive(true);
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        onComplete?.Invoke();
    }

    public bool IsActive => dialoguePanel.activeSelf;
}
