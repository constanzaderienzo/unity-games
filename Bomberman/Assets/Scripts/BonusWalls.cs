using System;
using UnityEngine;

public class BonusWalls : MonoBehaviour
{
    public Sprite bonusSprite;
    public int bonusLayer;
    public string bonusTag;
    public string bonusName;
    private Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

    }
    // Update is called once per frame
    void Break()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = bonusSprite;
        gameObject.tag = bonusTag;
        gameObject.layer = bonusLayer;
    }

    void DestroyConsumedBonus()
    {
        Destroy(gameObject);
    }

    void ActivateBonus()
    {
        switch(bonusName)
        {
            case "Bombs":
                Player.bombCount++;
            break;
            case "Speed":
                if(Player.targetSteps > 3)
                    Player.targetSteps--;
            break;
            case "Flamepass":
                player.flameImmunity = 1000f;
                player.sprite.color =  Color.red;
            break;
            case "Invincibility":
                player.enemyImmunity = 1000f;
                player.sprite.color =  Color.red;
            break;
            case "Wallpass":
                Player.wallPass = true;
            break;
            default:
            break;
        }
        DestroyConsumedBonus();
    }
}
