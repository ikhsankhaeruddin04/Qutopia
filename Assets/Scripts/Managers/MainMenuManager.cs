// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class MainMenuManager : MonoBehaviour
// {
//     public Button stage6Button;
//     public GameObject loginPanel;
//     public Button closeLoginButton;
//     public Button logoutButton;
//     public GameObject petunjukPanel;
//     public GameObject infoPanel;

//     // UI tambahan
//     public GameObject playButton;
//     public GameObject loginButton;

//     // Sound Panel dan Slider Volume
//     public GameObject soundPanel;
//     public Slider volumeSlider;
//     private bool isSoundOn = true;
//     public AudioSource bgmSource;

//     // Exit confirmation
//     public GameObject exitConfirmPanel;
//     public Button yesExitButton;
//     public Button noExitButton;

//     void Start()
//     {
//         // ===== Cek stage 1-5 =====
//         bool allStagesCompleted = true;
//         for (int i = 1; i <= 5; i++)
//         {
//             if (PlayerPrefs.GetInt($"Stage{i}_Completed", 0) == 0)
//             {
//                 allStagesCompleted = false;
//                 break;
//             }
//         }
//         if (stage6Button != null)
//             stage6Button.interactable = allStagesCompleted;

//         if (logoutButton != null) logoutButton.onClick.AddListener(LogoutPlayer);
//         if (closeLoginButton != null) closeLoginButton.onClick.AddListener(CloseLoginPanel);

//         // Set panel awal
//         if (loginPanel != null) loginPanel.SetActive(false);
//         if (petunjukPanel != null) petunjukPanel.SetActive(false);
//         if (infoPanel != null) infoPanel.SetActive(false);
//         if (soundPanel != null) soundPanel.SetActive(false);
//         if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

//         if (yesExitButton != null) yesExitButton.onClick.AddListener(YesQuit);
//         if (noExitButton != null) noExitButton.onClick.AddListener(NoQuit);

//         // Load volume
//         if (volumeSlider != null)
//         {
//             float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
//             volumeSlider.value = savedVolume;
//             AudioListener.volume = savedVolume;
//             volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
//         }

//         // Fade in music sekali
//         if (bgmSource != null)
//         {
//             float initialVolume = volumeSlider != null ? volumeSlider.value : 1f;
//             if (PlayerPrefs.GetInt("HasFadedIn", 0) == 0)
//             {
//                 bgmSource.volume = 0f;
//                 StartCoroutine(FadeInMusic(initialVolume, 2f));
//                 PlayerPrefs.SetInt("HasFadedIn", 1);
//                 PlayerPrefs.Save();
//             }
//             else
//             {
//                 bgmSource.volume = initialVolume;
//                 bgmSource.Play();
//             }
//         }

//         UpdateLoginUI();
//     }

//     void UpdateLoginUI()
//     {
//         string savedName = PlayerPrefs.GetString("PlayerName", "");
//         bool isLoggedIn = !string.IsNullOrEmpty(savedName);

//         if (playButton != null) playButton.SetActive(isLoggedIn);
//         if (loginButton != null) loginButton.SetActive(!isLoggedIn);
//         if (logoutButton != null) logoutButton.gameObject.SetActive(isLoggedIn);
//     }

// public void PlayGame()
// {
//     string nis = PlayerPrefs.GetString("NIS", "");
//     string lastScene = PlayerPrefs.GetString($"LastScene_{nis}", "");
//     bool watched = PlayerPrefs.GetInt($"IntrpAdvWatched_{nis}", 0) == 1;

//     if (!watched)
//     {
//         PlayerPrefs.SetInt($"IntrpAdvWatched_{nis}", 1);
//         PlayerPrefs.Save();
//         SceneManager.LoadScene("IntrpAdv");
//         return;
//     }

//     if (!string.IsNullOrEmpty(lastScene))
//     {
//         SceneManager.LoadScene(lastScene);
//         return;
//     }

//     SceneManager.LoadScene("Adv1");
// }




//     // ================= LOGIN PANEL =================
//     public void ShowLoginPanel() { if (loginPanel != null) loginPanel.SetActive(true); }
//     public void CloseLoginPanel() { if (loginPanel != null) loginPanel.SetActive(false); }

//     // ================= LOGOUT =================
//     public void LogoutPlayer()
//     {
//         string nis = PlayerPrefs.GetString("NIS", "");
//         PlayerPrefs.DeleteKey("PlayerName");
//         PlayerPrefs.DeleteKey("NIS");
//         PlayerPrefs.DeleteKey($"LastScene_{nis}");
//         PlayerPrefs.DeleteKey("StageCompleted");

//         for (int i = 1; i <= 5; i++)
//             PlayerPrefs.DeleteKey($"Stage{i}_Completed");

//         PlayerPrefs.Save();
//         UpdateLoginUI();

//         if (loginPanel != null) loginPanel.SetActive(true);
//     }

//     // ================= EXIT =================
//     public void QuitGame() { if (exitConfirmPanel != null) exitConfirmPanel.SetActive(true); }
//     public void YesQuit() { Application.Quit(); }
//     public void NoQuit() { if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false); }

//     // ================= PANELS =================
//     public void ShowPetunjuk() { if (petunjukPanel != null) petunjukPanel.SetActive(true); }
//     public void ClosePetunjuk() { if (petunjukPanel != null) petunjukPanel.SetActive(false); }
//     public void ShowInfo() { if (infoPanel != null) infoPanel.SetActive(true); }
//     public void CloseInfo() { if (infoPanel != null) infoPanel.SetActive(false); }
//     public void CloseSoundPanel() { if (soundPanel != null) soundPanel.SetActive(false); }
//     public void ToggleSoundPanel() { if (soundPanel != null) soundPanel.SetActive(!soundPanel.activeSelf); }

//     // ================= SOUND =================
//     public void OnVolumeChanged(float volume)
//     {
//         AudioListener.volume = volume;
//         PlayerPrefs.SetFloat("GameVolume", volume);
//         PlayerPrefs.Save();
//         isSoundOn = volume > 0f;
//         if (bgmSource != null) bgmSource.volume = volume;
//     }

//     public void ToggleSound()
//     {
//         isSoundOn = !isSoundOn;
//         float newVolume = isSoundOn ? 1f : 0f;
//         AudioListener.volume = newVolume;
//         if (volumeSlider != null) volumeSlider.value = newVolume;
//         if (bgmSource != null) bgmSource.volume = newVolume;
//     }

//     private IEnumerator FadeInMusic(float targetVolume, float duration)
//     {
//         if (bgmSource == null) yield break;
//         bgmSource.Play();
//         float t = 0f;
//         while (t < duration)
//         {
//             t += Time.deltaTime;
//             bgmSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
//             yield return null;
//         }
//         bgmSource.volume = targetVolume;
//     }
// }


using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button stage6Button;
    public GameObject loginPanel;
    public Button closeLoginButton;
    public Button logoutButton;
    public GameObject petunjukPanel;
    public GameObject infoPanel;

    public GameObject playButton;
    public GameObject loginButton;

    public GameObject soundPanel;
    public Slider volumeSlider;
    private bool isSoundOn = true;
    public AudioSource bgmSource;

    public GameObject exitConfirmPanel;
    public Button yesExitButton;
    public Button noExitButton;

    void Start()
    {
        // Cek stage 1–5
        bool allStagesCompleted = true;
        for (int i = 1; i <= 5; i++)
        {
            if (PlayerPrefs.GetInt($"Stage{i}_Completed", 0) == 0)
            {
                allStagesCompleted = false;
                break;
            }
        }
        if (stage6Button != null)
            stage6Button.interactable = allStagesCompleted;

        if (logoutButton != null) logoutButton.onClick.AddListener(LogoutPlayer);
        if (closeLoginButton != null) closeLoginButton.onClick.AddListener(CloseLoginPanel);

        if (loginPanel != null) loginPanel.SetActive(false);
        if (petunjukPanel != null) petunjukPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (soundPanel != null) soundPanel.SetActive(false);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        if (yesExitButton != null) yesExitButton.onClick.AddListener(YesQuit);
        if (noExitButton != null) noExitButton.onClick.AddListener(NoQuit);

        // Volume
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Music fade-in
        if (bgmSource != null)
        {
            float initialVolume = volumeSlider != null ? volumeSlider.value : 1f;
            if (PlayerPrefs.GetInt("HasFadedIn", 0) == 0)
            {
                bgmSource.volume = 0f;
                StartCoroutine(FadeInMusic(initialVolume, 2f));
                PlayerPrefs.SetInt("HasFadedIn", 1);
                PlayerPrefs.Save();
            }
            else
            {
                bgmSource.volume = initialVolume;
                bgmSource.Play();
            }
        }

        UpdateLoginUI();
    }

    void UpdateLoginUI()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        bool isLoggedIn = !string.IsNullOrEmpty(savedName);

        if (playButton != null) playButton.SetActive(isLoggedIn);
        if (loginButton != null) loginButton.SetActive(!isLoggedIn);
        if (logoutButton != null) logoutButton.gameObject.SetActive(isLoggedIn);
    }

    // ================= PLAY GAME =================
    public void PlayGame()
    {
        string nis = PlayerPrefs.GetString("NIS", "");
        string lastScene = PlayerPrefs.GetString($"LastScene_{nis}", "");
        bool watched = PlayerPrefs.GetInt($"IntrpAdvWatched_{nis}", 0) == 1;

        // Belum pernah melihat IntrpAdv → jalankan sekali
        if (!watched)
        {
            PlayerPrefs.SetInt($"IntrpAdvWatched_{nis}", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("IntrpAdv");
            return;
        }

        // Pakai lastScene jika ada
        if (!string.IsNullOrEmpty(lastScene))
        {
            SceneManager.LoadScene(lastScene);
            return;
        }

        // Jika tidak ada lastScene tapi intrp sudah ditonton → mulai dari adv1
        SceneManager.LoadScene("adv1");
    }

    // ================= LOGIN PANEL =================
    public void ShowLoginPanel() { if (loginPanel != null) loginPanel.SetActive(true); }
    public void CloseLoginPanel() { if (loginPanel != null) loginPanel.SetActive(false); }

    public void LogoutPlayer()
{
    string nis = PlayerPrefs.GetString("NIS", "");

    PlayerPrefs.DeleteKey("PlayerName");
    PlayerPrefs.DeleteKey("NIS");

    // ❌ Jangan hapus
    // PlayerPrefs.DeleteKey($"IntrpAdvWatched_{nis}");

    PlayerPrefs.DeleteKey("StageCompleted");

    for (int i = 1; i <= 5; i++)
        PlayerPrefs.DeleteKey($"Stage{i}_Completed");

    PlayerPrefs.Save();
    UpdateLoginUI();

    if (loginPanel != null) loginPanel.SetActive(true);
}



    // ================= EXIT =================
    public void QuitGame() { if (exitConfirmPanel != null) exitConfirmPanel.SetActive(true); }
    public void YesQuit() { Application.Quit(); }
    public void NoQuit() { if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false); }

    // ================= PANELS =================
    public void ShowPetunjuk() { if (petunjukPanel != null) petunjukPanel.SetActive(true); }
    public void ClosePetunjuk() { if (petunjukPanel != null) petunjukPanel.SetActive(false); }
    public void ShowInfo() { if (infoPanel != null) infoPanel.SetActive(true); }
    public void CloseInfo() { if (infoPanel != null) infoPanel.SetActive(false); }
    public void CloseSoundPanel() { if (soundPanel != null) soundPanel.SetActive(false); }
    public void ToggleSoundPanel() { if (soundPanel != null) soundPanel.SetActive(!soundPanel.activeSelf); }

    // ================= SOUND =================
    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
        isSoundOn = volume > 0f;
        if (bgmSource != null) bgmSource.volume = volume;
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        float newVolume = isSoundOn ? 1f : 0f;
        AudioListener.volume = newVolume;
        if (volumeSlider != null) volumeSlider.value = newVolume;
        if (bgmSource != null) bgmSource.volume = newVolume;
    }

    private IEnumerator FadeInMusic(float targetVolume, float duration)
    {
        if (bgmSource == null) yield break;
        bgmSource.Play();
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }
        bgmSource.volume = targetVolume;
    }
}
