using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public bool isDead;

    private Animator animator;
    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage, bool heavy=false)
    {
        currentHealth = currentHealth - damage;

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            currentHealth = 0;
            soundManager.PlaySound(soundManager.sword_slice_meat_02);
            animator.Play("Death_01");
        }
        else if(!isDead)
        {
            if (heavy)
            {
                soundManager.PlaySound(soundManager.sword_slice_metal_01);
            }
            else
            {
                soundManager.PlaySound(soundManager.sword_slice_meat_01);
            }
            
            animator.Play("NPC_Damage_01");
        }
    }
}
