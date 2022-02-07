using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Weapon in pickup")]
    public string weapon;

    Pickup m_Pickup;

    void Start()
    {
        m_Pickup = GetComponent<Pickup>();

        // Subscribe to pickup action
        m_Pickup.onPick += OnPicked;
    }

    void OnPicked(PlayerController player)
    {
        player.weaponSwitch.AddWeaponAmmo(weapon);
        m_Pickup.PlayPickupFeedback();
        Destroy(gameObject);
    }
}
