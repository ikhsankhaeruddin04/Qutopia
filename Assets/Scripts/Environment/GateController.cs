using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private float moveUpDistance = 2f;
    [SerializeField] private float moveDuration = 0.5f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveUpDistance;
    }

    public void OpenGate()
    {
        Debug.Log("OpenGate dipanggil!");
        StartCoroutine(MoveUp());
    }

    private IEnumerator MoveUp()
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // Nonaktifkan collider supaya player bisa lewat
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }
}
