using UnityEngine;

public class Rotator : MonoBehaviour
{
    public enum Mode { Rotate, Swing }
    public Mode mode = Mode.Rotate;

    [Header("Common Settings")]
    public float speed = 50f;
    public bool clockwise = true;

    [Header("Swing Settings")]
    public float swingAngle = 45f; // Maksimal sudut berayun
    private float angle;
    private float swingDirection = 1f;

    void Update()
    {
        if (mode == Mode.Rotate)
        {
            RotateObject();
        }
        else if (mode == Mode.Swing)
        {
            SwingObject();
        }
    }

    void RotateObject()
    {
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, speed * direction * Time.deltaTime);
    }

    void SwingObject()
    {
        angle += swingDirection * speed * Time.deltaTime;

        // Batasi sudut agar tidak lebih dari swingAngle
        if (angle > swingAngle)
        {
            angle = swingAngle;
            swingDirection *= -1f;
        }
        else if (angle < -swingAngle)
        {
            angle = -swingAngle;
            swingDirection *= -1f;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
