// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using UnityEngine.Networking;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine.Video;

// public class Materi2Manager : MonoBehaviour
// {
//     [System.Serializable]
//     public class MateriData
//     {
//         public string title;
//         public string content;
//     }

//     [System.Serializable]
//     public class MateriList
//     {
//         public List<MateriData> materi;
//     }

//     [System.Serializable]
//     public class MateriResponseData
//     {
//         public string playerName;
//         public string response;
//         public float initial_mastery;
//         public int stage;
//     }

//     [Header("Panels")]
//     public GameObject slide1Panel;
//     public GameObject slide2Panel;
//     public GameObject confirmPanel;

//     [Header("Slide 1 UI")]
//     public TMP_Text slide1TitleText;
//     public TMP_Text slide1ContentText;
//     public Image materiImage1;
//     public Image materiImage2;

//     [Header("Slide 2 UI")]
//     public VideoPlayer videoPlayer;
//     public RawImage videoRawImage;

//     [Header("Navigation")]
//     public Button nextButton;
//     public Button prevButton;
//     public Button yesButton;
//     public Button noButton;

//     [Header("Settings")]
//     public Sprite[] materiImages;
//     public string category = "Hobby";
//     public string videoFileName = "hobby.mp4"; // Harus ada di StreamingAssets

//     private string playerName;
//     private string apiUrlMateri = "https://mclquiz.my.id/quiz/api.php?action=get-materi&category=";
//     private string apiUrlSubmit = "https://mclquiz.my.id/quiz/api.php?action=submit-materi-response";

//     private List<MateriData> materiList = new List<MateriData>();
//     private int currentSlide = 0;

//     void Start()
//     {
//         playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

//         confirmPanel.SetActive(false);
//         slide1Panel.SetActive(false);
//         slide2Panel.SetActive(false);

//         nextButton.onClick.AddListener(NextSlide);
//         prevButton.onClick.AddListener(PrevSlide);

//         yesButton.onClick.AddListener(() =>
// {
//     PlayerPrefs.SetFloat("LastMastery", 0.3f);
//     PlayerPrefs.SetString("Stage2Difficulty", "Adaptive");
//     PlayerPrefs.Save();

//     StartCoroutine(SendMateriResponse("Sudah"));
//     SceneManager.LoadScene("IntroQuiz2");
// });

// noButton.onClick.AddListener(() =>
// {
//     PlayerPrefs.SetFloat("LastMastery", 0.3f);
//     PlayerPrefs.SetString("Stage2Difficulty", "easy");
//     PlayerPrefs.Save();

//     StartCoroutine(SendMateriResponse("Belum"));
//     SceneManager.LoadScene("Adv2");
// });


//         StartCoroutine(LoadMateri());
//     }

//     IEnumerator LoadMateri()
//     {
//         UnityWebRequest request = UnityWebRequest.Get(apiUrlMateri + category);
//         yield return request.SendWebRequest();

//         if (request.result == UnityWebRequest.Result.Success)
//         {
//             string json = FixJson(request.downloadHandler.text);
//             MateriList data = JsonUtility.FromJson<MateriList>(json);
//             materiList = data.materi;

//             if (materiList.Count > 0)
//                 ShowSlide(0);
//         }
//         else
//         {
//             Debug.LogError("Gagal ambil materi: " + request.error);
//         }
//     }

//     void ShowSlide(int index)
//     {
//         currentSlide = index;

//         // Reset semua panel
//         slide1Panel.SetActive(false);
//         slide2Panel.SetActive(false);
//         confirmPanel.SetActive(false);

//         if (index == 0)
//         {
//             slide1Panel.SetActive(true);
//             slide1TitleText.text = materiList[index].title;
//             slide1ContentText.text = materiList[index].content;

//             if (materiImages.Length >= 2)
//             {
//                 materiImage1.sprite = materiImages[0];
//                 materiImage2.sprite = materiImages[1];
//             }
//         }
//         else if (index == 1)
//         {
//             slide2Panel.SetActive(true);

//             string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
//             videoPlayer.url = path;
//             videoPlayer.Play();
//         }
//         else
//         {
//             confirmPanel.SetActive(true);
//         }

//         prevButton.gameObject.SetActive(index > 0);
//         nextButton.gameObject.SetActive(index < 2);
//     }

//     void NextSlide()
//     {
//         if (currentSlide < 2)
//         {
//             ShowSlide(currentSlide + 1);
//         }
//     }

//     void PrevSlide()
//     {
//         if (currentSlide > 0)
//         {
//             ShowSlide(currentSlide - 1);
//         }
//     }

//     IEnumerator SendMateriResponse(string response)
//     {
//         MateriResponseData data = new MateriResponseData
//         {
//             playerName = playerName,
//             response = response,
//             initial_mastery = 0.3f,
//             stage = 2
//         };

//         string jsonData = JsonUtility.ToJson(data);
//         byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

//         UnityWebRequest request = new UnityWebRequest(apiUrlSubmit, "POST");
//         request.uploadHandler = new UploadHandlerRaw(jsonBytes);
//         request.downloadHandler = new DownloadHandlerBuffer();
//         request.SetRequestHeader("Content-Type", "application/json");

//         yield return request.SendWebRequest();

//         if (request.result != UnityWebRequest.Result.Success)
//         {
//             Debug.LogError("Gagal kirim data materi: " + request.error);
//         }
//         else
//         {
//             Debug.Log("Berhasil kirim data materi: " + request.downloadHandler.text);
//         }
//     }

//     private string FixJson(string value)
//     {
//         if (!value.StartsWith("{"))
//         {
//             return "{\"materi\":" + value + "}";
//         }
//         return value;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Video;
using TMPro;

public class Materi2Manager : MonoBehaviour
{
    [System.Serializable]
    public class MateriResponseData
    {
        public string playerName;
        public string response;
        public float initial_mastery;
        public int stage;
    }

    [Header("Instruction Text")]
    public TMP_Text instructionText;

    [Header("Panels")]
    public GameObject slide1Panel;
    public GameObject slide2Panel;
    public GameObject confirmPanel;

    [Header("Slide 2 (Video)")]
    public VideoPlayer videoPlayer;
    public string videoFileName = "hobby.mp4"; // StreamingAssets

    [Header("Navigation")]
    public Button nextButton;
    public Button prevButton;
    public Button yesButton;
    public Button noButton;

    [Header("Settings")]
    public int stage = 2; // ✅ beda di sini saja

    private int currentSlide = 0;
    private string playerName;

    private string apiUrlSubmit =
        "https://mclquiz.my.id/quiz/api.php?action=submit-materi-response";

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

        slide1Panel.SetActive(true);
        slide2Panel.SetActive(false);
        confirmPanel.SetActive(false);

        nextButton.onClick.AddListener(NextSlide);
        prevButton.onClick.AddListener(PrevSlide);

        yesButton.onClick.AddListener(() =>
        {
            SaveMastery(0.3f, "Adaptive");
            StartCoroutine(SendMateriResponse("Sudah"));
            SceneManager.LoadScene("IntroQuiz2");
        });

        noButton.onClick.AddListener(() =>
        {
            SaveMastery(0.3f, "Easy");
            StartCoroutine(SendMateriResponse("Belum"));
            SceneManager.LoadScene("Adv2");
        });

        UpdateNavigation();
    }

    void ShowSlide(int index)
    {
        currentSlide = index;

        slide1Panel.SetActive(index == 0);
        slide2Panel.SetActive(index == 1);
        confirmPanel.SetActive(index == 2);

        if (index == 1)
        {
            string path = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                videoFileName
            );
            videoPlayer.url = path;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
        }

        UpdateNavigation();
    }

    void NextSlide()
    {
        if (currentSlide < 2)
            ShowSlide(currentSlide + 1);
    }

    void PrevSlide()
    {
        if (currentSlide > 0)
            ShowSlide(currentSlide - 1);
    }

    void UpdateNavigation()
    {
        prevButton.interactable = currentSlide > 0;
        nextButton.interactable = currentSlide < 2;

        if (instructionText != null)
            instructionText.gameObject.SetActive(currentSlide == 0);
    }

    void SaveMastery(float mastery, string difficulty)
    {
        PlayerPrefs.SetFloat("LastMastery", mastery);
        PlayerPrefs.SetString("Stage2Difficulty", difficulty); // ✅ stage 2
        PlayerPrefs.Save();
    }

    IEnumerator SendMateriResponse(string response)
    {
        MateriResponseData data = new MateriResponseData
        {
            playerName = playerName,
            response = response,
            initial_mastery = 0.3f,
            stage = stage
        };

        string jsonData = JsonUtility.ToJson(data);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrlSubmit, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Gagal kirim mastery: " + request.error);
    }
}
