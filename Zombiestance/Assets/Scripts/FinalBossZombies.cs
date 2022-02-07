using UnityEngine;
using UnityEngine.AI;

public class FinalBossZombies : BaseZombie
{
    private bool _wasFollowingPlayer;
    public bool indestructible;
    public SpriteRenderer healthShieldSprite;
    private bool _fadingOff;
    private readonly Color _transparentColor = new Color(255f, 255f, 255f, 0f);
    private readonly Color _whiteColor = new Color(255f, 255f, 255f, 255f);

    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        _wasFollowingPlayer = false;
        LastRandomPointToFollow = transform.position;
        AudioSource = GetComponent<AudioSource>();
        indestructible = true;
        _fadingOff = false;
    }
    
    void Update()
    {
        if(!GameManager.instance.playersTurn)
        {
            NavMeshAgent.isStopped = true;
            return;
        }
        
        NavMeshAgent.isStopped = false;
        
        if (Vector3.Distance(transform.position, playerTarget.transform.position) <= minRadiusToFollowTarget)
        {
            NavMeshAgent.SetDestination(playerTarget.transform.position);
            _wasFollowingPlayer = true;
        }
        else if (_wasFollowingPlayer || Vector3.Distance(transform.position, LastRandomPointToFollow) < 5f)
        {
            LastRandomPointToFollow = GetRandomPointToFollow();
            NavMeshAgent.SetDestination(LastRandomPointToFollow);
            _wasFollowingPlayer = false;
        }

        if (!_wasFollowingPlayer)
        {
            RefreshDestToAvoidInPlaceWalkingIfNeeded();
        }
        
        if (!Breathing)
        {
            Breathing = true;
            StartCoroutine(PlayBreathingSound());
        }

        if (_fadingOff)
        {
            healthShieldSprite.color = Color.Lerp(healthShieldSprite.color, _transparentColor, 10 * Time.deltaTime);
        }
    }

    public override void TakeDamage(float amount)
    {
        // Debug.Log("Overrides");
        if (!indestructible)
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
                Vector3 bloodPosition = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
                Instantiate(bloodSplatterPrefab, bloodPosition, Quaternion.identity);
            }
        }
        else
        {
            _fadingOff = true;
            healthShieldSprite.color = _whiteColor;
        }
    }

    public void SetDestructibilityAs(bool isDestructible)
    {
        if (isDestructible)
        {
            healthShieldSprite.color = _transparentColor;
            _fadingOff = false;
        }
        indestructible = !isDestructible;
    }
}
