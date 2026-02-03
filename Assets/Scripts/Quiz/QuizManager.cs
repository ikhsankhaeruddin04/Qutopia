using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StageManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public TMP_Text[] optionTexts;
    public Button[] optionButtons;
    public TMP_Text timerText;
    public TMP_Text progressText;
    public TMP_Text masteryText;
    public TMP_Text scoreText; // 🆕 Tambahan UI skor

    [Header("Heart System")]
    public TMP_Text heartText;
    public GameObject gameOverPanel;

    [Header("Stage Settings")]
    public int stageNumber = 1;

    [Header("Feedback Panel")]
    public GameObject successPanel;
    public GameObject retryPanel;
    public TMP_Text feedbackText;
    public Button nextButton;
    public Button retryButton;

    [Header("Music")]
    public AudioSource bgmSource;
    public AudioSource countdownSource;
    private bool countdownStarted = false;
    public AudioSource sfxSource;
    public AudioClip correctSFX;
    public AudioClip wrongSFX;

    [Header("Answer Button Sprites")]
    public Sprite defaultSprite;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    private List<QuestionData> questions = new();
    private List<SimpleQuestion> wrongQuestions = new();
    private int currentQuestionIndex = 0;
    private int score = 0;
    private float timeRemaining = 120f;
    private bool isAnswered = false;
    private int lives = 3;
    private float startTime;

    private BKTModel bktModel = new();
    private float pKnown = 0.3f;
    private string currentDifficulty = "Easy";

    private SubmitResult submitResult;


    void Start()
    {
        gameOverPanel.SetActive(false);
        successPanel.SetActive(false);
        retryPanel.SetActive(false);
        UpdateHeartsUI();
        UpdateScoreUI(); // 🆕 Inisialisasi skor UI
        submitResult = FindObjectOfType<SubmitResult>();


        startTime = Time.time;
        pKnown = PlayerPrefs.GetFloat("LastMastery", 0.3f);
        currentDifficulty = PlayerPrefs.GetString($"Stage{stageNumber}Difficulty", "Easy");

        StartCoroutine(FetchQuestionsFromAPI(currentDifficulty, stageNumber));

        if (bgmSource != null)
        {
            bgmSource.volume = 0f;
            StartCoroutine(FadeInMusic(bgmSource, AudioListener.volume, 2f));
        }
    }

    void Update()
    {
        if (!isAnswered)
        {
            timeRemaining -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (!countdownStarted && timeRemaining <= 10f)
            {
                countdownStarted = true;
                if (bgmSource != null) bgmSource.Stop();
                if (countdownSource != null)
                {
                    countdownSource.volume = AudioListener.volume;
                    countdownSource.Play();
                }
            }

            if (timeRemaining <= 0)
            {
                Answer(-1);
            }
        }
    }

    IEnumerator FetchQuestionsFromAPI(string difficulty, int stage)
    {
        string url = $"https://mclquiz.my.id/quiz/api.php?action=questions&stage={stage}&difficulty={difficulty}";
        using var www = UnityEngine.Networking.UnityWebRequest.Get(url);
        www.timeout = 10;
        yield return www.SendWebRequest();

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gagal mengambil soal: " + www.error);
        }
        else
        {
            string json = www.downloadHandler.text;
            questions = JsonConvert.DeserializeObject<List<QuestionData>>(json);
            if (questions == null || questions.Count == 0)
            {
                Debug.LogWarning("Soal tidak ditemukan.");
                yield break;
            }

            currentQuestionIndex = 0;
            LoadQuestion();
        }
    }

    void LoadQuestion()
    {
        isAnswered = false;
        var q = questions[currentQuestionIndex];

        questionText.text = q.question;
        progressText.text = $"{currentQuestionIndex + 1} / {questions.Count}";

        optionTexts[0].text = q.optionA;
        optionTexts[1].text = q.optionB;
        optionTexts[2].text = q.optionC;
        optionTexts[3].text = q.optionD;

        ResetOptionButtonSprites();
        SetOptionButtonsInteractable(true);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => Answer(index));
        }
    }

    void Answer(int selectedIndex)
    {
        if (isAnswered) return;
        isAnswered = true;
        SetOptionButtonsInteractable(false);
        var q = questions[currentQuestionIndex];

        bool correct = (selectedIndex == q.correctAnswerIndex);

        if (sfxSource != null)
        {
            if (correct && correctSFX != null)
                sfxSource.PlayOneShot(correctSFX);
            else if (!correct && wrongSFX != null)
                sfxSource.PlayOneShot(wrongSFX);
        }

        if (correct)
        {
            SetButtonSprite(optionButtons[selectedIndex], correctSprite);
            score++;
            UpdateScoreUI(); // 🆕 Update UI skor setelah benar
        }
        else
        {
            SetButtonSprite(optionButtons[selectedIndex], wrongSprite);
            SetButtonSprite(optionButtons[q.correctAnswerIndex], correctSprite);

            lives--;
            UpdateHeartsUI();

            if (lives <= 0)
            {
                GameOver();
                return;
            }

            wrongQuestions.Add(new SimpleQuestion
            {
                question = q.question,
                optionA = q.optionA,
                optionB = q.optionB,
                optionC = q.optionC,
                optionD = q.optionD,
                correctAnswerIndex = q.correctAnswerIndex,
                difficulty = q.difficulty,
                stage = q.stage
            });
        }

        pKnown = bktModel.UpdateProbability(correct);
        if (masteryText != null)
            masteryText.text = $"Mastery: {Mathf.RoundToInt(pKnown * 100)}%";

        StartCoroutine(WaitAndLoadNextQuestion());
    }

    IEnumerator WaitAndLoadNextQuestion()
    {
        yield return new WaitForSeconds(0.5f);

        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            LoadQuestion();
        }
        else
        {
            if (pKnown >= 0.75f)
                ShowSuccessPanel();
            else
                ShowRetryPanel();
        }
    }

    void SetButtonSprite(Button button, Sprite sprite)
    {
        var img = button.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = sprite;
        }
    }

    void ResetOptionButtonSprites()
    {
        foreach (var btn in optionButtons)
        {
            SetButtonSprite(btn, defaultSprite);
        }
    }

    void ShowSuccessPanel()
    {
        successPanel.SetActive(true);
        feedbackText.text = $"Hebat! Kamu sudah menguasai level {currentDifficulty}.";
        SetOptionButtonsInteractable(false);

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            if (currentDifficulty == "Easy")
            {
                PlayerPrefs.SetString($"Stage{stageNumber}Difficulty", "Medium");
                PlayerPrefs.SetFloat("LastMastery", 0.3f);
                pKnown = 0.3f;
                currentDifficulty = "Medium";
                lives = 3;
                UpdateHeartsUI();
                StartCoroutine(FetchQuestionsFromAPI(currentDifficulty, stageNumber));
                successPanel.SetActive(false);
            }
            else if (currentDifficulty == "Medium")
            {
                PlayerPrefs.SetString($"Stage{stageNumber}Difficulty", "Hard");
                PlayerPrefs.SetFloat("LastMastery", 0.3f);
                pKnown = 0.3f;
                currentDifficulty = "Hard";
                lives = 3;
                UpdateHeartsUI();
                StartCoroutine(FetchQuestionsFromAPI(currentDifficulty, stageNumber));
                successPanel.SetActive(false);
            }
            else
            {
                EndStage();
            }
        });
    }

    void ShowRetryPanel()
    {
        retryPanel.SetActive(true);
        feedbackText.text = $"Kamu belum mencapai KKM (75%) untuk level {currentDifficulty}.\nAyo coba lagi!";
        SetOptionButtonsInteractable(false);

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetFloat("LastMastery", pKnown);
            SceneManager.LoadScene($"Adv{stageNumber}");
        });
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        SetOptionButtonsInteractable(false);
        timerText.text = "Waktu: 0";

        if (bgmSource != null) bgmSource.Stop();
        if (countdownSource != null) countdownSource.Stop();
    }

//     void EndStage()
// {
//     float duration = Time.time - startTime;

//     PlayerPrefs.SetFloat("LastMastery", pKnown);
//     PlayerPrefs.SetFloat($"Stage{stageNumber}Mastery", pKnown);

//     PlayerPrefs.SetInt("TotalScore", PlayerPrefs.GetInt("TotalScore", 0) + score * 20);

//     // Progress per-stage
//     PlayerPrefs.SetInt($"Stage{stageNumber}_Completed", 1);

//     // Progress utama
//     PlayerPrefs.SetInt("StageCompleted", stageNumber);

//     PlayerPrefs.Save();

//     StartCoroutine(submitResult.Send(stageNumber, pKnown, score * 20, new string[] { }));

//     int nextStage = stageNumber + 1;
//     if (nextStage <= 5)
//         SceneManager.LoadScene($"Materi{nextStage}");
//     else
//         SceneManager.LoadScene("SummaryStage6");
// }
public void EndStage()
{
    float duration = Time.time - startTime;

    PlayerPrefs.SetFloat("LastMastery", pKnown);
    PlayerPrefs.SetFloat($"Stage{stageNumber}Mastery", pKnown);

    PlayerPrefs.SetInt("TotalScore", 
        PlayerPrefs.GetInt("TotalScore", 0) + score * 20);

    PlayerPrefs.SetInt($"Stage{stageNumber}_Completed", 1);

    int nextStage = stageNumber + 1;
    PlayerPrefs.SetInt("StageCompleted", nextStage);

    PlayerPrefs.Save();

    StartCoroutine(submitResult.Send(stageNumber, pKnown, score * 20, new string[] {}));

    if (nextStage <= 5)
        SceneManager.LoadScene($"Materi{nextStage}");
    else
        SceneManager.LoadScene("SummaryStage6");
}







    void UpdateHeartsUI()
    {
        heartText.text = $"{lives}/3";
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $" {score * 20}";
    }

    void SetOptionButtonsInteractable(bool enabled)
    {
        foreach (var btn in optionButtons)
            btn.interactable = enabled;
    }

    public void RetryStage() => SceneManager.LoadScene($"Adv{stageNumber}");
    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");

    IEnumerator FadeInMusic(AudioSource source, float targetVolume, float duration)
    {
        if (source == null) yield break;

        source.Play();
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, t / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }
}
