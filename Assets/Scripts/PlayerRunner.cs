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

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverUI;

    private Vector2 touchStartPosition;
    private bool isTouching;

    private bool inputLocked;

    // Prevents the touch that closed a puzzle
    // from being interpreted as a jump.
    private bool waitingForTouchRelease;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded;
    private bool isFalling;
    private bool gameEnded;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (inputLocked)
            return;

        CheckGround();

        HandleTouchInput();
        UpdateAnimator();

        CheckFallAnimationFinished();
    }

    private void FixedUpdate()
    {
        if (gameEnded || isFalling)
            return;

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

        if (isTouching && !waitingForTouchRelease)
        {
            Vector2 currentTouchPosition = Input.GetTouch(0).position;

            float horizontalDifference = currentTouchPosition.x - touchStartPosition.x;

            horizontalInput = Mathf.Clamp(horizontalDifference / 200f, -1f, 1f);
        }

        Vector3 velocity = rb.linearVelocity;

        velocity.x = horizontalInput * lateralSpeed;

        float nextX =transform.position.x +velocity.x * Time.fixedDeltaTime;

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
        if (gameEnded)
            return;

        /*
         * IMPORTANT:
         * If the puzzle was just closed while the player's
         * finger was still touching the screen, wait until
         * that touch completely disappears.
         */
        if (waitingForTouchRelease)
        {
            if (Input.touchCount == 0)
            {
                waitingForTouchRelease = false;
            }

            isTouching = false;

            return;
        }

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

                /*
                 * Swipe upward to jump.
                 */
                if (swipeDistance.y > swipeThreshold &&Mathf.Abs(swipeDistance.y) > Mathf.Abs(swipeDistance.x) &&isGrounded)
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
        if (isFalling)
            return;

        animator.SetBool("IsJumping",!isGrounded);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameEnded)
            return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ContactPoint contact = collision.GetContact(0);

            Vector3 normal = contact.normal;

            rb.linearVelocity = Vector3.zero;

            if (normal.y > 0.5f)
            {
                StartFall("Fall1");
            }
            else
            {
                Vector3 localNormal =transform.InverseTransformDirection(normal);

                if (Mathf.Abs(localNormal.x) > Mathf.Abs(localNormal.z))
                {
                    StartFall("Fall3");
                }
                else
                {
                    StartFall("Fall2");
                }
            }
        }

        if (collision.gameObject.CompareTag("Oil"))
        {
            StartFall("Fall4");
        }

        if (collision.gameObject.CompareTag("SpikeTrap"))
        {
            StartFall("Fall6");
        }

        if (collision.gameObject.CompareTag("SawBlade"))
        {
            StartFall("Fall7");
        }

        if (collision.gameObject.CompareTag("PressTrap"))
        {
            StartFall("Fall8");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded || isFalling)
            return;

        if (other.CompareTag("PoisonTrap"))
        {
            StartFall("Stun");
        }

        if (other.CompareTag("FlameThrower"))
        {
            StartFall("Crawl");
        }

        if (other.CompareTag("JumpTrap"))
        {
            StartFall("Fall5");
        }
    }

    private void StartFall(string fallTrigger)
    {
        if (isFalling || gameEnded)
            return;

        isFalling = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        animator.SetTrigger(fallTrigger);

        Debug.Log(fallTrigger + " Triggered");
    }

    private void CheckFallAnimationFinished()
    {
        if (!isFalling || gameEnded)
            return;

        AnimatorStateInfo stateInfo =
            animator.GetCurrentAnimatorStateInfo(0);

        bool playingFallAnimation =
            stateInfo.IsName("FallFlat") ||
            stateInfo.IsName("FallOver") ||
            stateInfo.IsName("FallingDown") ||
            stateInfo.IsName("SweepFall") ||
            stateInfo.IsName("FlyingBackDown") ||
            stateInfo.IsName("StandingDeathForward") ||
            stateInfo.IsName("DyingBackwards") ||
            stateInfo.IsName("StandingDeathLeft") ||
            stateInfo.IsName("DrunkWalking") ||
            stateInfo.IsName("CrawlBackwards");

        if (playingFallAnimation)
        {
            if (stateInfo.normalizedTime >= 1f && !stateInfo.loop)
            {
                EndGame();
            }
        }
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;

        if (locked)
        {
            isTouching = false;
            touchStartPosition = Vector2.zero;
        }
        else
        {
            /*
             * If a touch is still active when the puzzle closes,
             * do NOT let that same touch become a player input.
             */
            if (Input.touchCount > 0)
            {
                waitingForTouchRelease = true;
            }
        }
    }

    private void EndGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        isFalling = false;

        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Debug.Log("Game Over - Fall animation completed");
    }
}