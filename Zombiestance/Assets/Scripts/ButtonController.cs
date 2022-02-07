using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler
{

    public AudioClip onHighlightClip;
    public AudioClip onClickClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        audioSource.PlayOneShot(onHighlightClip) ;
    }
 
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        audioSource.PlayOneShot(onClickClip) ;
    }  
}