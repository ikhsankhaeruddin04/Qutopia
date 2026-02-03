// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class NPCTrigger : MonoBehaviour
// {
//      public string targetSceneName = "adv6"; // Ganti dengan nama scene stage 2 kamu

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             SceneManager.LoadScene(targetSceneName);
//         }
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;  // ← WAJIB ADA

public class NPCTrigger : MonoBehaviour
{
    public int advNumber = 1;
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        string nextQuiz = $"Stage{advNumber}";
        SceneManager.LoadScene(nextQuiz);
    }
}
