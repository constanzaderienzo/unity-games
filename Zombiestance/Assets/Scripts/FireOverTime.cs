using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOverTime : MonoBehaviour
{
    public GameObject[] fires;

    private List<Vector3> fireSizes; 
    // Start is called before the first frame update
    void Start()
    {
        fireSizes = new List<Vector3>();
        foreach (GameObject fire in fires)
        {
            fireSizes.Add(fire.transform.localScale);
        }
        
        InvokeRepeating("DieDown", 40, 60);
    }
    
    void DieDown()
    {
        // Debug.Log("Dying down");
        foreach (GameObject fire in fires)
        {
            fire.SetActive(false);
            transform.GetComponent<FinalBossZombies>().SetDestructibilityAs(true);
            //fire.transform.localScale = Vector3.zero;
        }

        StartCoroutine("Rekindle");
    }
    
    IEnumerator Rekindle()
    {
        // Debug.Log("Rekindling");

        yield return new WaitForSeconds(20);
        foreach (GameObject fire in fires)
        {
            fire.SetActive(true);
            transform.GetComponent<FinalBossZombies>().SetDestructibilityAs(false);
        }
        
    }
    
}
