using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class AudioButton : MonoBehaviour
{
    [Header("Drag audio clip ke sini")]
    public AudioClip audioClip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
{
    audioSource = gameObject.AddComponent<AudioSource>();
}
        audioSource.playOnAwake = false;

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        else
        {
            Debug.LogError("AudioClip belum diisi di Inspector!");
        }

        // Hubungkan klik button ke PlaySound
        GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
