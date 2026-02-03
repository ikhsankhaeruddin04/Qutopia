using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private enum State { idle, run, jump, falling, hurt }
    private State state = State.idle;

    private Collider2D coll;

    [SerializeField] private LayerMask Ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryCountText;

    [SerializeField] private float hurtforce = 10f;
    [SerializeField] private int health = 3;
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;

    [SerializeField] private int extraJumpsValue = 1;
    private int extraJumps;

    private Vector3 respawnPoint;
    private GameOverManager gameOverManager;

    [Header("Joystick Support")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private Button jumpButton;
    private bool jumpPressed;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip runSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        extraJumps = extraJumpsValue;

        if (cherryCountText != null)
            cherryCountText.text = cherries.ToString();

        UpdateHearts();
        respawnPoint = transform.position;

        gameOverManager = FindObjectOfType<GameOverManager>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(OnJumpButtonPressed);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (NPC.isTalking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetInteger("State", (int)State.idle);
            return;
        }

        if (IsGrounded())
        {
            extraJumps = extraJumpsValue;
        }

        if (state != State.hurt)
        {
            Movement();
        }

        VelocityState();
        anim.SetInteger("State", (int)state);

        if (transform.position.y < -10f)
        {
            StartCoroutine(TakeDamage(Vector2.zero, true));
        }
    }

    private void Movement()
    {
        float hDirection = (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.1f)
            ? joystick.Horizontal
            : Input.GetAxisRaw("Horizontal");

        if (hDirection > 0.1f)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        else if (hDirection < -0.1f)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        bool jumpInput = Input.GetButtonDown("Jump") || jumpPressed;
        if (jumpInput)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpforce);
                state = State.jump;
                if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            }
            else if (extraJumps > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpforce);
                state = State.jump;
                extraJumps--;
                anim.SetTrigger("DoubleJump");
                if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
            }

            jumpPressed = false;
        }

        // Play run sound if grounded & moving
        if (Mathf.Abs(rb.velocity.x) > 0.1f && IsGrounded())
        {
            if (runSound != null && !audioSource.isPlaying)
                audioSource.PlayOneShot(runSound);
        }
    }

    public void OnJumpButtonPressed()
    {
        jumpPressed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectable"))
{
    Destroy(collision.gameObject);
    cherries++;

    if (cherryCountText != null)
        cherryCountText.text = cherries.ToString();

    // Panggil pengecekan sisa collectable di frame berikutnya
    StartCoroutine(CheckRemainingCollectables());
}

        else if (collision.CompareTag("Trap"))
        {
            Vector2 knockback = new Vector2(-transform.localScale.x * hurtforce, rb.velocity.y);
            StartCoroutine(TakeDamage(knockback, false));
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            SetCheckpoint(collision.transform.position);
        }
    }
private IEnumerator CheckRemainingCollectables()
{
    yield return null; // tunggu satu frame

    int sisa = GameObject.FindGameObjectsWithTag("Collectable").Length;
    Debug.Log("Sisa collectable: " + sisa);

    if (sisa == 0)
    {
        GateController gate = FindObjectOfType<GateController>();
        if (gate != null)
        {
            Debug.Log("OpenGate dipanggil!");
            gate.OpenGate();
        }
    }
}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (state == State.falling)
            {
                Destroy(other.gameObject);
            }
            else
            {
                Vector2 knockback = (other.transform.position.x > transform.position.x)
                    ? new Vector2(-hurtforce, rb.velocity.y)
                    : new Vector2(hurtforce, rb.velocity.y);

                StartCoroutine(TakeDamage(knockback, false));
            }
        }
    }

    private IEnumerator TakeDamage(Vector2 knockback, bool forceRespawn = false)
    {
        state = State.hurt;
        anim.SetTrigger("hit");
        rb.velocity = knockback;

        HandleHealth();

        if (health > 0)
        {
            yield return new WaitForSeconds(0.5f);

            if (forceRespawn)
            {
                transform.position = respawnPoint;
                rb.velocity = Vector2.zero;
            }

            state = State.idle;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            if (gameOverManager != null)
            {
                gameOverManager.ShowMenu(true);
            }
        }
    }

    private void HandleHealth()
    {
        health--;
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = i < health ? fullHeart : emptyHeart;
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }

    private void VelocityState()
    {
        if (state == State.jump)
        {
            if (rb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (IsGrounded())
            {
                state = State.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            state = State.run;
        }
        else
        {
            state = State.idle;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
