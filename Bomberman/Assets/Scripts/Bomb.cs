using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{    
    private const float BombDistance = 1.49f; // One and a half cell (minus 0.01 to avoid unwanted destructions)
    private Animator animator;
    private AudioSource source;


    private static readonly Vector3 Left = new Vector3(-BombDistance, 0, 0);
    private static readonly Vector3 Right = new Vector3(BombDistance, 0, 0);
    private static readonly Vector3 Up = new Vector3(0, BombDistance, 0);
    private static readonly Vector3 Down = new Vector3(0, -BombDistance, 0);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        source.Pause();
    }

    void Start()
    {
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f);
        yield return new WaitForSeconds(3.0f);
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        float left = ExplodeTo(Left);
        float right = ExplodeTo(Right);
        float up = ExplodeTo(Up);
        float down = ExplodeTo(Down);
        animator.SetBool("Exploded", true);
        source.Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    /**
     * Checks for objects to destroy between the bomb and the given point
     * Returns the distance between the bomb and the farthest destroyed object
     */
    private float ExplodeTo(Vector3 offsetFromBomb)
    {
        Vector3 bombPosition = transform.position;
        RaycastHit2D[] hits = Physics2D.LinecastAll(bombPosition, bombPosition + offsetFromBomb);
        float maxReachedDistance = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("Player"))
            {
                if(hits[i].collider.gameObject.GetComponent<Player>().flameImmunity <= 0f)
                {
                    hits[i].collider.gameObject.SendMessage("Die");
                    maxReachedDistance = hits[i].distance;
                }
            }
            else if (hits[i].collider.gameObject.CompareTag("Enemy"))
            {
                hits[i].collider.gameObject.SendMessage("Die");
                switch(hits[i].collider.gameObject.name)
                {
                    case "Enemy(Clone)":
                        if(!hits[i].collider.gameObject.GetComponent<Enemy>().isDead)
                            GameManager.instance.AddPoints(100);
                        break;
                    case "Snowflake(Clone)":
                        if(!hits[i].collider.gameObject.GetComponent<Enemy>().isDead)
                            GameManager.instance.AddPoints(200);
                        break;
                    case "Barrel(Clone)":
                        if(!hits[i].collider.gameObject.GetComponent<Enemy>().isDead)
                            GameManager.instance.AddPoints(400);
                        break;
                    default:
                        if(!hits[i].collider.gameObject.GetComponent<Enemy>().isDead)
                            GameManager.instance.AddPoints(100);
                        break;
                }
                hits[i].collider.gameObject.GetComponent<Enemy>().isDead = true;
                maxReachedDistance = hits[i].distance;
            }
            else if (hits[i].collider.gameObject.CompareTag("Brick"))
            {
                hits[i].collider.gameObject.SendMessage("Break");
                maxReachedDistance = hits[i].distance;
            }
        }
        return maxReachedDistance;
    }
}
