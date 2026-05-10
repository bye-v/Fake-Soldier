using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("Animator 기본 방향 (0=S, 1=N, 2=E, 3=W)")]
    [SerializeField] Vector2 defaultFaceDir = new Vector2(0, -1);

    bool playerInRange;
    bool hasSpoken;
    PlayerController cachedPlayer;
    Animator animator;
    GameObject autoIndicator;  // 자동 생성된 [E] 표시기

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        animator = GetComponent<Animator>();
        if (interactIndicator) interactIndicator.SetActive(false);
    }

    void Start()
    {
        // Animator 기본 방향 설정 (Idle BlendTree가 올바른 방향 스프라이트를 표시)
        if (animator)
        {
            animator.SetFloat("MoveX", defaultFaceDir.x);
            animator.SetFloat("MoveY", defaultFaceDir.y);
        }

        // interactIndicator가 없으면 간단한 [E] 텍스트 자동 생성
        if (!interactIndicator && dialogueLines.Count > 0)
            CreateAutoIndicator();
    }

    void CreateAutoIndicator()
    {
        autoIndicator = new GameObject("InteractIndicator");
        autoIndicator.transform.SetParent(transform, false);
        autoIndicator.transform.localPosition = new Vector3(0, 3.2f, 0);

        var tmp = autoIndicator.AddComponent<TextMeshPro>();
        tmp.text         = "[ E ]";
        tmp.fontSize     = 6f;
        tmp.alignment    = TextAlignmentOptions.Center;
        tmp.color        = new Color(1f, 0.92f, 0.25f);
        tmp.sortingOrder = 10;

        autoIndicator.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;
        if (ChoiceSystem.Instance != null && ChoiceSystem.Instance.IsActive) return;
        if (!repeatDialogue && hasSpoken) return;
        if (Input.GetKeyDown(KeyCode.E)) TalkToPlayer();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        cachedPlayer = other.GetComponent<PlayerController>();
        if (interactIndicator) interactIndicator.SetActive(true);
        if (autoIndicator) autoIndicator.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (interactIndicator) interactIndicator.SetActive(false);
        if (autoIndicator) autoIndicator.SetActive(false);
    }

    void TalkToPlayer()
    {
        if (dialogueLines.Count == 0) return;
        hasSpoken = true;
        cachedPlayer?.SetCanMove(false);
        // speaker가 비어 있으면 npcName으로 자동 채움
        foreach (var line in dialogueLines)
            if (string.IsNullOrEmpty(line.speaker)) line.speaker = npcName;
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
