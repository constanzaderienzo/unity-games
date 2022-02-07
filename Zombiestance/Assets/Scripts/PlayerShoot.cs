using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public PlayerWeapon weapon;
    public GameObject[] bulletHole, bulletHoleVehicle;
    public LayerMask mask;

    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }
    }

    public void ChangeWeapon(PlayerWeapon weapon)
    {
        this.weapon = weapon;
    }
    public void Shoot(GameObject muzzleFlash)
    {
        
        if (weapon.ammo > 0 || weapon.name == "Pistol")
        {
            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            RaycastHit hit;
        
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, layerMask))
            {
                muzzleFlash.GetComponent<Animation>().Play();
                
                var hitRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                if (hit.transform.tag == "Vehicle")
                {
                    Instantiate(bulletHoleVehicle[Random.Range(0, bulletHoleVehicle.Length)], hit.point, hitRotation);
                }
                else if(hit.transform.tag == "Wood")
                {
                    Instantiate(bulletHole[Random.Range(0, bulletHole.Length)], hit.point, hitRotation);
                }
                
                Debug.DrawRay(cam.transform.position, cam.transform.forward * weapon.range, Color.yellow);
                BaseZombie zombie = hit.transform.GetComponent<BaseZombie>();
                if (zombie != null)
                {
                    zombie.TakeDamage(weapon.damage);
                    // Debug.Log("Zombie hit!");
                }
            }

            weapon.ammo--; 
        }
    }
}