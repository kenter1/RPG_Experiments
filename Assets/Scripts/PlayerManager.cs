using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    private Animator animator;
    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;
    private CameraManager cameraManager;

    private InteractableUI interactableUI;

    public GameObject interactableUIGameObject;
    public GameObject itemInteractableGameObject;

    public Collider playerCollider;


    [Header("Player Flags")]
    public bool isInteracting;
    public bool isUsingRootMotion;
    public bool canDoCombo;
    public bool isUsingRightHand;
    public bool isUsingLeftHand;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerCollider = GetComponent<CapsuleCollider>();
        interactableUI = FindObjectOfType<InteractableUI>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        canDoCombo = animator.GetBool("canDoCombo");
        CheckForInteractableObject();
        //Temporary
        isUsingRightHand = true;//animator.GetBool("isUsingRightHand");
        //isUsingLeftHand = animator.GetBool("isUsingLeftHand");
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

        isInteracting = animator.GetBool("isInteracting");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);

        inputManager.quickslot1_Input = false;
        inputManager.quickslot2_Input = false;
        inputManager.quickslot3_Input = false;
        inputManager.quickslot4_Input = false;
        inputManager.a_Input = false;

        inputManager.inventory_Input = false;
        inputManager.options_Input = false;
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f))
        {
            if(hit.collider.tag == "Interactable")
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                if(interactableObject != null)
                {
                    //Set the ui text to the interactable object's text
                    //Set the text pop up to true
                    string interactableText = interactableObject.interactableText;
                    interactableUI.interactableText.text = interactableText;
                    interactableUIGameObject.SetActive(true);


                    if (inputManager.a_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
      
            }
        }
        else
        {
            if (interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }

            if(itemInteractableGameObject != null && inputManager.a_Input)
            {
                itemInteractableGameObject.SetActive(false);
            }
        }

       
    }

  
}
