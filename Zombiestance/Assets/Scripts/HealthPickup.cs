﻿using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Parameters")]
    [Tooltip("Amount of health to heal on pickup")]
    public float healAmount;

    Pickup m_Pickup;

    void Start()
    {
        m_Pickup = GetComponent<Pickup>();

        // Subscribe to pickup action
        m_Pickup.onPick += OnPicked;
    }

    void OnPicked(PlayerController player)
    {
        float playerHealth = player.health;
        
        if (playerHealth <= 100f)
        {
            player.AddLife(healAmount);

            m_Pickup.PlayPickupFeedback();

            Destroy(gameObject);
        }
    }
}