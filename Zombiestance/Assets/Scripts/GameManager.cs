using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    
    public GameObject pinky;
    public GameObject rick;
    public RemainingZombies remainingZombies;
    public InfoPopup infoPopup;
    public GameObject inGameMenu;
    public GameObject musicToggle;
    public GameObject gameOver;
    private AudioSource audioSource;
    public WaveManager waveManager;
    public GameObject highscore;
    public InputField highscoreInput;
    
    private int zombiesCount;
    private int killedZombies;
    private static int wave;
    public int akAppearance = 5, shotgunAppearance = 10;
    public int burningZombieAppearance = 15, biohazardZombieAppearance = 20;
    [HideInInspector] public bool playersTurn;
    private string[] highscoreUsers;
    private int[] highscorePoints;
    private int highscoreIndex;
    public int hack = -1;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        highscorePoints = new int[5];
        highscoreUsers = new string[5];
        highscoreIndex = 0;
        inGameMenu.SetActive(false);
        gameOver.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        SetHighscoreTable();
        SetSoundState();
        SetPlayer();
        SetWave(1, -1);
    }

    private void Update()
    {
        WaveWon();
    }

    void SetPlayer()
    {
        if (PlayerPrefs.GetInt("DefaultPlayer", 1) == 1)
        {
            pinky.SetActive(true);
            rick.SetActive(false);
        }
        else
        {
            pinky.SetActive(false);
            rick.SetActive(true); 
        }
    }

    void SetWave(int waveNumber, int hack)
    {
        wave = waveNumber;
        if (hack == 0)
        {
            wave = 15;
            waveNumber = 15;
        }

        else if (hack == 1)
        {
            wave = 20;
            waveNumber = 20;
        }
        
        zombiesCount = waveManager.SetupWave(waveNumber, akAppearance, shotgunAppearance, burningZombieAppearance, biohazardZombieAppearance);
        killedZombies = 0;
        remainingZombies.SetRemaining(zombiesCount);
        playersTurn = true;
    }

    void SetHighscoreTable()
    {
        for(int i = 0; i < highscoreUsers.Length; i++)
        {
            highscoreUsers[i] = PlayerPrefs.GetString("user" + i, "");
            highscorePoints[i] = PlayerPrefs.GetInt("score" + i, 0);
        }
    }
    
    public int GetWave()
    {
        return wave;
    }
    
    public void SetSoundState()
    { 
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
           AudioListener.volume = 1f;
        }
        else
        {
            AudioListener.volume = 0f;
        }
    }

    public void WaveWon()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            hack = 0;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            hack = 1;
        }
        
        if(killedZombies >= zombiesCount)
        {
            // Debug.Log("Wave won");
            playersTurn = false;
            SetWave(++wave, hack);
            infoPopup.NewWave(wave);
        }
    }

    public void GameOver()
    {
        playersTurn = false;
        gameOver.SetActive(true);
        highscore.SetActive(CheckIfHighScore());
    }

    private bool CheckIfHighScore()
    {
        for (int i = 0; i < highscorePoints.Length; i++)
        {
            if (wave >= highscorePoints[i])
            {
                highscoreIndex = i;
                return true;
            }
        }
        return false;
    }
    
    public void AddHighscore()
    {
        string user = highscoreInput.text;
        if (!String.IsNullOrEmpty(user))
        {
            for (int j = highscorePoints.Length - 1; j > highscoreIndex; j--)
            {
                highscorePoints[j] = highscorePoints[j - 1];
                highscoreUsers[j] = highscoreUsers[j - 1];
                int shiftDownScore = PlayerPrefs.GetInt("score" + (j - 1));
                PlayerPrefs.SetInt("score"+ j, shiftDownScore);
                string shiftDownName = PlayerPrefs.GetString("user" + (j - 1));
                PlayerPrefs.SetString("user"+j, shiftDownName);
            }

            highscorePoints[highscoreIndex] = wave;
            PlayerPrefs.SetInt("score" + highscoreIndex, wave);
            PlayerPrefs.SetString("user" + highscoreIndex, user);
            highscore.SetActive(false);
        }
    }
    

    public void KilledZombie()
    {
        // Debug.Log("Killed zombie");
        killedZombies++;
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

    public void Pause()
    {
        inGameMenu.SetActive(true);
        Toggle toggle = musicToggle.GetComponent<Toggle>();
        Mute(toggle.onValueChanged);
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
            toggle.isOn = true;
        }
        else 
        {
            toggle.isOn = false;
        }
        Unmute(toggle.onValueChanged);
        playersTurn = false;
    }

    public void ResumePause()
    {
        inGameMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        playersTurn = true;
    }

    public void Exit()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("MainMenu");
    }


    public void Mute( UnityEngine.Events.UnityEventBase ev )
    {
        int count = ev.GetPersistentEventCount();
        for ( int i = 0 ; i < count ; i++ )
        {
            ev.SetPersistentListenerState( i, UnityEngine.Events.UnityEventCallState.Off );
        }
    }
 
    public void Unmute( UnityEngine.Events.UnityEventBase ev )
    {
        int count = ev.GetPersistentEventCount();
        for ( int i = 0 ; i < count ; i++ )
        {
            ev.SetPersistentListenerState( i, UnityEngine.Events.UnityEventCallState.RuntimeOnly );
        }
    }
}
