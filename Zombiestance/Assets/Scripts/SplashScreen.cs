using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject splashScreen;
    public GameObject titleScreen;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.time = 2.00f;
        StartCoroutine(PlaySplashScreenVideo());
    }
    
    public IEnumerator PlaySplashScreenVideo() 
    {
        splashScreen.SetActive(true);
        yield return new WaitForSeconds(16.0f);
        splashScreen.SetActive(false);
        StartCoroutine(PlayTitleScreen());
    }
    
    public IEnumerator PlayTitleScreen() 
    {
        titleScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        titleScreen.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
