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

    [Header("Voice Clips")]
    [Tooltip("0=p1 청년남, 1=p2 성인남, 2=p3 여성, 3=p4 노인")]
    [SerializeField] AudioClip[] voiceClips;

    Queue<DialogueLine> lineQueue = new();
    bool isTyping;
    bool skipRequested;
    Action onComplete;
    AudioClip currentVoiceClip;

    static readonly Color InnerVoiceColor    = new Color(0.6f, 0.85f, 1f);
    static readonly Color NormalSpeakerColor = Color.white;

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

    AudioClip GetVoiceClip(string speaker)
    {
        if (string.IsNullOrEmpty(speaker) || speaker.Contains("속")) return null;
        if (voiceClips == null) return null;

        // p4 (index 3): 노인
        if (speaker == "노인") return voiceClips.Length > 3 ? voiceClips[3] : null;
        // p3 (index 2): 여성, 손녀
        if (speaker == "여성" || speaker == "손녀") return voiceClips.Length > 2 ? voiceClips[2] : null;
        // p2 (index 1): 상관, 시민군, 부상자 — 성인 남성
        if (speaker == "상관" || speaker == "시민군" || speaker == "부상자") return voiceClips.Length > 1 ? voiceClips[1] : null;
        // p1 (index 0): 학생 — 청년 남성
        if (speaker == "학생") return voiceClips.Length > 0 ? voiceClips[0] : null;

        return null;
    }

    void NextLine()
    {
        if (lineQueue.Count == 0) { EndDialogue(); return; }
        var line = lineQueue.Dequeue();

        bool isInner = line.speaker.Contains("속");
        currentVoiceClip = GetVoiceClip(line.speaker);

        if (speakerText)
        {
            speakerText.text      = line.speaker;
            speakerText.color     = isInner ? InnerVoiceColor : NormalSpeakerColor;
            speakerText.fontStyle = isInner ? FontStyles.Italic : FontStyles.Normal;
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
        bodyText.fontStyle = isInner ? FontStyles.Italic : FontStyles.Normal;
        bodyText.text = "";

        // 타이핑 시작 시 목소리 재생 — 타이핑이 끝날 때까지 충분히 긴 시간으로
        if (!isInner && currentVoiceClip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayVoice(currentVoiceClip, 99f);

        foreach (char c in text)
        {
            if (skipRequested)
            {
                bodyText.text = text;
                break;
            }
            bodyText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        // 타이핑 종료(자연 완료 또는 스킵) 즉시 목소리 정지
        if (!isInner && AudioManager.Instance != null)
            AudioManager.Instance.StopVoice();

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
