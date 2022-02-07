using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public GameObject[] radiusZombies;
    public GameObject[] zombies;
    public GameObject[] bossZombies;
    public GameObject burningZombie, biohazardZombie;
    public GameObject akPickup;
    public GameObject shotgunPickup;
    public GameObject weaponSignal;
    private List<EntryPoint> entryPoints;
    private const int wavesUntilBosses = 5;

    protected class EntryPoint
    {
        public float left, right, forward, behind;

        public EntryPoint(float left, float right, float forward, float behind)
        {
            this.left = left;
            this.right = right;
            this.forward = forward;
            this.behind = behind;
        }
    }

    private void Awake()
    {
        entryPoints = new List<EntryPoint>();
        entryPoints.Add(new EntryPoint(-36f, -39f, 150f, 139f));
        entryPoints.Add(new EntryPoint(-69, -67f, 204f, 208f));
    }

    private void PlaceZombie()
    {
        int entryPoint = Random.Range(0, entryPoints.Count);
        int index = Random.Range(0, zombies.Length);
        GameObject gameObject = zombies[index];
        Vector3 position = new Vector3(Random.Range(entryPoints[entryPoint].left, entryPoints[entryPoint].right), 0f, Random.Range(entryPoints[entryPoint].behind, entryPoints[entryPoint].forward));
        Instantiate(gameObject, position, Quaternion.identity);
    }
    
    private void PlaceRadiusZombie()
    {
        int entryPoint = Random.Range(0, entryPoints.Count);
        int index = Random.Range(0, radiusZombies.Length);
        GameObject gameObject = radiusZombies[index]; 
        Instantiate(gameObject, new Vector3(Random.Range(entryPoints[entryPoint].left, entryPoints[entryPoint].right), 0f, Random.Range(entryPoints[entryPoint].behind, entryPoints[entryPoint].forward)),
            Quaternion.identity);
    }
    
    private void PlaceBossZombie()
    {
        int entryPoint = Random.Range(0, entryPoints.Count);
        int index = Random.Range(0, bossZombies.Length);
        GameObject gameObject = bossZombies[index]; 
        Instantiate(gameObject, new Vector3(Random.Range(entryPoints[entryPoint].left, entryPoints[entryPoint].right), 0f, Random.Range(entryPoints[entryPoint].behind, entryPoints[entryPoint].forward)),
            Quaternion.identity);
    }

    private void PlaceFinalBossZombie(int type)
    {
        GameObject gameObject = type == 0 ? burningZombie : biohazardZombie;
        Vector3 position = type == 0 ? new Vector3(-40f, 0, 185f) : new Vector3(-44f,0,185f); 
        Instantiate(gameObject, position, Quaternion.identity);

    }
    
    public int SetupWave(int wave, int akAppearance, int shotgunAppearance, int burningZombieAppearance, int biohazardZombieAppearance)
    {
        GameManager.instance.hack = -1;
        if (wave == akAppearance)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-68f, -8f ), 0.3f, Random.Range(150f, 200f));
            GameObject signal = Instantiate(weaponSignal, randomPosition, Quaternion.identity);
            StartCoroutine(DestroyWeaponSignal(signal));
            Instantiate(akPickup, randomPosition, Quaternion.identity);
        }
        
        else if (wave == shotgunAppearance)
        {
            Vector3 randomPosition = new Vector3(Random.Range(-68f, -8f ), 0.3f, Random.Range(150f, 200f));
            GameObject signal = Instantiate(weaponSignal, randomPosition, Quaternion.identity);
            StartCoroutine(DestroyWeaponSignal(signal));
            Instantiate(shotgunPickup, randomPosition, Quaternion.identity);
        }

        int zombies = 0;
        
        if (wave == burningZombieAppearance)
        {
            
            PlaceFinalBossZombie(0);
            zombies++;
        }
        else if (wave == biohazardZombieAppearance)
        {
            PlaceFinalBossZombie(1);
            zombies++;
        }

        if (wave > 20 && wave % 10 == 0)
        {
            PlaceFinalBossZombie(0);
            zombies++;
            PlaceFinalBossZombie(1);
            zombies++;
        }

        for (int i = 0; i < wave; i++)
        {
            PlaceZombie();
            zombies++;
            
            PlaceRadiusZombie();
            zombies++;
            
            PlaceZombie();
            zombies++;
            
            PlaceRadiusZombie();
            zombies++;
        }

        if (wave > 0 && wave % wavesUntilBosses == 0)
        {
            for (int i = 0; i < wave / wavesUntilBosses; i++)
            {
                PlaceBossZombie();
                zombies++;
            }
        }

        return zombies;
    }

    private IEnumerator DestroyWeaponSignal(GameObject signal)
    {
        yield return new WaitForSeconds(5f);
        Destroy(signal);
    }
}
