using UnityEngine;
using UnityEngine.AI;

public class RadiusZombie : BaseZombie
{
    private bool _wasFollowingPlayer;

    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        _wasFollowingPlayer = false;
        LastRandomPointToFollow = transform.position;
        AudioSource = GetComponent<AudioSource>();
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
    }
}
