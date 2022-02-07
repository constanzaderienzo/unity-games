using UnityEngine;

public class Fire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(5f);
        }
    }
}
