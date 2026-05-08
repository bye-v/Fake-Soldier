using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class NPCController : MonoBehaviour
{
    [Header("NPC 정보")]
    [SerializeField] string npcName = "시민";

    [Header("대화")]
    [SerializeField] List<DialogueLine> dialogueLines = new();
    [SerializeField] bool repeatDialogue = false;

    [Header("상호작용")]
    [SerializeField] GameObject interactIndicator;

    bool playerInRange;
    bool hasSpoken;
    PlayerController cachedPlayer;

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        if (interactIndicator) interactIndicator.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;
        if (!repeatDialogue && hasSpoken) return;
        if (Input.GetKeyDown(KeyCode.E)) TalkToPlayer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        cachedPlayer = other.GetComponent<PlayerController>();
        if (interactIndicator) interactIndicator.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactIndicator) interactIndicator.SetActive(false);
    }

    void TalkToPlayer()
    {
        if (dialogueLines.Count == 0) return;
        hasSpoken = true;
        cachedPlayer?.SetCanMove(false);
        DialogueManager.Instance.StartDialogue(dialogueLines, () => cachedPlayer?.SetCanMove(true));
    }

    // StageDirector에서 직접 대화를 시작할 때 사용
    public void StartDialogueExternal(List<DialogueLine> lines, System.Action onDone = null)
    {
        if (DialogueManager.Instance == null) return;
        cachedPlayer?.SetCanMove(false);
        DialogueManager.Instance.StartDialogue(lines, () =>
        {
            cachedPlayer?.SetCanMove(true);
            onDone?.Invoke();
        });
    }
}
