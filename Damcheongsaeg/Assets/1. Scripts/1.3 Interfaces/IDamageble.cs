using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageble
{
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }

    public void Damage(float damageAmount, Vector2 hitDirection);   
    public void Die();

    public void DamageFlashing();
}
