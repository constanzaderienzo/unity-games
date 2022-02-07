using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public const string Tag = "Enemy";
    private const float CenterDeltaPosition = 0.5f;
    public int StepsToBeCentered = 10;
    private const int MinSteps = 10;
    private const int MaxSteps = 20;
    
    public LayerMask boardLayer;
    public LayerMask bricksLayer;
    public LayerMask bombLayer;

    private Animator animator;
    
    private Vector3 movementDir;
    private float speed;
    private int cellsToChangeAxis;
    private int cells;
    private int stepsToBeCentered;
    private int steps;
    public bool isDead;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        isDead = false;
        speed = CenterDeltaPosition / StepsToBeCentered;
        cells = 0;
        cellsToChangeAxis = Random.Range(MinSteps, MaxSteps);
        steps = 0;
        stepsToBeCentered = StepsToBeCentered;
        movementDir = GetInitialDirection();
    }

    private Vector3 GetInitialDirection()
    {
        Vector3 direction = Vector3.right;
        Vector3 futurePosition1 = transform.position + direction * speed;
        Vector3 futurePosition2 = transform.position - direction * speed;
        if (IsOverlappingWithBoardOrBricks(futurePosition1) && IsOverlappingWithBoardOrBricks(futurePosition2))
        {
            direction = Vector3.up;
        }
        return direction;
    }

    private void FixedUpdate()
    {
        if(!GameManager.instance.playersTurn) return;
        if (cells == cellsToChangeAxis)
        {
            cellsToChangeAxis = Random.Range(MinSteps, MaxSteps);
            cells = 0;
            TryToChangeMovementAxis();
        }
        Move();
        Animate();
    }
    
    private void Animate()
    {
        if(!isDead)
        {
            if (movementDir.x != 0)
            {
                animator.SetFloat("Horizontal", movementDir.x);
            }
            else 
            {
                animator.SetFloat("Horizontal", movementDir.y);
            }
        }
        animator.SetBool("Dead", this.isDead);
    }

    private void TryToChangeMovementAxis()
    {
        Vector3 newMovementDir = new Vector3(movementDir.y, movementDir.x);
        Vector3 futurePosition = transform.position + newMovementDir * speed;
        if (IsOverlappingWithBoardOrBricks(futurePosition))
        {
            newMovementDir = -newMovementDir;
            futurePosition = transform.position + newMovementDir * speed;
        }
        
        if (!IsOverlappingWithBoardOrBricks(futurePosition))
        {
            movementDir = newMovementDir;
        }
    }

    private void Move()
    {
        if(!isDead)
        {
            Vector3 futurePosition = transform.position + movementDir * speed;

            if (IsOverlappingWithBoardOrBricks(futurePosition))
            {
                movementDir = -movementDir;
                futurePosition = transform.position + movementDir * speed;
            }
            
            if (!IsOverlappingWithBoardOrBricks(futurePosition))
            {
                transform.position = transform.position + movementDir * speed;
                if (++steps == stepsToBeCentered)
                {
                    steps = 0;
                    cells++;
                }
            }
        }
    }
    
    private bool IsOverlappingWithBoardOrBricks(Vector2 futurePosition)
    {
        Vector2 boxSize = new Vector2(0.95f, 0.95f);
        var overlappedBoard = Physics2D.OverlapBox(futurePosition, boxSize, 0, boardLayer);
        var overlappedBrick = Physics2D.OverlapBox(futurePosition, boxSize, 0, bricksLayer);
        var overlappedBomb = Physics2D.OverlapBox(futurePosition, boxSize, 0, bombLayer);
        return overlappedBoard != null || overlappedBrick != null || overlappedBomb != null;
    }

    public void Die()
    {
        if (!isDead)
        {
            StartCoroutine(DestroyEnemy());
        }
    }

    public IEnumerator DestroyEnemy() 
    {
        Animate();
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
