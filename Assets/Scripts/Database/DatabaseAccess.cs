using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    public void SignupLogin(string username, string password)
    {
        print("Attempting to sign up or login for username " + username);
        StartCoroutine(SignupLoginRequest(username, password));
    }


    IEnumerator PushRequest(string url)
    {
        url = serverURL + url;
        
        var www = new UnityWebRequest(url);

        yield return www.SendWebRequest();
    }

    private IEnumerator SignupLoginRequest(string username, string password)
    {
        var url = serverURL + $"user.php?name={username}&password={password}";

        var www = new UnityWebRequest(url);
        www.timeout = 5;
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) yield break;
        
        switch (www.responseCode)
        {
            case 200:
                gameController.UpdatePlayer(username);
                gameController.AdvanceLevel();
                break;
            default:
                gameController.WrongPassword();
                break;
        }
    }
}