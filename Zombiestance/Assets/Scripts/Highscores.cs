using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Highscores : MonoBehaviour
{
    public GameObject playerEntry;
    public Text player, wave, noHighscore;
    
    private string[] highscoreUsers;
    private int[] highscorePoints;
    void Awake()
    {
        highscorePoints = new int[5];
        highscoreUsers = new string[5];
        player.enabled = false;
        wave.enabled = false;
        noHighscore.enabled = true;
        SetHighscoreTable();
    }

    void SetHighscoreTable()
    {
        for(int i = 0; i < highscoreUsers.Length; i++)
        {
            highscoreUsers[i] = PlayerPrefs.GetString("user" + i, "");
            highscorePoints[i] = PlayerPrefs.GetInt("score" + i, 0);
            if (highscorePoints[i] != 0)
            {
                player.enabled = true;
                wave.enabled = true;
                noHighscore.enabled = false;
                GameObject go = Instantiate(playerEntry, this.transform);
                if (highscoreUsers[i] == "")
                {
                    go.GetComponent<PlayerEntry>().SetUp("---", highscorePoints[i]);   
                }
                else
                {
                    go.GetComponent<PlayerEntry>().SetUp(highscoreUsers[i], highscorePoints[i]);
                }
            }
        }
    }
}
