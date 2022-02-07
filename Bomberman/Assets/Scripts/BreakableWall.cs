using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Break()
    {
        StartCoroutine(DestroyBrick());
    }

    public IEnumerator DestroyBrick() 
    {
        animator.SetBool("Break", true);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
