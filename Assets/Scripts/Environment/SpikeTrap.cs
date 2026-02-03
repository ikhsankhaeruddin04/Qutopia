using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private bool movingUp = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (movingUp)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            if (transform.position.y >= startPos.y + moveDistance)
                movingUp = false;
        }
        else
        {
            transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
            if (transform.position.y <= startPos.y)
                movingUp = true;
        }
    }
}