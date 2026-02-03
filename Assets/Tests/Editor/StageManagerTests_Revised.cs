using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class StageManagerTests
{
    private BKTModel bkt;

    [SetUp]
    public void Setup()
    {
        bkt = new BKTModel();
    }

    [Test]
    public void UpdateProbability_Increase_WhenCorrect()
    {
        float initial = bkt.pKnown;
        float updated = bkt.UpdateProbability(true);
        Assert.Greater(updated, initial, "Probabilitas seharusnya naik jika jawaban benar.");
    }

    [Test]
    public void UpdateProbability_Decrease_WhenWrong()
    {
        bkt.pKnown = 0.9f;
        float updated = bkt.UpdateProbability(false);
        Assert.Less(updated, 0.9f, "Probabilitas seharusnya turun jika jawaban salah.");
    }

    [Test]
    public void Answer_DecreasesLives_WhenWrongAnswer()
    {
        var obj = new GameObject();
        var sm = obj.AddComponent<MockStageManager>();
        sm.lives = 5;
        sm.questions = new List<QuestionData>
        {
            new QuestionData { correctAnswerIndex = 0, optionA = "A", optionB = "B", optionC = "C", optionD = "D", question = "Q" }
        };
        sm.currentQuestionIndex = 0;

        sm.Answer(1); // salah
        Assert.AreEqual(4, sm.lives);
    }

    [Test]
    public void Answer_AddsScore_WhenCorrectAnswer()
    {
        var obj = new GameObject();
        var sm = obj.AddComponent<MockStageManager>();
        sm.score = 0;
        sm.questions = new List<QuestionData>
        {
            new QuestionData { correctAnswerIndex = 0, optionA = "A", optionB = "B", optionC = "C", optionD = "D", question = "Q" }
        };
        sm.currentQuestionIndex = 0;

        sm.Answer(0); // benar
        Assert.AreEqual(1, sm.score);
    }

    [Test]
    public void Triggers_GameOver_WhenLivesReachZero()
    {
        var obj = new GameObject();
        var sm = obj.AddComponent<MockStageManagerExtended>();
        sm.lives = 1;
        sm.questions = new List<QuestionData>
        {
            new QuestionData { correctAnswerIndex = 0, optionA = "A", optionB = "B", optionC = "C", optionD = "D", question = "Q" }
        };
        sm.currentQuestionIndex = 0;

        sm.Answer(1); // salah
        Assert.IsTrue(sm.gameOverCalled, "GameOver() seharusnya dipanggil saat lives habis.");
    }

    [Test]
    public void Triggers_Success_WhenMasteryIsAbove75()
    {
        var obj = new GameObject();
        var sm = obj.AddComponent<MockStageManagerExtended>();
        sm.pKnown = 0.76f;
        sm.questions = new List<QuestionData>
        {
            new QuestionData { correctAnswerIndex = 0, optionA = "A", optionB = "B", optionC = "C", optionD = "D", question = "Q" }
        };
        sm.currentQuestionIndex = 0;
        sm.LoadQuestion();  // akan load 1 soal
        sm.Answer(0);       // jawaban benar

        Assert.IsTrue(sm.successPanelShown, "ShowSuccessPanel() seharusnya dipanggil jika pKnown >= 0.75 di akhir soal.");
    }
}

// Extended mock class dengan flags untuk pengujian efek
public class MockStageManagerExtended : MonoBehaviour
{
    public int lives = 5;
    public int score = 0;
    public int currentQuestionIndex = 0;
    public List<QuestionData> questions = new();
    public BKTModel bktModel = new();
    public float pKnown = 0.3f;
    public bool gameOverCalled = false;
    public bool successPanelShown = false;

    public void LoadQuestion() { /* hanya mock trigger */ }

    public void Answer(int selectedIndex)
    {
        var q = questions[currentQuestionIndex];
        bool correct = (selectedIndex == q.correctAnswerIndex);
        if (correct) score++;
        else
        {
            lives--;
            if (lives <= 0)
            {
                GameOver();
                return;
            }
        }

        pKnown = bktModel.UpdateProbability(correct);
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Count)
        {
            if (pKnown >= 0.75f)
                ShowSuccessPanel();
        }
    }

    public void GameOver() => gameOverCalled = true;
    public void ShowSuccessPanel() => successPanelShown = true;
}

// Mock StageManager untuk testing logika tanpa UI
public class MockStageManager : MonoBehaviour
{
    public int lives = 5;
    public int score = 0;
    public int currentQuestionIndex = 0;
    public List<QuestionData> questions = new();
    public BKTModel bktModel = new();
    public float pKnown = 0.3f;

    public void Answer(int selectedIndex)
    {
        var q = questions[currentQuestionIndex];
        bool correct = (selectedIndex == q.correctAnswerIndex);
        if (correct) score++;
        else lives--;

        pKnown = bktModel.UpdateProbability(correct);
    }
}