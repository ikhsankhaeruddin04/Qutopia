using UnityEngine;
using UnityEngine.UI;

public class PauseButtonManager : MonoBehaviour
{
    public Button pauseButton;
    public GameOverManager gameOverMenuManager;

    void Start()
    {
        pauseButton.onClick.AddListener(OnPauseClicked);
    }

    void OnPauseClicked()
{
    gameOverMenuManager.ShowMenu(); // sama dengan ShowMenu(false)
}

}
