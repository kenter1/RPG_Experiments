using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float distanceCameraFromLockOnTarget;
    public float newLockedPositionValue;
    private InputManager inputManager;
    private PlayerManager playerManager;

    public Transform targetTransform; //The object the camera will follow
    public Transform cameraPivot; //The object the camera uses to pivot
    public Transform cameraTransform; // The transform of the actual camera object in the scene
    public LayerMask collisionLayers; // The layers we want our camera to collide with
    public LayerMask environmentLayer;

    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffSet = 0.2f; // How much the camera will jump off of objects it colliding with
    public float minimumCollisionOffSet = 0.2f;
    public float cameraCollisionRadius = 0.2f;
    public float cameraFollowSpeed = 0.1f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;

    public float lookAngle; //Camera Looking up and Down
    public float pivotAngle; //Camera looking left and right

    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;
    public float lockedPivotPosition = 2.25f;
    public float unlockedPivotPosition = 1.65f;


    public CharacterManager currentLockOnTarget;
    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockTarget;
    public CharacterManager rightLockTarget;
    public float maximumLockOnDistance = 30;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    private void Start()
    {
        environmentLayer = LayerMask.NameToLayer("Environment");
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        if (!inputManager.rb_Input || !inputManager.rt_Input)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(
            transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

            transform.position = targetPosition;
        }


    }

    private void RotateCamera()
    {
        if (inputManager.lockOnFlag == false && currentLockOnTarget == null)
        {
            Vector3 rotation;
            Quaternion targetRotation;

            lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
            pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

            rotation = Vector3.zero;
            rotation.y = lookAngle;
            targetRotation = Quaternion.Euler(rotation);
            transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotation = Quaternion.Euler(rotation);
            cameraPivot.localRotation = targetRotation;
        }
        else
        {
            float velocity = 0;
            
            Vector3 dir = currentLockOnTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;

            dir = currentLockOnTarget.transform.position - cameraPivot.position;
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            cameraPivot.localEulerAngles = eulerAngle;
         
        }

    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers) )
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffSet);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    
    public void CustomHandleLockOn()
    {
        /*
        availableTargets.Clear();

        float shortestDistance = Mathf.Infinity;
        float shortestDistanceLeftTarget = Mathf.Infinity;
        float shortestDistanceRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 26);

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager charHandler = colliders[i].GetComponent<CharacterManager>();

                if (charHandler != null)
                {
                    Vector3 lockTargetDir = charHandler.transform.position - transform.position;

                    float lockTargetDis = Vector3.Distance(transform.position, charHandler.transform.position);

                    float viewableAngle = Vector3.Angle(lockTargetDir, transform.forward);


                    if (charHandler.transform.root != transform.root  //Make sure we don't lock onto ourselves and the target is on screen and we are close enough to lock on
                        && viewableAngle > -50 && viewableAngle < 50
                        && lockTargetDis <= maximumLockOnDistance)
                    {
                        availableTargets.Add(charHandler);
                    }

                }
            }

            if (availableTargets.Count > 0)
            {
                for (int k = 0; k < availableTargets.Count; k++)
                {
                    float distFromTarget = Vector3.Distance(transform.position, availableTargets[k].transform.position);

                    if (distFromTarget < shortestDistance)
                    {
                        shortestDistance = distFromTarget;
                        nearestLockOnTarget = availableTargets[k].lockOnTransform;
                    }

                    if (inputManager.lockOnFlag)
                    {
                        //This is the big change, from enemy position to player
                        Vector3 relativePlayerPostion = transform.InverseTransformPoint(availableTargets[k].transform.position);

                        var distanceFromLeftTarget = 1000f; //_currentLockOnTarget.transform.position.x - _availableTargets[k].transform.position.x;
                        var distanceFromRightTarget = 1000f; //_currentLockOnTarget.transform.position.x + _availableTargets[k].transform.position.x;

                        if (relativePlayerPostion.x < 0.00)
                        {
                            distanceFromLeftTarget = Vector3.Distance(currentLockOnTarget.position, availableTargets[k].transform.position);
                        }
                        else if (relativePlayerPostion.x > 0.00)
                        {
                            distanceFromRightTarget = Vector3.Distance(currentLockOnTarget.position, availableTargets[k].transform.position);
                        }

                        if (relativePlayerPostion.x < 0.00 && distanceFromLeftTarget < shortestDistanceLeftTarget)
                        {
                            shortestDistanceLeftTarget = distanceFromLeftTarget;
                            leftLockTarget = availableTargets[k].lockOnTransform;
                        }

                        if (relativePlayerPostion.x > 0.00 && distanceFromRightTarget < shortestDistanceRightTarget)
                        {
                            shortestDistanceRightTarget = distanceFromRightTarget;
                            rightLockTarget = availableTargets[k].lockOnTransform;
                        }
                    }
                }

            }
            else
            {
                Debug.Log("no lock on targets found A");
            }
        }
        else
        {
            Debug.Log("no lock on targets found B");
        }
        */
    }


    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        for(int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if(character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                RaycastHit hit;
                if(character.transform.root != targetTransform.transform.root && viewableAngle > -50 
                    && viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance)
                {
                    if(Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                    {
                        Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                        if(hit.transform.gameObject.layer == environmentLayer)
                        {
                            //Cannot Lock On
                        }
                        else
                        {
                            availableTargets.Add(character);
                        }
                    }
                    
                }
            }
        }

        for(int k = 0; k < availableTargets.Count; k++)
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

            if(distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTargets[k];
            }

            if (inputManager.lockOnFlag)
            {
                //Vector3 relativeEnemyPosition = currentLockOnTarget.transform.InverseTransformPoint(availableTargets[k].transform.position);
                //var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                //var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;
                Vector3 relativeEnemyPosition = inputManager.transform.InverseTransformPoint(availableTargets[k].transform.position);
                var distanceFromLeftTarget = relativeEnemyPosition.x;
                var distanceFromRightTarget = relativeEnemyPosition.x;

                if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget 
                    && availableTargets[k] != currentLockOnTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockTarget = availableTargets[k];
                }

                else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget 
                    && availableTargets[k] != currentLockOnTarget) {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockTarget = availableTargets[k];
                }
            }

        }
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        currentLockOnTarget = null;
        nearestLockOnTarget = null;
    }

    public void SetCameraHeight()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
        Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);
        

        if (currentLockOnTarget != null)
        {
            distanceCameraFromLockOnTarget = Vector3.Distance(cameraPivot.transform.position, currentLockOnTarget.transform.position);
            newLockedPositionValue = Mathf.Max((distanceCameraFromLockOnTarget / lockedPivotPosition), 1.5f);

            newLockedPosition = new Vector3(0, newLockedPositionValue);
            cameraPivot.transform.localPosition = Vector3.SmoothDamp(cameraPivot.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
        }
        else
        {
            cameraPivot.transform.localPosition = Vector3.SmoothDamp(cameraPivot.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
        }
    }

}
