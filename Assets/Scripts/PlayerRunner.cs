using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 6f;
    [SerializeField] private float lateralSpeed = 5f;
    [SerializeField] private float lateralLimit = 4f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float swipeThreshold = 100f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 touchStartPosition;
    private bool isTouching;

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

        HandleTouchInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MoveForward();
        MoveLateral();
    }

    private void MoveForward()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.z = forwardSpeed;

        rb.linearVelocity = velocity;
    }

    private void MoveLateral()
    {
        float horizontalInput = 0f;

        if (isTouching)
        {
            Vector2 currentTouchPosition = Input.GetTouch(0).position;

            float horizontalDifference = currentTouchPosition.x - touchStartPosition.x;

            horizontalInput = Mathf.Clamp(horizontalDifference / 200f,-1f,1f);
        }

        Vector3 velocity = rb.linearVelocity;

        velocity.x = horizontalInput * lateralSpeed;

        float nextX = transform.position.x +velocity.x * Time.fixedDeltaTime;

        nextX = Mathf.Clamp(nextX,-lateralLimit,lateralLimit);

        if (nextX <= -lateralLimit && velocity.x < 0)
        {
            velocity.x = 0f;
        }

        if (nextX >= lateralLimit && velocity.x > 0)
        {
            velocity.x = 0f;
        }

        rb.linearVelocity = velocity;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            isTouching = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStartPosition = touch.position;
                isTouching = true;
                break;

            case TouchPhase.Ended:
                Vector2 swipeDistance = touch.position - touchStartPosition;
                // Swipe upward to jump
                if (swipeDistance.y > swipeThreshold && Mathf.Abs(swipeDistance.y) > Mathf.Abs(swipeDistance.x) && isGrounded)
                {
                    Jump();
                }
                isTouching = false;
                break;

            case TouchPhase.Canceled:
                isTouching = false;
                break;
        }
    }

    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;

        velocity.y = 0f;

        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ContactPoint contact = collision.GetContact(0);

            Vector3 normal = contact.normal;

            // Mostly horizontal surface = player hit the top
            if (normal.y > 0.5f)
            {
                // Hit the top of the obstacle
                animator.SetTrigger("Fall1");
                Debug.Log("Fall1 Triggered");
            }
            else
            {
                Vector3 localNormal = transform.InverseTransformDirection(normal);

                // Side collision
                if (Mathf.Abs(localNormal.x) > Mathf.Abs(localNormal.z))
                {
                    animator.SetTrigger("Fall3");
                    Debug.Log("Fall3 Triggered");
                }
                // Front face collision
                else
                {
                    animator.SetTrigger("Fall2");
                    Debug.Log("Fall2 Triggered");
                }
            }
        }

        if (collision.gameObject.CompareTag("Oil"))
        {
            animator.SetTrigger("Fall4");
            Debug.Log("Fall4 Triggered");
        }
    }



    

}