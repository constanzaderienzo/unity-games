using UnityEngine;

public class BloodSplatterController : MonoBehaviour
{

    private void Start()
    {
        Destroy(gameObject, 3);
    }

    private void OnCollisionEnter()
    {
        Destroy(gameObject);
    }
}