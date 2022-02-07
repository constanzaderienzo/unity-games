using UnityEngine;
using UnityEngine.AI;

public class Zombie : BaseZombie
{
    private bool _followingPlayer;
    
    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        _followingPlayer = false;
        LastRandomPointToFollow = transform.position;
    }
    
    void Update()
    {
        if(!GameManager.instance.playersTurn)
        {
            NavMeshAgent.isStopped = true;
            return;
        }
        
        NavMeshAgent.isStopped = false;
        
        if (MustFollowPlayer())
        {
            NavMeshAgent.SetDestination(playerTarget.transform.position);
            _followingPlayer = true;
        }
        else if (Vector3.Distance(transform.position, LastRandomPointToFollow) < 5f)
        {
            LastRandomPointToFollow = GetRandomPointToFollow();
            NavMeshAgent.SetDestination(LastRandomPointToFollow);
        }
        
        if (!_followingPlayer)
        {
            RefreshDestToAvoidInPlaceWalkingIfNeeded();
        }
        
        if (!Breathing)
        {
            Breathing = true;
            StartCoroutine(PlayBreathingSound());
        }
    }

    private bool MustFollowPlayer()
    {
        return _followingPlayer 
               || (Vector3.Distance(transform.position, playerTarget.transform.position) <= minRadiusToFollowTarget 
                   && NothingBetweenPlayerAndZombie());
    }

    private bool NothingBetweenPlayerAndZombie()
    {
        return Physics.Linecast(transform.position + Vector3.up, playerTarget.transform.position + Vector3.up, out var raycastHit) 
               && raycastHit.collider.gameObject.CompareTag("Player");;
    }
}
