using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoResume : MonoBehaviour
{
    void Start()
    {
        string nis = PlayerPrefs.GetString("NIS", "");
        if (string.IsNullOrEmpty(nis)) return;

        string lastScene = PlayerPrefs.GetString($"LastScene_{nis}", "");

        // hanya resume adv, bukan intro dan bukan materi
        if (!string.IsNullOrEmpty(lastScene) && lastScene.StartsWith("Adv"))
        {
            SceneManager.LoadScene(lastScene);
        }
    }
}
