using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Button firstResolution;
    public Button secondResolution;

    public GameObject optionsScreen;

    void Start()
    {
        if (PlayerPrefs.GetInt("DefaultRes", 1) == 1)
        {
            ColorBlock secondColorBlock = secondResolution.colors;
            secondColorBlock.normalColor = secondResolution.colors.highlightedColor;
            secondResolution.colors = secondColorBlock;
        }
        else
        {
            ColorBlock firstColorBlock = firstResolution.colors;
            firstColorBlock.normalColor = firstResolution.colors.highlightedColor;
            firstResolution.colors = firstColorBlock;
        }
            
    }

    public void ApplyFirstResolution()
    {
        PlayerPrefs.SetInt("DefaultRes", 0);
        ColorBlock firstColorBlock = firstResolution.colors;
        firstColorBlock.normalColor = firstResolution.colors.highlightedColor;
        firstResolution.colors = firstColorBlock;

        ColorBlock secondColorBlock = secondResolution.colors;
        secondColorBlock.normalColor = Color.black;
        secondResolution.colors = secondColorBlock;
    }

    public void ApplySecondResolution()
    {
        PlayerPrefs.SetInt("DefaultRes", 1);
        ColorBlock firstColorBlock = firstResolution.colors;
        firstColorBlock.normalColor = Color.black;
        firstResolution.colors = firstColorBlock;

        ColorBlock secondColorBlock = secondResolution.colors;
        secondColorBlock.normalColor = secondResolution.colors.highlightedColor;
        secondResolution.colors = secondColorBlock;
    }
}
