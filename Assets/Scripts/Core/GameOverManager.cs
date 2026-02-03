using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    public Button resumeButton;
    public Button retryButton;
    public Button quitButton;

    private void Start()
    {
        gameOverMenu.SetActive(false);
        resumeButton.onClick.AddListener(ResumeGame);
        retryButton.onClick.AddListener(RetryGame);
        quitButton.onClick.AddListener(QuitToMainMenu);
    }

    // public void ShowMenu()
    // {
    //     gameOverMenu.SetActive(true);
    //     Time.timeScale = 0f;
    // }

    public void ShowMenu(bool isGameOver = false)
{
    gameOverMenu.SetActive(true);
    Time.timeScale = 0f;

    if (isGameOver)
    {
        resumeButton.interactable = false;           // Disable klik
        resumeButton.gameObject.SetActive(false);    // Optional: sembunyikan tombol
    }
    else
    {
        resumeButton.interactable = true;
        resumeButton.gameObject.SetActive(true);
    }
}


    public void ResumeGame()
    {
        gameOverMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Ganti nama scene jika berbeda
    }
}
