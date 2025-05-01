using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageble
{
    public DamageFlash DamageFlash { get; private set; }

    // [field: SerializeField]: �̺�Ʈ�� �븮�ڿ� ���� ����� ���
    [field: SerializeField] public float CurrentHealth { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }

    private void Awake()
    {
        DamageFlash = GetComponent<DamageFlash>();
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damageAmount, Vector2 direction)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        Debug.Log("����");
    }

    public void DamageFlashing()
    {
        DamageFlash.CallDamageFlash();
    }
}
