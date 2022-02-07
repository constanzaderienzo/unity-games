using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    public Text player;
    public Text score;

    public void SetUp(string name, int points)
    {
        player.text = name;
        score.text = points.ToString();
    }

}
