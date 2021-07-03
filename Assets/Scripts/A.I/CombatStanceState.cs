using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PursueTargetState pursueTargetState;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {


        enemyManager.distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        HandleRotateTowardsTarget(enemyManager);
        //potentially circle player or walk around them

        //Check for attack range
        if (enemyManager.distanceFromTarget > enemyManager.maximumAttackRange || PlayerBehindAI(enemyManager))
        {
            return pursueTargetState;
        }
        else if (enemyManager.currentRecoveryTime <= 0 && enemyManager.distanceFromTarget <= enemyManager.maximumAttackRange)
        {
            return attackState;
        }
        else         
        {
            return this;
        }
        
    }

    private bool PlayerBehindAI(EnemyManager enemyManager)
    {
        return !(Vector3.Dot(Vector3.forward, enemyManager.transform.InverseTransformPoint(enemyManager.currentTarget.transform.position)) > 0);
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
