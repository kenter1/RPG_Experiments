using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private BoxCollider damageCollider;
    private PlayerAttacker playerAttacker;

    public int currentWeaponDamage = 25;

    private void Awake()
    {
        playerAttacker = FindObjectOfType<PlayerAttacker>();
        damageCollider = GetComponent<BoxCollider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }

        if(collision.tag == "Enemy")
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                bool heavy = playerAttacker.lastAttack.ToLower().Contains("heavy");
                Debug.Log("Hit: " + playerAttacker.lastAttack.ToLower());
                enemyStats.TakeDamage(currentWeaponDamage, heavy);
            }
        }

        if(collision.tag == "DestroyableBuilding")
        {
            WallBuilding wallBuilding = collision.GetComponent<WallBuilding>();

            if(wallBuilding != null)
            {
                wallBuilding.TakeDamage(currentWeaponDamage / 4);
            }
        }
    }
}
