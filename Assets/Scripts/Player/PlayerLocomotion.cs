using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerLocomotion : MonoBehaviour
{
    AddonNavMesh addonNavMesh;
    private PlayerManager playerManager;
    private PlayerAnimatorManager animatorManager;
    private InputManager inputManager;
    private PlayerAttacker playerAttacker;
    private CameraManager cameraManager;

    private Vector3 moveDirection;
    private Transform cameraObject;
    private CapsuleCollider capsuleCollider;

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
    public bool rollingFlag;


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
        
        cameraManager = FindObjectOfType<CameraManager>();
        playerAttacker = GetComponent<PlayerAttacker>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<PlayerAnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        cameraObject = Camera.main.transform;

        isGrounded = true;
        //ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        
    }


    public void LongCollider()
    {
        capsuleCollider.height = 1.7f;
        capsuleCollider.center = new Vector3(0, 0.922716f, 0);
    }

    public void ShortCollider()
    {
        capsuleCollider.center = new Vector3(0, 1.146192f, 0);
        capsuleCollider.height = 1.3f;
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

        if (isGrounded)
        {
            ShortCollider();
        }

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
        if (inputManager.lockOnFlag)
        {
            if(isSprinting || rollingFlag)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = cameraManager.cameraTransform.forward * inputManager.verticalInput;
                targetDirection += cameraManager.cameraTransform.right * inputManager.horizontalInput;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 rotationDirection = moveDirection;
                rotationDirection = cameraManager.currentLockOnTarget.transform.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();

                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
            
        }
        else
        {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = cameraObject.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }


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
                    animatorManager.PlayTargetAnimation("Landing", true);
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

        Debug.DrawRay(rayCastOrigin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, true);
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

    public void OpenRolling()
    {
        rollingFlag = true;
    }

    public void CloseRolling()
    {
        rollingFlag = false;
    }

    public void HandleDodge()
    {
        if (!isGrounded)
        {
            return;
        }
        animatorManager.PlayTargetAnimation("Dodge", true, true);
        //Toggle Invulnerable bool for no hp damage during animation
    }

    public void HandleRolling()
    {
        if (!isGrounded || rollingFlag)
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
