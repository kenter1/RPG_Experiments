using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public HealthBar healthBar;
    public HealthBar staminaBar;

    float lerpSpeed;
    public float lerpValue = 0.00015f;

    private PlayerAnimatorManager animatorManager;

    private void Awake()
    {
        animatorManager = GetComponentInChildren<PlayerAnimatorManager>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
        healthBar.SetMaxBarValue(maxHealth);

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;
        staminaBar.SetMaxBarValue(maxStamina);
    }

    private void FixedUpdate()
    {
        lerpSpeed = lerpValue * Time.deltaTime;

        if (Mathf.RoundToInt(healthBar.slider.value) != currentHealth)
        {
            healthBar.SetCurrentBarValue((int)Mathf.Floor(Mathf.Lerp(healthBar.slider.value, currentHealth, lerpSpeed)));
        }

        if (Mathf.RoundToInt(staminaBar.slider.value) != currentStamina)
        {
            staminaBar.SetCurrentBarValue((int)Mathf.Floor(Mathf.Lerp(staminaBar.slider.value, currentStamina, lerpSpeed)));
        }

            

        //print(healthBar.slider.value);
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    private int SetMaxStaminaFromStaminaLevel()
    {
        maxStamina = staminaLevel * 10;
        return maxStamina;
    }


    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            animatorManager.PlayTargetAnimation("Death_01", true, true);
        }
        else
        {
            animatorManager.PlayTargetAnimation("Damage_01", true, true);
        }
    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = currentStamina - damage;
    }
}
