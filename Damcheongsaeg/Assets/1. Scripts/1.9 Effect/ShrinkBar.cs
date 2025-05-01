using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShrinkBar : MonoBehaviour
{
    public GameObject Player { get; private set; }
    public PlayerHealth PlayerHealth { get; private set; }

    [SerializeField] private Transform _damageBarTransform;  // DamageBerImage가 있는 자식 오브젝트의 Transform
    [SerializeField] private Transform _healthBarTransform;  // HpBerImage가 있는 자식 오브젝트의 Transform

    [SerializeField] private float _shrinkSpeed;

    private Image DamageBarImage;
    private Image HealthBarImage;

    private float _shrinkTime;
    private const float DAMAGED_HEALTH_SHRINK_TIMER_MAX = 1f;

    private void Awake()
    {
        HealthBarImage = _healthBarTransform.GetComponent<Image>();
        DamageBarImage = _damageBarTransform.GetComponent<Image>();

        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth = Player.GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        SetHP(PlayerHealth.CurrentHealth, PlayerHealth.MaxHealth);
    }

    private void Update()
    {
        ShrinkBarUpdate();
    }

    private void SetHP(float currentHealth, float maxHealth)
    {
        float normalizedHealth = currentHealth / maxHealth;
        HealthBarImage.fillAmount = normalizedHealth;
        HealthBarImage.fillAmount = DamageBarImage.fillAmount;
    }

    private void ShrinkBarUpdate()
    {
        _shrinkTime -= Time.deltaTime;
        if (_shrinkTime < 0)
        {
            if (HealthBarImage.fillAmount < DamageBarImage.fillAmount)
            {
                float shrinkSpeed = _shrinkSpeed;
                DamageBarImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }

    public void ReduceBar(float currentHealth, float maxHealth)
    {
        _shrinkTime = DAMAGED_HEALTH_SHRINK_TIMER_MAX;
        float normalizedHealth = currentHealth / maxHealth;
        HealthBarImage.fillAmount = normalizedHealth;
    }
}