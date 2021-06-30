using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    private EnemyLocomotionManager enemyLocomotion;
    public bool isPerformingAction;

    [Header("A.I Settings")]
    public float detectionRadius = 20;
    //The higher, and lower, respectively these angles are, the greater detection Field of View
    public float maximumDetectionAngle = 50;
    public float minimumDetectionAngle = -50;

    private void Awake()
    {
        enemyLocomotion = GetComponent<EnemyLocomotionManager>();
    }

    private void Update()
    {
        enemyLocomotion.HandleFalling();
    }


    private void FixedUpdate()
    {
        
        HandleCurrentAction();
        


    }

    private void HandleCurrentAction()
    {
        if(enemyLocomotion.currentTarget == null)
        {
            enemyLocomotion.HandleDetection();
        }
        else
        {
            enemyLocomotion.HandleMoveToTarget();
        }

    }

}
