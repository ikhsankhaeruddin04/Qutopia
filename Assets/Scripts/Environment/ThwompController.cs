using System.Collections;
using UnityEngine;

public class ThwompController : MonoBehaviour
{
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float speedMin = 1f;
    [SerializeField] private float speedMax = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float waitTime = 0.5f;

    private Vector3 startPos;
    private bool movingDown = true;
    private float currentSpeed;
    private bool isWaiting = false;
    private Animator animator;

    void Start()
    {
        startPos = transform.position;
        currentSpeed = speedMin;
        animator = GetComponent<Animator>();
        animator.SetTrigger("Fall"); // Mulai dengan animasi jatuh
    }

    void Update()
    {
        if (isWaiting) return;

        currentSpeed += acceleration * Time.deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, speedMin, speedMax);

        float direction = movingDown ? -1 : 1;
        float movement = currentSpeed * Time.deltaTime * direction;
        transform.Translate(Vector3.up * movement);

        if (movingDown && transform.position.y <= startPos.y - moveDistance)
        {
            isWaiting = true;
            StartCoroutine(SwitchDirectionAfterDelay(false));
        }
        else if (!movingDown && transform.position.y >= startPos.y)
        {
            isWaiting = true;
            StartCoroutine(SwitchDirectionAfterDelay(true));
        }
    }

    IEnumerator SwitchDirectionAfterDelay(bool toDown)
    {
        // Trigger animasi dulu, baru delay
        if (toDown)
            animator.SetTrigger("Fall");
        else
            animator.SetTrigger("Return");

        yield return new WaitForSeconds(waitTime); // delay 0.5s seperti biasa

        // Reset posisi agar selalu presisi
        transform.position = toDown ? startPos : startPos - Vector3.up * moveDistance;

        movingDown = toDown;
        currentSpeed = speedMin;
        isWaiting = false;
    }
}
