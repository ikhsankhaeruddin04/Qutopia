// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class SaveLastScene : MonoBehaviour
// {
//     void Start()
//     {
//         string currentNIS = PlayerPrefs.GetString("NIS", "");
//         if (string.IsNullOrEmpty(currentNIS)) return;

//         string sceneName = SceneManager.GetActiveScene().name;

//         // Jangan simpan LastScene untuk IntrpAdv
//         if (sceneName != "IntrpAdv")
//         {
//             PlayerPrefs.SetString($"LastScene_{currentNIS}", sceneName);
//             PlayerPrefs.Save();
//         }
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLastScene : MonoBehaviour
{
    void Start()
    {
        string nis = PlayerPrefs.GetString("NIS", "");
        if (string.IsNullOrEmpty(nis)) return;

        string scene = SceneManager.GetActiveScene().name;

        // Simpan hanya adv1 - adv5
        if (scene.StartsWith("Adv"))
        {
            PlayerPrefs.SetString($"LastScene_{nis}", scene);
            PlayerPrefs.Save();
        }
    }
}
