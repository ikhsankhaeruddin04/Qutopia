using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    public int index;

    public GameObject contButton;
    public float wordSpeed = 0.05f;
    public bool playerIsClose;

    public GameObject interactionIcon; // 👈 Tambahkan ini
    public static bool isTalking = false;

    [SerializeField] private string sceneToLoad = "Materi";

    void Start()
{
    if (interactionIcon != null)
        interactionIcon.SetActive(true); // Aktifkan langsung
}


    void Update()
{
    if (Input.GetKeyDown(KeyCode.E) && playerIsClose)
    {
        TriggerDialogue();
    }

    if (dialoguePanel.activeInHierarchy && dialogueText.text == dialogue[index])
    {
        contButton.SetActive(true);
    }

    // Ikon aktif jika belum bicara, nonaktif saat sedang dialog
    if (interactionIcon != null)
        interactionIcon.SetActive(!dialoguePanel.activeInHierarchy);
}


    public void TriggerDialogue()
    {
        if (playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                EndDialogue();
            }
            else
            {
                dialoguePanel.SetActive(true);
                isTalking = true;
                StartCoroutine(Typing());
            }
        }
    }
public void EndDialogue()
{
    if (dialogueText != null)
        dialogueText.text = "";

    index = 0;

    if (dialoguePanel != null)
        dialoguePanel.SetActive(false);

    if (contButton != null)
        contButton.SetActive(false);

    isTalking = false;
}

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            EndDialogue();
            // 🔥 SIMPAN PROGRESS AWAL
        if (PlayerPrefs.GetInt("StageCompleted", 0) == 0)
        {
            PlayerPrefs.SetInt("StageCompleted", 1);
            PlayerPrefs.Save();
        }
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            EndDialogue();
        }
    }
}
