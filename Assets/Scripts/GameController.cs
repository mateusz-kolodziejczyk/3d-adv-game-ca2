using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

public class GameController : MonoBehaviour
{
    private DatabaseAccess databaseAccess;
    private PlayerData playerData;
    [SerializeField] int scorePerLevel = 0;
    [SerializeField] private TMP_InputField usernameField, passwordField;
    private TextMeshProUGUI scoreText;
    [SerializeField] private ContinueMenu continueMenu;
    private int startingScore = 0;
    private void Start()
    {
        databaseAccess = GetComponent<DatabaseAccess>();
        playerData = GetComponent<PlayerData>();
        // Delete all previous preferences. if on the first scene
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("score", 0);
            PlayerPrefs.SetInt("level", 0);
            PlayerPrefs.SetString("username", "");
        }

        startingScore = PlayerPrefs.GetInt("score");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    private void Update()
    {
        if (scoreText == null && SceneManager.GetActiveScene().buildIndex > 1)
        {
            UpdateScoreText();
        }
    }

    public void UpdatePlayerDatabase(bool exitAfter = false)
    {
        databaseAccess.UpdatePlayerData(PlayerPrefs.GetString("username"), PlayerPrefs.GetInt("score"), PlayerPrefs.GetInt("level"), exitAfter);
    }

    public void UpdatePlayerData(ScoreLevelData data)
    {
        PlayerPrefs.SetInt("score", data.score);
        PlayerPrefs.SetInt("level", data.level);
    }
    public void AddToScore()
    {
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + scorePerLevel);
        UpdateScoreText();
    }

    public void AddToScore(int score)
    {
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + score);
        UpdateScoreText();
    }
    public void AdvanceLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void WrongPassword()
    {
        
    }

    public void AttemptLogin()
    {
        databaseAccess.Login(usernameField.text, passwordField.text);
    }
    public void AttemptSignup()
    {
        databaseAccess.SignUp(usernameField.text, passwordField.text);
    }

    public void RequestDataForPlayer()
    {
        databaseAccess.RequestData(PlayerPrefs.GetString("username"));
    }
    
    public void UpdateScoreText()
    {
        // Only set scoretext if it's null
        scoreText ??= GameObject.FindWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        
        scoreText.text = $"Score: {PlayerPrefs.GetInt("score")}";
        Debug.Log(scoreText.text);
    }

    
    // On successful login load in all the data and load the correct level.
    public void SuccessfulLogin(string username)
    {
        PlayerPrefs.SetString("username", username);
        RequestDataForPlayer();
    }

    public void LoadSavedLevel()
    {
        // Load saved level unless its 0(sign in screen then load the next)
        var level = PlayerPrefs.GetInt("level");
        if (level == 0)
        {
            AdvanceLevel();
        }
        else
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("level"));
        }
    }

    public void SaveAndQuit()
    {
        UpdatePlayerDatabase(true);
    }

    // On successful signup load the first level and set the useraname
    public void SuccessfulSignup(string username)
    {
        PlayerPrefs.SetString("username", username);
        AdvanceLevel();
    }
    public void WrongUsername()
    {
        
    }

    public void LaunchContinueMenu()
    {
        continueMenu?.SwitchChildrenActive();
    }
    public void RestartGame()
    {
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("level", 0);
        UpdatePlayerDatabase();
        SceneManager.LoadScene(2);
    }

    public void ContinueGame()
    {
        var level = PlayerPrefs.GetInt("level");
        if (level > 1)
        {
            SceneManager.LoadScene(level);
        }
        else
        {
            AdvanceLevel();
        }
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        PlayerPrefs.SetInt("score", startingScore);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
