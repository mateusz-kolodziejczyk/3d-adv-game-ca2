using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private DatabaseAccess databaseAccess;
    private PlayerData playerData;
    [SerializeField] int scorePerLevel = 0;
    [SerializeField] private TMP_InputField usernameField, passwordField;


    private void Start()
    {
        databaseAccess = GetComponent<DatabaseAccess>();
        playerData = GetComponent<PlayerData>();
    }

    public void UpdatePlayerDatabase()
    {
        databaseAccess.SaveScore(playerData.Username, playerData.Score);
        databaseAccess.UpdateLevel(playerData.Username, SceneManager.GetActiveScene().buildIndex);
    }

    public void AddToScore()
    {
        playerData.Score += scorePerLevel;
    }

    public void UpdatePlayer(string username)
    {
        playerData.Username = username;
    }
    public void AdvanceLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void WrongPassword()
    {
        
    }

    public void AttemptSignupLogin()
    {
        databaseAccess.SignupLogin(usernameField.text, passwordField.text);
    }
}
