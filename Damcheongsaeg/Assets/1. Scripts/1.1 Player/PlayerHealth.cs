using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageble
{
    public PlayerMovement PlayerMovement { get; private set; }
    public GameStop GameStop { get; private set; }
    public Knockback Knockback { get; private set; }

    public GameObject GameObjectShrinkBar { get; private set; } 
    public ShrinkBar ShrinkBar { get; private set; } 

    [field: SerializeField] public float CurrentHealth { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }

    private void Awake()
    {
        Knockback = GetComponent<Knockback>();
        PlayerMovement = GetComponent<PlayerMovement>();
        GameStop = GetComponent<GameStop>();

        GameObjectShrinkBar = GameObject.FindGameObjectWithTag("SHRINKBAR");
        ShrinkBar = GameObjectShrinkBar.GetComponent<ShrinkBar>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damageAmount, Vector2 hitDirection)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0f)
        {
            Die();
        }

        ShrinkBar.ReduceBar(CurrentHealth, MaxHealth);
        GameStop.Sleep();
        Knockback.StartKnockback(hitDirection, Vector2.up.normalized, PlayerMovement._moveInput.x);
    }

    public void Die()
    {
        
    }

    public void DamageFlashing() { }
}
