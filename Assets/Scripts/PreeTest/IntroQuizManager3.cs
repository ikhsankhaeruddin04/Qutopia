using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class IntroQuizManager3 : MonoBehaviour
{
    [Header("Soal dan Opsi")]
    public TMP_Text questionText;
    public TMP_Text[] optionTexts;
    public Button[] optionButtons;
    public TMP_Text progressText;

    [Header("Panel Hasil Pre-Test")]
    public GameObject masteryPanel;
    public TMP_Text levelText;              // Text untuk Level Soal
    public TMP_Text masteryValueText;       // Text untuk nilai penguasaan
    public Image levelIcon;                 // Icon 🧠
    public Image masteryIcon;               // Icon 📊
    public Sprite brainSprite;              // Sprite otak
    public Sprite statsSprite;              // Sprite statistik

    public int stageNumber = 3;

    private List<QuestionData> questions = new();
    private int currentQuestionIndex = 0;
    private BKTModel bktModel = new();
    private float pKnown = 0.3f;

    void Start()
    {
        if (masteryPanel != null)
            masteryPanel.SetActive(false); // Sembunyikan panel di awal

        StartCoroutine(FetchQuestions());
    }

    IEnumerator FetchQuestions()
    {
        string url = $"https://mclquiz.my.id/quiz/api.php?action=pretest&stage={stageNumber}";

        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gagal ambil soal pretest: " + www.error);
        }
        else
        {
            questions = JsonConvert.DeserializeObject<List<QuestionData>>(www.downloadHandler.text);
            if (questions != null && questions.Count > 0)
            {
                currentQuestionIndex = 0;
                LoadQuestion();
            }
            else
            {
                Debug.LogWarning("Soal pretest kosong.");
            }
        }
    }

    void LoadQuestion()
    {
        var q = questions[currentQuestionIndex];
        questionText.text = q.question;
        progressText.text = $"{currentQuestionIndex + 1} / {questions.Count}";

        optionTexts[0].text = q.optionA;
        optionTexts[1].text = q.optionB;
        optionTexts[2].text = q.optionC;
        optionTexts[3].text = q.optionD;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => Answer(index));
        }
    }

    void Answer(int selectedIndex)
    {
        var q = questions[currentQuestionIndex];
        bool correct = (selectedIndex == q.correctAnswerIndex);
        pKnown = bktModel.UpdateProbability(correct);

        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Count)
        {
            LoadQuestion();
        }
        else
        {
            DetermineDifficulty();
        }
    }

    void DetermineDifficulty()
    {
        string difficulty = pKnown switch
        {
            < 0.4f => "Easy",
            < 0.7f => "Medium",
            _ => "Hard"
        };

        PlayerPrefs.SetFloat("LastMastery", pKnown);
        PlayerPrefs.SetString($"Stage{stageNumber}Difficulty", difficulty);
        PlayerPrefs.Save();

        // ⬇️ Tampilkan UI hasil pre-test dengan ikon
        if (masteryPanel != null && levelText != null && masteryValueText != null)
        {
            masteryPanel.SetActive(true);

            levelText.text = $"Level Soal: {difficulty}";
            masteryValueText.text = $"Penguasaan: {Mathf.RoundToInt(pKnown * 100)}%";

            if (levelIcon != null && brainSprite != null)
                levelIcon.sprite = brainSprite;

            if (masteryIcon != null && statsSprite != null)
                masteryIcon.sprite = statsSprite;
        }

        StartCoroutine(GoToAdaptiveStage());
    }

    IEnumerator GoToAdaptiveStage()
    {
        yield return new WaitForSeconds(1.5f); // Tampilkan hasil sebentar
        SceneManager.LoadScene($"Adv{stageNumber}");
    }
}
