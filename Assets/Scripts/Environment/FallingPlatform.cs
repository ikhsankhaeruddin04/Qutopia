using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    public float fallDelay = 0.5f;
    public float destroyDelay = 3f; // Jika ingin dihancurkan setelah jatuh

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool hasFallen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasFallen && collision.gameObject.CompareTag("Player"))
        {
            hasFallen = true;
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Platform akan hancur dalam 3 detik (opsional)
        Destroy(gameObject, destroyDelay);
    }

    // Jika ingin platform respawn (opsional)
    public void ResetPlatform()
    {
        hasFallen = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
