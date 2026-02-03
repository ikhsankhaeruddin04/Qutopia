using UnityEngine;

public class ButtonClickAnim : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        animator.Play("ButtonClick", 0, 0f);
    }
}
