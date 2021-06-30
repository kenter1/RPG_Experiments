using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyBuildSystem.Addons;

public class WallBuilding : BuildingItem
{

    private CoroutineManager coroutineManager;
    public int healthLevel = 10;
    private int maxHealth;
    public int currentHealth;
    private SoundManager soundManager;

    private int SetMaxHealthFromHealthLevel()
    {
        int maxHealth = healthLevel * 10;
        return maxHealth;
    }

    private void Awake()
    {
        coroutineManager = FindObjectOfType<CoroutineManager>();
        soundManager = FindObjectOfType<SoundManager>();
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        soundManager.PlaySound(soundManager.wall_hit_01);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Destroy Building
            Destroy(transform.parent.gameObject);
            coroutineManager.isSomethingDestroyed = true;
        }
        else
        {
            //Take Building Damage
            
        }
    }


}
