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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping) skipRequested = true;
            else          NextLine();
        }
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

    void NextLine()
    {
        if (lineQueue.Count == 0) { EndDialogue(); return; }
        var line = lineQueue.Dequeue();
        if (speakerText) speakerText.text = line.speaker;
        StopAllCoroutines();
        StartCoroutine(TypeLine(line.text));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        skipRequested = false;
        if (continueIndicator) continueIndicator.SetActive(false);
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
