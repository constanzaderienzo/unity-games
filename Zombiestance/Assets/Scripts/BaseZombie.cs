using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using System.Collections;

public abstract class BaseZombie : MonoBehaviour
{
    public float health = 100f;
    public float minRadiusToFollowTarget = 50f;
    public float secondsToTakeDamage = 1f;
    public float damage = 10f;
    public bool isDead;
    public RemainingZombies remaining;
    public GameObject playerTarget;
    public GameManager gameManager;
    public GameObject pickupPrefab;
    public GameObject bloodSplatterPrefab;
    public GameObject akAmmoPrefab;
    public GameObject shotgunAmmoPrefab;
    public AudioClip[] breathings;
    public AudioClip damageClip;
    public AudioClip attackClip;
    public AudioClip dyingClip;

    protected NavMeshAgent NavMeshAgent;
    protected Vector3 LastRandomPointToFollow;
    protected AudioSource AudioSource;
    protected bool Breathing;
    private const float PositionTimeThreshold = 2f;
    private Vector3 _lastPosition;
    private float _timePassedInSameArea;
    
    private Animator _animator;
    private float _secondsColliding = 0;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        remaining = GameObject.Find("RemainingZombies").GetComponent<RemainingZombies>();
        AudioSource = GetComponent<AudioSource>();
        playerTarget = GetPlayerTarget();
        _animator = GetComponent<Animator>();
        _animator.SetBool("dead", isDead);
        isDead = false;
        Breathing = false;
        _lastPosition = transform.position;
        _timePassedInSameArea = 0;
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        OnTakeDamage();
        if (!isDead && health <= 0f)
        {
            Vector3 bloodPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            Instantiate(bloodSplatterPrefab, bloodPosition, Quaternion.identity);
            Die();
        }
        else
        {
            Vector3 bloodPosition = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            Instantiate(bloodSplatterPrefab, bloodPosition, Quaternion.identity);
        }
    }

    protected void Die()
    {
        if (!isDead)
        {
            remaining.Decrease();
            gameManager.KilledZombie();
            float rand = Random.value;
            if (gameManager.GetWave() > gameManager.akAppearance)
            {
                if (rand >= 0.99f)
                    Instantiate(akAmmoPrefab, transform.position, Quaternion.identity);
                else if (rand <= 0.01)
                    Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            }
            else if (gameManager.GetWave() > gameManager.shotgunAppearance)
            {
                if (rand >= 0.99f)
                    Instantiate(shotgunAmmoPrefab, transform.position, Quaternion.identity);
                else if (rand >= 0.98)
                    Instantiate(akAmmoPrefab, transform.position, Quaternion.identity);
                else if (rand <= 0.01)
                    Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                if (rand >= 0.99f)
                    Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            }

            OnDie();
            StartCoroutine(DestroyZombie());
        }
    }

    private void IncrementTimeInSameArea()
    {
        if (Vector3.Distance(transform.position, _lastPosition) < 3f)
        {
            _timePassedInSameArea += Time.deltaTime;
        }
        else
        {
            _timePassedInSameArea = 0;
        }
    }

    protected void RefreshDestToAvoidInPlaceWalkingIfNeeded()
    {
        IncrementTimeInSameArea();
        if (_timePassedInSameArea > PositionTimeThreshold)
        {
            LastRandomPointToFollow = GetRandomPointToFollow();
            NavMeshAgent.SetDestination(LastRandomPointToFollow);
            _timePassedInSameArea = 0;
        }
    }

    public IEnumerator DestroyZombie() 
    {
        isDead = true;
        _animator.SetBool("dead", isDead);
        NavMeshAgent.isStopped = true;
        NavMeshAgent.velocity = Vector3.zero;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private GameObject GetPlayerTarget()
    {
        var target = GameObject.Find("Pinky");
        if (target == null)
        {
            target = GameObject.Find("Rick");
        }
        return target;
    }
    
    protected Vector3 GetRandomPointToFollow()
    {
        Vector3 baseRandomPoint = transform.position + Random.insideUnitSphere * 45f;
        NavMeshHit hit;
        float maxDistance = 100f;
        while (!NavMesh.SamplePosition(baseRandomPoint, out hit, maxDistance, NavMeshAgent.areaMask))
        {
            baseRandomPoint = transform.position + Random.insideUnitSphere * 45f;
        }
        return hit.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.instance.playersTurn && collision.gameObject.CompareTag("Player"))
        {
            _secondsColliding = 0;
            playerTarget.GetComponent<PlayerController>().TakeDamage(damage, transform.position);
            OnAttack();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.instance.playersTurn && collision.gameObject.CompareTag("Player"))
        {
            if (_secondsColliding < secondsToTakeDamage)
            {
                _secondsColliding += Time.deltaTime;
            }
            else
            {
                _secondsColliding = 0;
                playerTarget.GetComponent<PlayerController>().TakeDamage(damage, transform.position);
                OnAttack();
            }
        }
    }
    
    public IEnumerator PlayBreathingSound()
    {
        AudioSource.PlayOneShot(breathings[Random.Range(0, breathings.Length)]);
        yield return new WaitForSeconds(10.0f);
        Breathing = false;
    }

    protected void OnTakeDamage()
    {
        AudioSource.PlayOneShot(damageClip);
    }

    private void OnDie()
    {
        AudioSource.PlayOneShot(dyingClip);
    }

    private void OnAttack()
    {
        if (GameManager.instance.playersTurn)
        {
            AudioSource.PlayOneShot(attackClip);
        }
    }
}
