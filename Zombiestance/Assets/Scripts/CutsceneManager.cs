using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public GameObject pinky;
    public GameObject rick;
    public PlayableDirector pinkyPlayableDirector;
    public PlayableDirector rickPlayableDirector;

    private bool sceneLoaded = false;
    
    void OnEnable()
    {
        pinkyPlayableDirector.stopped += OnPlayableDirectorStopped;
        rickPlayableDirector.stopped += OnPlayableDirectorStopped;
    }
    private void Awake()
    {
        if (PlayerPrefs.GetInt("DefaultPlayer", 1) == 1)
        {
            pinky.SetActive(true);
            Animator animator = pinky.GetComponent<Animator>();
            rick.SetActive(false);
            pinkyPlayableDirector.Play();
        }
        else
        {
            pinky.SetActive(false);
            rick.SetActive(true); 
            rickPlayableDirector.Play();
        }
    }
    

    void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if ((director == pinkyPlayableDirector || director == rickPlayableDirector) && !sceneLoaded)
        {
            sceneLoaded = true;
            SceneManager.LoadScene("Game");
        }
    }

    private void OnDisable()
    {
        pinkyPlayableDirector.stopped -= OnPlayableDirectorStopped;
        rickPlayableDirector.stopped -= OnPlayableDirectorStopped;
    }
}
