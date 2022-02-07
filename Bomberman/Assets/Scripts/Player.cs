using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public const string Tag = "Player";
    private const int InitialTargetSteps = 5;
    private const int CellFractionByDeltaPosition = 4;
    private const float AtomicDeltaPosition = 1.0f / CellFractionByDeltaPosition;
    private const float InitialAnimatorSpeed = 1f;
    public GameObject bombPrefab;
    public LayerMask boardLayer;
    public LayerMask bricksLayer;
    private Vector3 movementDir;
    private Animator animator;
    public AudioClip verticalWalking;
    public AudioClip horizontalWalking;
    public AudioClip bombDropped;
    public AudioClip lifeLost;
    public AudioClip dyingClip;
    public AudioClip bonusAcquired;
    private AudioSource source;
    public LayerMask exitLayer;
    public LayerMask bonusLayer;
    public Text livesText;
    public Text timeText;

    private bool isDead;
    private bool isOnTargetPosition;
    private int lives;
    private int steps;
    public static int targetSteps;
    private int timeLeft;
    private float animatorSpeed;
    private float speed;

    private List<GameObject> bombs;
    private Vector2Int offsetFromCellCenter;
    public static int bombCount;
    public float flameImmunity = 0f;
    public float enemyImmunity = 0f; 
    public static bool wallPass = false;
    public SpriteRenderer sprite;
    private Color originalColor;
    private Vector3 futureBombPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        lives = GameManager.instance.lives;
        bombs = new List<GameObject>();
        originalColor = sprite.color;
    }

    private void Start()
    {
        isDead = false;
        transform.position = new Vector3(1.5f, 9.5f, -1.0f);
        animatorSpeed = InitialAnimatorSpeed;
        animator.speed = 0;
        steps = 0;
        targetSteps = InitialTargetSteps;
        speed = AtomicDeltaPosition / targetSteps;
        isOnTargetPosition = true;
        animator.SetBool("Dead", false);
        animator.SetFloat("Horizontal", -1);
        offsetFromCellCenter = Vector2Int.zero;
        futureBombPosition = transform.position;
        timeLeft = GameManager.instance.timeLeft;
        InvokeRepeating(nameof(OutputTime), 1f, 1f);  //1s delay, repeat every 1s
    }

    void OutputTime() {
        if (!GameManager.paused && GameManager.instance.playersTurn)
        {
            timeLeft--;
            if (timeLeft == 0)
                StartCoroutine(Die());
            timeText.text = "TIME " + timeLeft;
        }
    }

    private void CheckIfGameOver()
    {
        if (lives < 0)
        {
            enemyImmunity = 0f;
            flameImmunity = 0f;
            wallPass = false;
            targetSteps = 5;
            bombCount = 0;
            GameManager.instance.GameOver();
        }

    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        if (!isDead)
        {
            if (isOnTargetPosition)
            {
                ReadMovementInput();
            }
            Move();
            DecreaseBonus();
            Animate();
            PlaySounds();
        }
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GameManager.instance.Pause();
        }
        if (!GameManager.instance.playersTurn)
        {
            return;
        }
        if (!isDead)
        {
            Action();
        }
        CheckIfGameOver();
    }

    private void Restart()
    {
        transform.position = new Vector3(1.5f, 9.5f, -1.0f);
        timeLeft = GameManager.instance.timeLeft;
        timeText.text = "TIME " + timeLeft;
        animatorSpeed = InitialAnimatorSpeed;
        animator.speed = 0;
        steps = 0;
        speed = AtomicDeltaPosition / targetSteps;
        isOnTargetPosition = true;
        animator.SetBool("Dead", false);
        animator.SetFloat("Horizontal", -1);
        futureBombPosition = transform.position;
        offsetFromCellCenter = Vector2Int.zero;
        isDead = false;
    }

    private void ReadMovementInput()
    {
        int movementX = 0;
        int movementY = 0;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movementX = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movementX = 1;
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            movementY = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            movementY = -1;
        }

        movementDir = new Vector3(movementX, movementY);

        if (movementX != 0 || movementY != 0)
        {
            Vector3 futurePositionCandidate = transform.position + movementDir * speed * targetSteps;
            isOnTargetPosition = IsOverlappingWithBoardOrBricks(futurePositionCandidate);
            if (!isOnTargetPosition)
            {
                offsetFromCellCenter = new Vector2Int(
                    (offsetFromCellCenter.x + (int) movementDir.x) % CellFractionByDeltaPosition,
                    (offsetFromCellCenter.y + (int) movementDir.y) % CellFractionByDeltaPosition
                    );
            }
        }
    }

    private Vector2 NextBombPosition(Vector2 playerPosition, Vector2 playerLookingDirection)
    {
        const float epsilon = 0.1f;
        var x = Mathf.RoundToInt(playerPosition.x);
        if (Mathf.Abs(playerPosition.x - x) > epsilon)
        {
            x = (int) playerPosition.x;
        }
        var y = Mathf.RoundToInt(playerPosition.y);
        if (Mathf.Abs(playerPosition.y - y) > epsilon)
        {
            y = (int) playerPosition.y;
        }
        return IsExactlyBetweenTwoCells(playerPosition, x, y)
            ? GetBombPositionBreakingTieByDirection(playerLookingDirection, x, y)
            : new Vector2(x + 0.5f, y + 0.5f);
    }

    private bool IsExactlyBetweenTwoCells(Vector2 playerPosition, int x, int y)
    {
        const float epsilon = 0.1f;
        return Mathf.Abs(playerPosition.x - x) < epsilon || Mathf.Abs(playerPosition.y - y) < epsilon;
    }

    private Vector2 GetBombPositionBreakingTieByDirection(Vector2 playerLookingDirection, int x, int y)
    {
        Vector2 nextBombPosition;
        if (playerLookingDirection.Equals(Vector2.left))
        {
            nextBombPosition = new Vector2(x - 0.5f, y + 0.5f);
        }
        else if (playerLookingDirection.Equals(Vector2.right))
        {
            nextBombPosition = new Vector2(x + 0.5f, y + 0.5f);
        }
        else if (playerLookingDirection.Equals(Vector2.up))
        {
            nextBombPosition = new Vector2(x + 0.5f, y + 0.5f);
        }
        else
        {
            nextBombPosition = new Vector2(x + 0.5f, y - 0.5f);
        }
        return nextBombPosition;
    }

    private void Move()
    {
        if (!isOnTargetPosition)
        {
            transform.position += movementDir * speed;;
            steps++;
            if (steps == targetSteps)
            {
                steps = 0;
                isOnTargetPosition = true;
                if (offsetFromCellCenter == Vector2Int.zero)
                {
                    Collider2D collider;
                    if (IsOverlappingWithExit())
                    {
                        GameManager.instance.LevelWon();
                    }
                    else if((collider = IsOverlappingWithBonus()) != null)
                    {
                        source.PlayOneShot(bonusAcquired);
                        collider.gameObject.SendMessage("ActivateBonus");
                        speed = AtomicDeltaPosition / targetSteps;
                    }
                }
            }
            futureBombPosition = NextBombPosition(transform.position, movementDir);
        }
    }

    private bool IsOverlappingWithBoardOrBricks(Vector2 futurePosition)
    {
        Vector2 boxSize = new Vector2(0.95f, 0.95f);
        var overlappedBoard = Physics2D.OverlapBox(futurePosition, boxSize, 0, boardLayer);
        var overlappedBrick = Physics2D.OverlapBox(futurePosition, boxSize, 0, bricksLayer);
        return overlappedBoard != null || (overlappedBrick != null && !wallPass);
    }  
    
    private bool IsOverlappingWithExit()
    {
        return Physics2D.OverlapCircle(transform.position, 0.1f, exitLayer) != null;
    }
    
    private Collider2D IsOverlappingWithBonus()
    {
        return Physics2D.OverlapCircle(transform.position, 0.1f, bonusLayer);
    }

    private void Animate()
    {
        if (!isDead)
        {
            animator.SetFloat("Horizontal", movementDir.x);
            animator.SetFloat("Vertical", movementDir.y);
            animator.speed = isOnTargetPosition ? 0 : animatorSpeed;
        }
        else
        {
            animator.SetBool("Dead", isDead);
            animator.speed = animatorSpeed;
        }
    }

    private void PlaySounds()
    {
        if(animator.GetFloat("Horizontal") != 0)
        {
            if(!source.isPlaying || source.clip != horizontalWalking)
            {
                source.Play();
                source.clip = horizontalWalking;
            }
        }
        else if(animator.GetFloat("Vertical") != 0)
        {
            if(!source.isPlaying || source.clip != verticalWalking)
            {
                source.Play();
                source.clip = verticalWalking;
            }
        }
    }
    
    private void Action()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropBomb();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy") && !isDead) {
            StartCoroutine(Die());
        }
    } 
    
    IEnumerator Die() 
    {
        if(!isDead && (enemyImmunity <= 0f || timeLeft <= 0))
        {
            lives--;
            enemyImmunity = 0f;
            flameImmunity = 0f;
            CheckIfGameOver();
            livesText.text = "LEFT " + lives;
            isDead = true;
            Animate();
            source.clip = dyingClip;
            source.Play();
            GameManager.instance.playersTurn = false;
            GameManager.instance.TogglePlayingMusic();
            yield return new WaitForSeconds(1.5f);
            if(lives >=0)
                StartCoroutine(PlayDeath());
        }
    }
    
    IEnumerator PlayDeath()
    {
        source.clip = lifeLost;
        source.pitch = 1;
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
        source.pitch = 2;
        GameManager.instance.playersTurn = true;
        GameManager.instance.TogglePlayingMusic();
        Restart();
    }
    
    private void DropBomb()
    {
        bombs.RemoveAll(item => item == null);
        if (bombs.Count <= bombCount)
        {
            source.PlayOneShot(bombDropped);
            bombs.Add(Instantiate(bombPrefab, futureBombPosition + Vector3.forward, Quaternion.identity));
        }
    }
    
    private void DecreaseBonus()
    {
        if(flameImmunity > 0f)
            flameImmunity--;
        if(enemyImmunity > 0f)
            enemyImmunity--;
        if(flameImmunity <= 0f && enemyImmunity <= 0f)
            sprite.color = originalColor;
    }
}
