using UnityEngine;
using UnityEngine.UI;

public class RemainingZombies : MonoBehaviour
{
    private int remaining;

    public Text text;


    public void SetRemaining(int zombies)
    {
        remaining = zombies;
        text.text = "Remaining: " + remaining;

    }
    
    public void Decrease()
    {
        remaining = remaining-- > 0 ? remaining-- : 0;
        text.text = "Remaining: " + remaining;
    } 
}
