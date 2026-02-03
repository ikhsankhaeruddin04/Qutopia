using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Stage6Manager : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text totalScoreText;
    public TMP_Text[] stageTexts = new TMP_Text[5];
    public UIStarsHandler[] stageStarHandlers;

    public Image badgeImage;
    public TMP_Text badgeLabel;

    public Sprite goldBadge;
    public Sprite silverBadge;
    public Sprite bronzeBadge;

    public Button mainMenuButton;
    public Button nextLearningButton;

    [Header("Audio")]
    public AudioClip victorySound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // ===================================================
        // 🔥 NAMA SISWA: PlayerName → fallback ke PlayerNIS
        // ===================================================
        string name = PlayerPrefs.GetString("PlayerName", "");
        if (string.IsNullOrEmpty(name))
        {
            name = PlayerPrefs.GetString("PlayerNIS", "Guest");
        }
        playerNameText.text = $": {name}";

        // ===================================================
        // 🔥 TOTAL SCORE
        // ===================================================
        int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        totalScoreText.text = $"Total Score: {totalScore}";

        // ===================================================
        // 🔥 MASTERY PER STAGE + HITUNG STAR
        // ===================================================
        for (int i = 0; i < 5; i++)
        {
            float mastery = PlayerPrefs.GetFloat($"Stage{i + 1}Mastery", 0f);
            int percent = Mathf.RoundToInt(mastery * 100);
            int point = percent * 2;

            stageTexts[i].text = $"Stage {i + 1}: {point} pts";

            int starCount =
                point >= 170 ? 3 :
                point >= 140 ? 2 :
                point >= 100 ? 1 : 0;

            stageStarHandlers[i].SetStars(starCount);
        }

        // ===================================================
        // 🔥 MAIN AUDIO
        // ===================================================
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        // ===================================================
        // 🔥 BADGE
        // ===================================================
        ShowBadge(totalScore);

        // ===================================================
        // 🔥 BUTTON EVENTS
        // ===================================================
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        nextLearningButton.onClick.AddListener(() => SceneManager.LoadScene("NextLearningScene"));
    }

    void ShowBadge(int totalScore)
    {
        if (totalScore > 400)
        {
            badgeImage.sprite = goldBadge;
            badgeLabel.text = "Vocabulary Master!";
        }
        else if (totalScore >= 300)
        {
            badgeImage.sprite = silverBadge;
            badgeLabel.text = "Fast Learner!";
        }
        else
        {
            badgeImage.sprite = bronzeBadge;
            badgeLabel.text = "Perfect Score!";
        }

        badgeImage.enabled = true;
        badgeLabel.enabled = true;
    }
}
