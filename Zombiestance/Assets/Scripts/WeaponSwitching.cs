using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    private PlayerShoot _shoot;
    public List<PlayerWeapon> weapons;
    public AudioClip[] shootingClips;
    public AudioClip emptyClipClip;
    public GameObject[] muzzleFlashByWeapon;
    private Text ammoText;

    
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find("Pinky");
        if (player == null)
        {
            player = GameObject.Find("Rick");
        }
        _shoot = player.GetComponent<PlayerShoot>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
        weapons.Add(new PlayerWeapon("Pistol", 20f, 50f));
        SelectWeapon();
    }

    public AudioClip GetCurrentWeaponShootingClip()
    {

        PlayerWeapon currentWeapon = weapons[selectedWeapon];
        if (currentWeapon.name != "Pistol" && currentWeapon.ammo <= 0)
        {
            return emptyClipClip;
        }
        return shootingClips[selectedWeapon];
    }

    public GameObject GetCurrentWeaponMuzzleFlash()
    {
        return muzzleFlashByWeapon[selectedWeapon];
    }

    public void AddWeapon(PlayerWeapon weapon)
    {
        bool hasWeapon = false;
        foreach (var currentWeapon in weapons)
        {
            if (currentWeapon.name == weapon.name)
            {
                // Debug.Log("Already had weapon... adding ammo");
                AddWeaponAmmo(weapon.name);
                hasWeapon = true;
            }
        }
        if(!hasWeapon)
        {
            // Debug.Log("Weapon added " + weapon.name);
            weapons.Add(weapon); 
        }

    }

    public void AddWeaponAmmo(string weapon)
    {
        bool hasWeapon = false;
        foreach (var currentWeapon in weapons)
        {
            if (currentWeapon.name == weapon)
            {
                hasWeapon = true;
                currentWeapon.ammo += 20;
            }
        }

        if (!hasWeapon)
        {
            if (weapon == "AK47")
            {
                AddWeapon(new PlayerWeapon("AK47", 50, 50, 60));
            }
            else if (weapon == "Shotgun")
            {
                AddWeapon(new PlayerWeapon("Shotgun", 100, 5 , 30));
            }
        }
    }
    
    public void UpdateAmmoInfo()
    {
        PlayerWeapon currentWeapon = weapons[selectedWeapon];
        if (currentWeapon.name == "Pistol")
        {
            ammoText.text = "\u221E";
        }
        else
        {
            if (currentWeapon.ammo <= 0)
                ammoText.text = "<color=red>" + currentWeapon.ammo.ToString() + "</color>";
            else
                ammoText.text = currentWeapon.ammo.ToString();
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                _shoot.ChangeWeapon(weapons[i]);
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
        {
            selectedWeapon = 3;
        }

        if (previousSelectedWeapon != selectedWeapon && selectedWeapon < weapons.Count) 
        {
            //Debug.Log(weapons.Count);
            SelectWeapon();
        }
        else
        {
            selectedWeapon = previousSelectedWeapon;
        }
        UpdateAmmoInfo();
    }
}
