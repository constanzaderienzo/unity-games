using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public static GameManager instance = null;
    
    public BoardManager boardScript;
    private static int level = 1;
    public int lives = 2;
    private static int points = 0;
    public int timeLeft = 200;
    private int enemyCount = 2;
    private int killedEnemies = 0;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private Text gameOverText;
    private GameObject gameOverImage;
    private GameObject inGameMenuImage;

    private AudioSource source;
    public AudioClip playingGame;
    public AudioClip gameOverAudio;
    public Text pointsText;
    private Text musicText;
    public static bool paused;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        boardScript = GetComponent<BoardManager>();
        source = GetComponent<AudioSource>();
        paused = false;
        InitGame();
    }

    private void OnSceneLoaded(int index) 
    {
        level++;
        InitGame();
    }

    public void GameOver() 
    {
        gameOverImage.SetActive(true);
        playersTurn = false;
        StartCoroutine(PlayGameOver());
    }
    
    IEnumerator PlayGameOver()
    {
        source.Play();
        source.clip = gameOverAudio;
        yield return new WaitForSeconds(source.clip.length);
        source.Pause();
        level = 1;
        points = 0;
        killedEnemies = 0;
        lives = 2;
        SceneManager.LoadScene("Main Menu");    
    }

    public void Pause() 
    {
        if (!paused) 
        {
            inGameMenuImage.SetActive(true);
            playersTurn = false;
            paused = true;
        }
        else
        {
            inGameMenuImage.SetActive(false);
            playersTurn = true;
            paused = false;
        }
    }

    void InitGame() {
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        pointsText = GameObject.Find("PointsText").GetComponent<Text>();
        gameOverImage = GameObject.Find("GameOverImage");
        gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        levelText.text = "STAGE " + level;
        if(GameManager.points == 0)
        {
            pointsText.text = "00";
        }
        else 
        {
            pointsText.text = GameManager.points.ToString();
        }
        levelImage.SetActive(true);
        gameOverImage.SetActive(false);
        inGameMenuImage = GameObject.Find("InGameMenuImage");
        musicText = GameObject.Find("MusicText").GetComponent<Text>();
        inGameMenuImage.SetActive(false);
        Invoke("HideLevelImage", levelStartDelay);
        enemyCount = level * 2;
        boardScript.SetupScene(level, enemyCount);
        SetSoundState();
        SetResolution();
        StartCoroutine(PlayAudios());
    }

    IEnumerator PlayAudios()
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
        source.clip = playingGame;
        source.loop = true;
        source.Play();
    }

    public void TogglePlayingMusic()
    {
        if(source.isPlaying)
            source.Pause();
        else
            source.Play();
    }

    public void SetSoundState()
    { 
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
           AudioListener.volume = 1f;
           if (musicText == null) 
           {
                musicText = GameObject.Find("MusicText").GetComponent<Text>();
           }
           musicText.text = "MUSIC: ON";
        }
        else
        {
            AudioListener.volume = 0f;
            if (musicText == null) 
            {
                musicText = GameObject.Find("MusicText").GetComponent<Text>();
            }
            musicText.text = "MUSIC: OFF";
        }
    }

    public void SetResolution()
    { 
        if(PlayerPrefs.GetInt("DefaultRes", 1) == 1)
        {
            Screen.SetResolution(640, 480, Screen.fullScreen);
        }
        else
        {
            Screen.SetResolution(320, 200, Screen.fullScreen);
        }
    }
    
    public void ToggleMusic()
    {
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
            PlayerPrefs.SetInt("Muted", 1);
        }
        else 
        {
            PlayerPrefs.SetInt("Muted", 0);
        }
        SetSoundState();
    }

    private void HideLevelImage() 
    {
        levelImage.SetActive(false);
    }   

    public void AddPoints(int points)
    {
        killedEnemies++;
        GameManager.points += points;
        pointsText.text = GameManager.points.ToString();
    }

    public void LevelWon()
    {
        if(killedEnemies >= enemyCount)
        {
            playersTurn = false;
            level++;
            killedEnemies = 0;
            SceneManager.LoadScene("SampleScene");
            return;
        }
    }
}
