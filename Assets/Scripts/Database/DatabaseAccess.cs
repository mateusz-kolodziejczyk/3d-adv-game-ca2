using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Util;

public class DatabaseAccess : MonoBehaviour
{

    [SerializeField] private string serverURL;


    private GameController gameController;

    private void Start()
    {
        gameController = GetComponent<GameController>();
    }

    public void SaveScore(string username, int score)
    {
        print("Starting to save score for user " + username);

        StartCoroutine(PushRequest($"score.php?name={username}&score={score}"));
    }

    public void UpdateLevel(string username, int currentLevel)
    {
        print("Starting to updated current Level for user " + username);
        
        StartCoroutine(PushRequest($"level.php?name={username}&current-level={currentLevel}"));
        
    }

    public void Login(string username, string password)
    {
        print("Attempting login for username " + username);
        StartCoroutine(LoginRequest(username, password));
    }

    public void SignUp(string username, string password)
    {
        print("Attempting to signup for username " + username);
        StartCoroutine(SignUpRequest(username, password));
    }

    IEnumerator PushRequest(string url)
    {
        url = serverURL + url;
        
        var www = new UnityWebRequest(url);

        yield return www.SendWebRequest();
    }
    private IEnumerator SignUpRequest(string username, string password)
    {
        var url = serverURL + $"signup.php?name={username}&password={password}";

        var www = new UnityWebRequest(url);
        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) yield break;
        
        switch (www.responseCode)
        {
            case 200:
                gameController.SuccessfulSignup(username);
                break;
            default:
                gameController.WrongUsername();
                break;
        }
    }
    private IEnumerator LoginRequest(string username, string password)
    {
        var url = serverURL + $"login.php?name={username}&password={password}";

        var www = new UnityWebRequest(url);
        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) yield break;
        
        switch (www.responseCode)
        {
            case 200:
                gameController.SuccessfulLogin(username);
                break;
            default:
                gameController.WrongPassword();
                break;
        }
    }

    public void GetData(string username)
    {

    }
    private IEnumerator GetDataRequest(string username)
    {
        var url = serverURL + $"data.php?name={username}";
        var www = UnityWebRequest.Get(url);

        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) yield break;
        switch (www.responseCode)
        {
            case 200:
                var response = www.downloadHandler.text;
                var data = JsonUtility.FromJson<ScoreLevelData>(response);
                Debug.Log("Response" + response);
                gameController.UpdatePlayerData(data);
                gameController.LaunchContinueMenu();
                break;
            default:
                break;
        }
    }
    private IEnumerator UpdateDataRequest(string username, int score, int level, bool exitAfter = false)
    {
        var url = serverURL + $"data.php?name={username}&score={score}&level={level}";
        var www = new UnityWebRequest(url);

        www.method = "PATCH";

        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) yield break;
            
        switch (www.responseCode)
        {
            case 200:
                Debug.Log("Successfuly updated data");
                if (exitAfter)
                {
                    gameController.Quit();
                }
                break;
            default:
                break;
        }
    }

    public void RequestData(string username)
    {
        print("Getting data for " + username);
        StartCoroutine(GetDataRequest(username));
    }

    public void UpdatePlayerData(string username, int score, int level, bool exitAfter = false)
    {
        print("Updating data for " + username);
        StartCoroutine(UpdateDataRequest(username, score, level, exitAfter));
    }
}