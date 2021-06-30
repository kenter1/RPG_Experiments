using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    private EnemyLocomotionManager enemyLocomotionManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyLocomotionManager.enemyRigidBody.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        enemyLocomotionManager.enemyRigidBody.velocity = velocity;
    }
}
