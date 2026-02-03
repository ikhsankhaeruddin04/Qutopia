
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class PlayerLoginManager : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public Button loginButton;
    public GameObject loginPanel;

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);

        string savedNis = PlayerPrefs.GetString("NIS", "");
        loginPanel.SetActive(string.IsNullOrEmpty(savedNis));
    }

    void OnLoginClicked()
    {
        string nis = playerNameInput.text.Trim();
        if (string.IsNullOrEmpty(nis))
        {
            Debug.LogWarning("NIS tidak boleh kosong!");
            return;
        }
        StartCoroutine(FetchPlayerData(nis));
    }

    IEnumerator FetchPlayerData(string nis)
    {
        string url = "https://mclquiz.my.id/quiz/api.php?action=get-player-data" +
                     "&nis=" + UnityWebRequest.EscapeURL(nis) +
                     "&playerName=" + UnityWebRequest.EscapeURL(nis);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gagal menghubungi server: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        if (string.IsNullOrEmpty(json) || json == "null")
        {
            Debug.LogError("Data kosong dari server!");
            yield break;
        }

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        if (data == null) yield break;

        // ============================
        // SIMPAN DATA PLAYER
        // ============================
        PlayerPrefs.SetString("NIS", data.nis);
        PlayerPrefs.SetString("PlayerName", data.player_name);
        PlayerPrefs.SetInt("TotalScore", data.total_score);
        PlayerPrefs.SetInt("StageCompleted", data.stage_completed);
        PlayerPrefs.SetString("Achievement", data.achievement);

        PlayerPrefs.SetFloat("Stage1Mastery", data.stage1_mastery);
        PlayerPrefs.SetFloat("Stage2Mastery", data.stage2_mastery);
        PlayerPrefs.SetFloat("Stage3Mastery", data.stage3_mastery);
        PlayerPrefs.SetFloat("Stage4Mastery", data.stage4_mastery);
        PlayerPrefs.SetFloat("Stage5Mastery", data.stage5_mastery);

        // ============================
        // FIX RESTORE LAST SCENE
        // ============================

        string savedScene = PlayerPrefs.GetString($"LastScene_{data.nis}", "");

        if (string.IsNullOrEmpty(savedScene))
        {
            savedScene = "IntrpAdv";   // default
        }

        PlayerPrefs.SetString($"LastScene_{data.nis}", savedScene);
        PlayerPrefs.Save();

        Debug.Log("RESTORED LAST SCENE = " + savedScene);

        // Masuk ke Main Menu
        SceneManager.LoadScene("MainMenu");
    }

    [System.Serializable]
    public class PlayerData
    {
        public string nis;
        public string player_name;
        public int total_score;
        public int stage_completed;
        public float stage1_mastery;
        public float stage2_mastery;
        public float stage3_mastery;
        public float stage4_mastery;
        public float stage5_mastery;
        public string achievement;
    }
}
