using UnityEngine;

public class Minimap : MonoBehaviour
{
    private Transform player;

    public void Start()
    {
        player = GetPlayerTarget();
    }
    
    private Transform GetPlayerTarget()
    {
        var target = GameObject.Find("Pinky");
        if (target == null)
        {
            target = GameObject.Find("Rick");
        }
        return target.transform;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
