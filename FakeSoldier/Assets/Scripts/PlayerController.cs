using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;

    Rigidbody2D rb;
    Vector2 moveInput;
    Animator animator;
    bool canMove = true;

    static readonly int HashMoveX  = Animator.StringToHash("MoveX");
    static readonly int HashMoveY  = Animator.StringToHash("MoveY");
    static readonly int HashMoving = Animator.StringToHash("IsMoving");

    // 마지막 이동 방향 기억 (Idle 블렌드트리용)
    Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!canMove) { moveInput = Vector2.zero; return; }
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        if (moveInput != Vector2.zero) lastMoveDir = moveInput;

        if (animator == null) return;
        // 이동 중이면 현재 방향, 정지 중이면 마지막 방향 유지
        var dir = moveInput != Vector2.zero ? moveInput : lastMoveDir;
        animator.SetFloat(HashMoveX, dir.x);
        animator.SetFloat(HashMoveY, dir.y);
        animator.SetBool(HashMoving, moveInput != Vector2.zero);
    }

    public void SetCanMove(bool value) => canMove = value;
}
