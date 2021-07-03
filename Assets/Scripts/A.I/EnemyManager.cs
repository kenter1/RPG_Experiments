using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{
    private EnemyLocomotionManager enemyLocomotion;
    private EnemyAnimatorManager enemyAnimatorManager;
    private EnemyStats enemyStats;

    public Rigidbody enemyRigidBody;
    public NavMeshAgent navMeshAgent;
    public CharacterStats currentTarget;
    public State currentState;
    public float distanceFromTarget;
    public float rotationSpeed = 15;
    public bool isGrounded;
    public float maximumAttackRange = 1.5f;

    public bool isPerformingAction;
    public bool isInteracting;

    [Header("A.I Settings")]
    public float detectionRadius = 20;
    //The higher, and lower, respectively these angles are, the greater detection Field of View
    public float maximumDetectionAngle = 50;
    public float minimumDetectionAngle = -50;
    public float viewableAngle;
    public float currentRecoveryTime = 0;

    private void Awake()
    {
        enemyLocomotion = GetComponent<EnemyLocomotionManager>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidBody = GetComponent<Rigidbody>();
        navMeshAgent.enabled = false;
    }
    private void Start()
    {
        enemyRigidBody.isKinematic = false;
    }

    private void Update()
    {
        enemyLocomotion.HandleFalling();
        HandleRecoveryTimer();
        isInteracting = enemyAnimatorManager.animator.GetBool("isInteracting");
    }


    private void FixedUpdate()
    {
        
        HandleStateMachine();
        


    }

    private void HandleStateMachine()
    {
        if(currentState != null)
        {
            State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);

            if(nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }

    }

    private void SwitchToNextState(State state)
    {
        currentState = state;
    }

    private void HandleRecoveryTimer()
    {
        if(currentRecoveryTime  > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPerformingAction)
        {
            if(currentRecoveryTime <= 0)
            {
                isPerformingAction = false;
            }
        }
    }
    

    
}
