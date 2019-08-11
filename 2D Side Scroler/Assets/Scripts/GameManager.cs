using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public GameObject playCanvas;
    public GameObject gameCanvas;
    public GameObject deathCanvas;
    
    public float HighScore { set; get; }

    
    private bool gameStarted;
    public bool GameStarted {
        get {
            return gameStarted;
        }
    }
    private bool paused;
    public bool Paused { get { return paused; } }

    private float timeElapsed = 0;
    private float score = 0;

    private PlayerMovement player;

    //Reset game event
    public delegate void ResetGameDelegate();
    public event ResetGameDelegate ResetGameEvent;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        gameStarted = false;
        player = FindObjectOfType<PlayerMovement>();

        //Find the respective buttons the respective canvases
        if (playCanvas != null)
        {
            Button playButton = playCanvas.transform.Find("PlayButton").GetComponent<Button>();
            //Add the play game function to the button
            playButton.onClick.AddListener(() => { StartGame(); });

            Text t = playCanvas.transform.Find("HighScore").GetComponent<Text>();
            t.text = "High Score: " + ((int)PlayerPrefs.GetFloat("HighScore")).ToString();
        }

        if (gameCanvas != null)
        {
            //Assign proper functions to the pause panel
            gameCanvas.transform.Find("ReturnButton").GetComponent<Button>().onClick.AddListener(ActivatePausePanel);

            gameCanvas.transform.GetChild(1).gameObject.SetActive(false);
            gameCanvas.transform.GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(DeactivatePausePanel);
            gameCanvas.transform.GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener(GoToMainMenu);
            //gameCanvas.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(GoToMainMenu);
            gameCanvas.GetComponent<Canvas>().sortingOrder = 25;

            gameCanvas.SetActive(false);
        }
        if (deathCanvas != null)
        {
            Button mainMenu = deathCanvas.transform.Find("TryAgain").GetComponent<Button>();
            //Add the play game function to the button
            mainMenu.onClick.AddListener(() => { ResetGame(); });

            deathCanvas.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameStarted)
            return;

        timeElapsed += Time.deltaTime;
        GenerateScore();
        CheckPlayerDeath();
	}
    
    private void GenerateScore()
    {
        score +=Time.deltaTime * 2 * DifficultyManager.Instance.DifficultyParameter;
        gameCanvas.transform.GetChild(2).GetComponent<Text>().text = "Score: " + (int)score;
    }

    private void CheckPlayerDeath()
    {
        if(player.Dead && gameStarted)
        {
            StartCoroutine(PlayerIsDead());
        }
        //Display health
        DisplayPlayerHealth();
    }

    private void DisplayPlayerHealth()
    {
        gameCanvas.transform.GetChild(3).GetComponent<Text>().text = "Health: " + player.Health;
    }

    private IEnumerator PlayerIsDead()
    {
        //Reset the game started variable so that all the processes pauses
        gameStarted = false;

        yield return new WaitForSeconds(2);
        deathCanvas.SetActive(true);
        SaveHighScore();

        Text t;
        t = deathCanvas.transform.Find("HighScore").GetComponent<Text>();
        t.text = "High Score: " + ((int)PlayerPrefs.GetFloat("HighScore")).ToString();
        t = deathCanvas.transform.Find("YourScore").GetComponent<Text>();
        t.text = "Your Score: " + ((int)score).ToString();
        
    }

    private void SaveHighScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (PlayerPrefs.GetFloat("HighScore") < score)
            {
                Debug.Log("High Score is set");
                PlayerPrefs.SetFloat("HighScore", score);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
        
        PlayerPrefs.Save();
    }

    private void StartGame()
    {
        //TODO: add the canvas fade animation, then start the game
        playCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameStarted = true;
        
    }

    private void ActivatePausePanel()
    {
        AudioListener.pause = true;
        gameCanvas.transform.GetChild(1).gameObject.SetActive(true);
        Time.timeScale = 0;

        paused = true;
    }

    private void DeactivatePausePanel()
    {
        AudioListener.pause = false;
        gameCanvas.transform.GetChild(1).gameObject.SetActive(false);
        Time.timeScale = 1;

        paused = false;
    }

    private void GoToMainMenu()
    {
        DeactivatePausePanel();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void ResetGame()
    {
        deathCanvas.SetActive(false);
        timeElapsed = 0;
        score = 0;

        ResetGameEvent();   //Execute all functions subscribed to this event

        gameStarted = true;
        Time.timeScale = 1;
    }

    public float GetTimeElapsed()
    {
        return timeElapsed;
    }


}
