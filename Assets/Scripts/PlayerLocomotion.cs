using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerManager playerManager;
    private AnimatorManager animatorManager;
    private InputManager inputManager;

    private Vector3 moveDirection;
    private Transform cameraObject;
    public Rigidbody playerRigidBody;

    [Header("Falling")]
    public float inAirTimer;
    public float fallingVelocity = 45;
    public float leapingVelocity = 3;
    public float rayCastHeightOffSet = 0.5f;
    public LayerMask groundLayer;

    [Header("Ground & Air Detection Stats")]
    [SerializeField]
    private float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    private float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    private float groundDirectionRayDistance = 0.2f;
    public LayerMask ignoreForGroundCheck;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;
    public bool isInAir;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotationSpeed = 15;

    [Header("Jump Speeds")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;


    Vector3 normalVector;
    Vector3 targetPosition;


    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidBody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;

        isGrounded = true;
        //ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
        {
            return;
        }

        if (isJumping || !isGrounded)
        {
            return;
        }
        HandleMovement();
        HandleRotation();
    }
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput; //Movement Input
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;

        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }


        Vector3 movementVelocity = moveDirection;
        playerRigidBody.velocity = movementVelocity;

    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }


    private void HandleFalling()
    {
        isGrounded = false;
        RaycastHit hit;
        Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        if(Physics.Raycast(origin, transform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (isInAir)
        {
            playerRigidBody.AddForce(-Vector3.up * fallingVelocity);
            playerRigidBody.AddForce(moveDirection * fallingVelocity / 5f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = transform.position;

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if(Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            isGrounded = true;
            targetPosition.y = tp.y;

            if (isInAir)
            {
                if(inAirTimer > 0.5f)
                {
                    animatorManager.PlayTargetAnimation("Land", true);
                    inAirTimer = 0;

                }
                else
                {
                    animatorManager.PlayTargetAnimation("Empty", false);
                    inAirTimer = 0;
                }

                isInAir = false;
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
            }

            if(isInAir == false)
            {
                if(playerManager.isInteracting == false)
                {
                    animatorManager.PlayTargetAnimation("Falling", true);
                }
                Vector3 vel = playerRigidBody.velocity;
                vel.Normalize();
                playerRigidBody.velocity = vel * (runningSpeed / 2);
                isInAir = true;
            }
        }

        if (isGrounded)
        {
            if(playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidBody.AddForce(transform.forward * leapingVelocity);
            playerRigidBody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        //if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, groundLayer))
        if (Physics.Raycast(rayCastOrigin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Landing", true, true);
            }
        
            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            
            isGrounded = true;
   
        }
        else
        {
            isGrounded = false;
        }

        if(isGrounded && !isJumping)
        {
            if(playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }


    }

    public void HandleJumping()
    {
        if (isGrounded && !playerManager.isUsingRootMotion && !playerManager.isInteracting)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false, false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidBody.velocity = playerVelocity;
        }
    }

    public void HandleDodge()
    {
        if (playerManager.isInteracting || !isGrounded)
        {
            return;
        }

        animatorManager.PlayTargetAnimation("Dodge", true, true);
        //Toggle Invulnerable bool for no hp damage during animation
    }

    public void HandleRolling()
    {
        if (playerManager.isInteracting || !isGrounded)
        {
            return;
        }

        animatorManager.PlayTargetAnimation("Rolling", true, true);
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;

        moveDirection.y = 0;
        Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = rollRotation;
    }
}
