using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ApiService : MonoBehaviour
{
    private string baseUrl = "https://mclquiz.my.id/quiz/api.php";  // Ganti dengan URL API kamu

    public IEnumerator GetQuestions(int stage, string difficulty, System.Action<List<QuestionData>> onSuccess)
    {
        string url = $"{baseUrl}?action=questions&stage={stage}&difficulty={difficulty}";

        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("GetQuestions Error: " + www.error);
            onSuccess?.Invoke(null);
        }
        else
        {
            try
            {
                var questions = JsonConvert.DeserializeObject<List<QuestionData>>(www.downloadHandler.text);
                onSuccess?.Invoke(questions);
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSON Parse Error: " + e.Message);
                onSuccess?.Invoke(null);
            }
        }
    }

    public IEnumerator SubmitStageResult(int stage, int score, List<SimpleQuestion> wrongQuestions, System.Action<bool> onComplete)
    {
        string url = $"{baseUrl}?action=submit-result";

        var postData = new
        {
            stage = stage,
            score = score,
            wrongQuestions = wrongQuestions
        };

        string json = JsonConvert.SerializeObject(postData);

        using UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("SubmitStageResult Error: " + www.error);
            onComplete?.Invoke(false);
        }
        else
        {
            onComplete?.Invoke(true);
        }
    }
}
