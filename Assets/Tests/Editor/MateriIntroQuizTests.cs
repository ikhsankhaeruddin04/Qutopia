using NUnit.Framework;
using UnityEngine;

public class MateriIntroTests
{
    [Test]
    public void YesButton_Sets_AdaptiveMode()
    {
        PlayerPrefs.DeleteAll();
        var mm = new GameObject().AddComponent<MockMateriManager>();
        mm.ConfirmYes();

        Assert.AreEqual(0.3f, PlayerPrefs.GetFloat("LastMastery"));
        Assert.AreEqual("Adaptive", PlayerPrefs.GetString("Stage1Difficulty"));
        Assert.AreEqual("IntroQuiz", mm.loadedScene);
    }

    [Test]
    public void NoButton_Sets_EasyMode()
    {
        PlayerPrefs.DeleteAll();
        var mm = new GameObject().AddComponent<MockMateriManager>();
        mm.ConfirmNo();

        Assert.AreEqual(0.3f, PlayerPrefs.GetFloat("LastMastery"));
        Assert.AreEqual("Easy", PlayerPrefs.GetString("Stage1Difficulty"));
        Assert.AreEqual("Adv1", mm.loadedScene);
    }

    [Test]
    public void DetermineDifficulty_Correctly_Assigns_Level()
    {
        var intro = new GameObject().AddComponent<MockIntroQuizManager>();

        intro.SetMastery(0.35f);
        intro.DetermineDifficulty();
        Assert.AreEqual("Easy", PlayerPrefs.GetString("Stage1Difficulty"));

        intro.SetMastery(0.65f);
        intro.DetermineDifficulty();
        Assert.AreEqual("Medium", PlayerPrefs.GetString("Stage1Difficulty"));

        intro.SetMastery(0.75f);
        intro.DetermineDifficulty();
        Assert.AreEqual("Hard", PlayerPrefs.GetString("Stage1Difficulty"));
    }
}

public class MockMateriManager : MonoBehaviour
{
    public string loadedScene;

    public void ConfirmYes()
    {
        PlayerPrefs.SetFloat("LastMastery", 0.3f);
        PlayerPrefs.SetString("Stage1Difficulty", "Adaptive");
        PlayerPrefs.Save();
        loadedScene = "IntroQuiz";
    }

    public void ConfirmNo()
    {
        PlayerPrefs.SetFloat("LastMastery", 0.3f);
        PlayerPrefs.SetString("Stage1Difficulty", "Easy");
        PlayerPrefs.Save();
        loadedScene = "Adv1";
    }
}

public class MockIntroQuizManager : MonoBehaviour
{
    private float pKnown;
    public int stageNumber = 1;

    public void SetMastery(float value)
    {
        pKnown = value;
    }

    public void DetermineDifficulty()
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
    }
}
