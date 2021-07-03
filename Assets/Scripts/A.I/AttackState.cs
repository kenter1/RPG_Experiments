using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public CombatStanceState combatStanceState;
    
    public EnemyAttackAction[] enemyAttacks;
    public EnemyAttackAction currentAttack;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        //Select one of our many attacks based on attack scores
        //if the selected attack is not able to be used because of bad angle or distance, select a new attack
        //if the attack is viable, stop our movement and attack our target
        //set our recovery timerr to the attacks recovery timee
        //return to the combat stance state
        Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
        enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);
        enemyManager.viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        HandleRotateTowardsTarget(enemyManager);

        Debug.Log("Goal: Attacking Target");
        if (enemyManager.isPerformingAction)
        {
            return combatStanceState;
        }

        if(currentAttack != null)
        {
            //If we are too close to the enemy to perform attack, get a new attack
            if(enemyManager.distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
            {
                return this;
            }
            //if we are close enough to attack, then let us proceed
            else if(enemyManager.distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
            {
                //if our enemy is within our attacks viewable angle, we attack
                if(enemyManager.viewableAngle < currentAttack.maximumAttackAngle &&
                    enemyManager.viewableAngle >= currentAttack.minimumAttackAngle)
                {
                    if(enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
                    {
                        enemyAnimatorManager.animator.SetFloat("Vertical", 0);
                        enemyAnimatorManager.animator.SetFloat("Horizontal", 0);
                        enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                        enemyManager.isPerformingAction = true;
                        enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                        currentAttack = null;
                        return combatStanceState;
                    }
                }
            }
        }
        else
        {
            GetNewAttack(enemyManager);
        }


        return combatStanceState;   
    }


    private void GetNewAttack(EnemyManager enemyManager)
    {
        Vector3 targetsDirection = enemyManager.currentTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetsDirection, transform.forward);
        enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

        int maxScore = 0;
        
        for(int i = 0;  i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];
            
            if(enemyManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                enemyManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
 
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle &&
                    viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    maxScore += enemyAttackAction.attackScore;
                }
            }
        }


        int randomValue = Random.Range(0, maxScore);
        int temporaryScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (enemyManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack &&
                enemyManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle &&
                    viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    if(currentAttack != null)
                    {
                        return;
                    }

                    temporaryScore += enemyAttackAction.attackScore;

                    if(temporaryScore > randomValue)
                    {
                        currentAttack = enemyAttackAction;
                    }
                }
            }
        }
    }

    public void HandleRotateTowardsTarget(EnemyManager enemyManager)
    {
        //rotate manually
        if (enemyManager.isPerformingAction || enemyManager.distanceFromTarget < enemyManager.maximumAttackRange + 3)
        {
            Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = enemyManager.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
        else // rotate with path finding (navmesh)
        {
            Debug.Log("Rotating Towards player");
            Vector3 relativeDirection = enemyManager.transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }

    }

}
