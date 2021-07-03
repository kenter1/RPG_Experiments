using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionManager : MonoBehaviour
{
    private EnemyManager enemyManager;
    private EnemyAnimatorManager enemyAnimatorManager;
    public LayerMask ignoreForGroundCheck;
    Vector3 direction;
    public bool isInAir;
    public float inAirTimer = 0;





    [Header("Enemy Step Climb")]
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private GameObject stepRayLower;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
       
        

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    private void Start()
    {

        
    }



    


    public void HandleFalling()
    {
        float rayCastHeightOffSet = 0.7f;
        int leapingVelocity = 7;
        int fallingVelocity = 50;
        float minimumDistanceNeededToBeginFall = 1.5f;


        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        targetPosition = transform.position;

        if (!enemyManager.isGrounded)
        {
            
  
            enemyAnimatorManager.PlayTargetAnimation("Falling", true);

            enemyAnimatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer = inAirTimer + Time.deltaTime;
            enemyManager.enemyRigidBody.AddForce(transform.forward * leapingVelocity);
            enemyManager.enemyRigidBody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        Debug.DrawRay(rayCastOrigin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, true);
        //if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, groundLayer))
        if (Physics.Raycast(rayCastOrigin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            if (!enemyManager.isGrounded)
            {
                enemyAnimatorManager.PlayTargetAnimation("Landing", true, true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;

            enemyManager.isGrounded = true;
        }
        else
        {
            enemyManager.isGrounded = false;


        }

        if (enemyManager.isGrounded)
        {
            transform.position = targetPosition;

        }


    }


}
