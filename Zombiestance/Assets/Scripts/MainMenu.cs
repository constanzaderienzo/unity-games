using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;
    public GameObject mainScreen;
    public GameObject optionsScreen;
    public GameObject helpScreen;
    public GameObject highscoresScreen;
    
    private bool musicOn = true;
    public static MainMenu instance = null;
    public Button startButton;
    public Button optionsButton;
    public Button helpButton;
    public Button highscoresButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.time = 20.00f;
        //CleanHT();
        PlayerPrefs.SetInt("Muted", 0);
    }
    void CleanHT()
    {
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt("score" + i, 0);
            PlayerPrefs.SetString("user" + i, "");
        }
    }
    
    private void Start()
    {
        audioSource.Play();
        SetSoundState();
        mainScreen.SetActive(true);
        optionsScreen.SetActive(false);
        helpScreen.SetActive(false);
        highscoresScreen.SetActive(false);
    }
    

    public void StartGame()
    {
        audioSource.Pause();
        SceneManager.LoadScene("OpeningCutScene");
        if(!musicOn)
        {
            AudioListener.volume = 0f;
        }
    }

    public void SetSoundState()
    { 
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
            AudioListener.volume = 0.6f;
        }
        else
        {
            AudioListener.volume = 0f;
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

    public void OpenOptions()
    {
        startButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        highscoresButton.enabled = false;
        optionsScreen.SetActive(true);
        mainScreen.SetActive(false);
    }

    public void OpenHelp()
    {
        startButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        highscoresButton.enabled = false;
        helpScreen.SetActive(true);
        mainScreen.SetActive(false);
    }

    public void OpenHighscores()
    {
        startButton.enabled = false;
        optionsButton.enabled = false;
        helpButton.enabled = false;
        highscoresButton.enabled = false;
        highscoresScreen.SetActive(true);
        mainScreen.SetActive(false);
    }
    
    public void CloseOptions()
    {
        mainScreen.SetActive(true);
        optionsScreen.SetActive(false);
        startButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        highscoresButton.enabled = true;
    }

    public void CloseHelp()
    {
        mainScreen.SetActive(true);
        helpScreen.SetActive(false);
        startButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        highscoresButton.enabled = true;
    }

    public void CloseHighscores()
    {
        mainScreen.SetActive(true);
        highscoresScreen.SetActive(false);
        startButton.enabled = true;
        optionsButton.enabled = true;
        helpButton.enabled = true;
        highscoresButton.enabled = true;
    }
}
