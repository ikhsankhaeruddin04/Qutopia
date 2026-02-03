using UnityEngine;

public class NinjaFrog : MonoBehaviour
{
    [SerializeField] private Transform leftWaypoint;
    [SerializeField] private Transform rightWaypoint;
    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private bool movingLeft = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (movingLeft)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);

            // Hadap kiri
            transform.localScale = new Vector3(1, 1, 1);

            if (transform.position.x <= leftWaypoint.position.x)
            {
                movingLeft = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);

            // Hadap kanan
            transform.localScale = new Vector3(-1, 1, 1);

            if (transform.position.x >= rightWaypoint.position.x)
            {
                movingLeft = true;
            }
        }
    }
}
