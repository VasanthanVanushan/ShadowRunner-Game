using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckGround();

        HandleJump();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.z = forwardSpeed;

        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position,groundCheckRadius,groundLayer);

        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("IsJumping", !isGrounded);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }
}