using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Weapon in pickup")]
    public PlayerWeapon weapon;

    Pickup m_Pickup;

    void Start()
    {
        m_Pickup = GetComponent<Pickup>();

        // Subscribe to pickup action
        m_Pickup.onPick += OnPicked;
    }

    void OnPicked(PlayerController player)
    {
        player.weaponSwitch.AddWeapon(weapon);
        m_Pickup.PlayPickupFeedback();
        Destroy(gameObject);
    }
}