using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

/// <summary>
/// Helper untuk mengirim mastery, skor, dan progres stage
/// ke server via endpoint submit-result.
/// </summary>
public class SubmitResult : MonoBehaviour
{
    private const string API_URL = "https://mclquiz.my.id/quiz/api.php?action=submit-result";

    /// <summary>
    /// Kirim progres stage ke server.
    /// </summary>
    /// <param name="stage">Stage yang sedang diselesaikan</param>
    /// <param name="mastery">Mastery BKT</param>
    /// <param name="score">Skor stage</param>
    /// <param name="response">Response BKT (boleh string/json/array)</param>
    public IEnumerator Send(int stage, float mastery, int score, object response)
    {
        string name = PlayerPrefs.GetString("PlayerName", "Guest");
        string nis = PlayerPrefs.GetString("NIS", "");

        var payload = new
        {
            playerName = name,
            nis = nis,
            stage = stage,
            initial_mastery = mastery,
            score = score,
            response = response
        };

        string json = JsonConvert.SerializeObject(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(API_URL, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"[SubmitResult] SEND → stage={stage}, mastery={mastery}, score={score}, nis={nis}, player={name}");


        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            
            Debug.LogError("[SubmitResult] ERROR: " + request.error);
        }
        else
        {
            Debug.Log("[SubmitResult] PROGRESS TERKIRIM");
        }
    }
}
