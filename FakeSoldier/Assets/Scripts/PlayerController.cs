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

    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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

        // 좌우 방향에 따라 스프라이트 뒤집기
        if (sr != null && Mathf.Abs(moveInput.x) > 0.1f)
            sr.flipX = moveInput.x < 0;

        if (animator == null) return;
        animator.SetFloat(HashMoveX, moveInput.x);
        animator.SetFloat(HashMoveY, moveInput.y);
        animator.SetBool(HashMoving, moveInput != Vector2.zero);
    }

    public void SetCanMove(bool value) => canMove = value;
}
