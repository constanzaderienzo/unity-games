using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{

    private AudioSource source;
    public GameObject optionsScreen;
    public GameObject helpScreen;

    private bool musicOn = true;
    public static MainMenu instance = null;
    public Button startButton;
    public Button optionsButton;
    public Button helpButton;
    public Text musicText;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        source = GetComponent<AudioSource>();
    }
    private void Start()
    {
        source.Play();
        SetSoundState();
        optionsScreen.SetActive(false);
        helpScreen.SetActive(false);
    }

    public void StartGame()
    {
        source.Pause();
        SceneManager.LoadScene("SampleScene");
        if(!musicOn)
        {
            AudioListener.volume = 0f;
        }
    }

    public void SetSoundState()
    { 
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
           AudioListener.volume = 1f;
            musicText.text = "MUSIC: ON";
        }
        else
        {
            AudioListener.volume = 0f;
            musicText.text = "MUSIC: OFF";
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
        helpButton.enabled = false;
        optionsScreen.SetActive(true);
    }

    public void OpenHelp()
    {
        startButton.enabled = false;
        optionsButton.enabled = false;
        helpScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
        startButton.enabled = true;
        helpButton.enabled = true;
        startButton.Select();
    }

    public void CloseHelp()
    {
        helpScreen.SetActive(false);
        startButton.enabled = true;
        optionsButton.enabled = true;
        startButton.Select();
    }
}
