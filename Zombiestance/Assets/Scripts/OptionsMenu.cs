using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Button rickButton;
    public Button pinkyButton;
    public GameObject optionsScreen;

    void Start()
    {
        if (PlayerPrefs.GetInt("DefaultPlayer", 1) == 1)
        {
            ColorBlock pinkyColorBlock = pinkyButton.colors;
            pinkyColorBlock.normalColor = pinkyButton.colors.selectedColor;
            pinkyButton.colors = pinkyColorBlock;
        }
        else
        {
            ColorBlock rickColorBlock = rickButton.colors;
            rickColorBlock.normalColor = rickButton.colors.selectedColor;
            rickButton.colors = rickColorBlock;
        }
            
    }

    public void SelectRickAsPlayer()
    {
        PlayerPrefs.SetInt("DefaultPlayer", 0);
        ColorBlock rickColorBlock = rickButton.colors;
        rickColorBlock.normalColor = rickButton.colors.selectedColor;
        rickButton.colors = rickColorBlock;

        ColorBlock pinkyColorBlock = pinkyButton.colors;
        pinkyColorBlock.normalColor = Color.white;
        pinkyButton.colors = pinkyColorBlock;
    }

    public void SelectPinkyAsPlayer()
    {
        PlayerPrefs.SetInt("DefaultPlayer", 1);
        ColorBlock rickColorBlock = rickButton.colors;
        rickColorBlock.normalColor = Color.white;
        rickButton.colors = rickColorBlock;

        ColorBlock pinkyColorBlock = pinkyButton.colors;
        pinkyColorBlock.normalColor = pinkyButton.colors.selectedColor;
        pinkyButton.colors = pinkyColorBlock;
    }
}
