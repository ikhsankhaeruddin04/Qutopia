using UnityEngine;
using UnityEngine.SceneManagement;

public static class ProgressManager
{
    private const string LAST_SCENE_KEY = "LastScene";
    private const string INTRO_DONE_KEY = "IntroDone";
    private const string STAGE_KEY = "StageProgress";

    // ============================================================
    //  SAVE / LOAD SCENE TERAKHIR
    // ============================================================
    public static void SaveScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(LAST_SCENE_KEY, currentScene);
        PlayerPrefs.Save();
    }

    public static string LoadLastScene()
    {
        return PlayerPrefs.GetString(LAST_SCENE_KEY, "MainMenu");
    }

    // ============================================================
    //  STATUS INTRO
    // ============================================================
    public static void SetIntroDone()
    {
        PlayerPrefs.SetInt(INTRO_DONE_KEY, 1);
        PlayerPrefs.Save();
    }

    public static bool IsIntroDone()
    {
        return PlayerPrefs.GetInt(INTRO_DONE_KEY, 0) == 1;
    }

    // ============================================================
    //  STAGE PROGRESS
    // ============================================================
    public static void SetStage(int stage)
    {
        PlayerPrefs.SetInt(STAGE_KEY, stage);
        PlayerPrefs.Save();
    }

    public static int GetStage()
    {
        return PlayerPrefs.GetInt(STAGE_KEY, 0);
    }
}
