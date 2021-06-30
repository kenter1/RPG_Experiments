using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder;

public class BuildInput_Custom : MonoBehaviour
{

    private StatusUI statusUI;
    #region Variables
    
    public string curMode, curPiece, curRotation; // TODO: REMOVE troubleshooting
    [SerializeField]
    public bool buildingNow;   // is player in act of construction?
    [SerializeField]
    public bool creativeMode;
    public bool uiBlocking = false;
    // public bool buildMode; 
    // just use BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement
    public int SelectedIndex { get; set; }
    public float pressTolerance = .5f;
    float WheelAxis;
    public InputAction placeMode, removeMode, selectionMode, confirmAction, cancelAction;
    public InputAction scrollingAction;
    private bool buildPlacementFlag;
    private bool placeMode_Input;
    private bool confirmAction_Input;

    #endregion

    #region Functions

    IEnumerator SlowDownPlacement()
    {

        if (buildPlacementFlag)
        {
            yield return new WaitForSeconds(2f);
            buildPlacementFlag = false;
        }
  
        //Debug.Log("IS this even working?");
    }

    private void LateUpdate()
    {
    }

    private void Awake()
    {
        statusUI = FindObjectOfType<StatusUI>();
    }

    void OnEnable()
    {
        statusUI.SetStatus("Building Mode");
        placeMode.Enable();
        removeMode.Enable();
        selectionMode.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
        scrollingAction.Enable();

        placeMode.performed += i => placeMode_Input = true;
        confirmAction.performed += i => confirmAction_Input = true;

    }

    void OnDisable()
    {
        statusUI.SetStatus("Normal Mode");
        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

        placeMode.Disable();
        removeMode.Disable();
        selectionMode.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
        scrollingAction.Disable();
    }


    private void Update()
    {
        curMode = BuilderBehaviour.Instance.CurrentMode.ToString(); // TODO: REMOVE troubleshooting
        curPiece = BuildManager.Instance.Pieces[SelectedIndex].ToString(); // TODO: REMOVE troubleshooting
        //if (BuilderBehaviour.Instance.SelectedPrefab != null) curRotation = BuilderBehaviour.Instance.SelectedPrefab.RotationAxis.ToString();
        if (statusUI.textComponent.text != "Building Mode: " + curPiece)
        {
            Debug.Log("Changing Modular Item");
            statusUI.SetStatus("Building Mode: " + curPiece);
        }

        if (buildingNow)
        {

            
            if (placeMode_Input)
            {
                placeMode_Input = false;
                BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
            }


            if (removeMode.ReadValue<float>() > pressTolerance) BuilderBehaviour.Instance.ChangeMode(BuildMode.Destruction);
            if (selectionMode.ReadValue<float>() > pressTolerance) BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);  // edition = pick up a current part
            if (CancelInput()) BuilderBehaviour.Instance.ChangeMode(BuildMode.None);

            // Select Prefab (but not while a blueprint is in front of the player
            if (BuilderBehaviour.Instance.CurrentMode != BuildMode.Placement)
            {
                WheelAxis = scrollingAction.ReadValue<float>();
                UpdatePrefabSelection();
            }

            // Rotate blueprint in front of the player
            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
            {
                if (IsPointerOverUIElement())
                {
                    Debug.LogWarning("Attempted to Place Building Over GUI");
                    return;
                }
                if (ConfirmInput())
                {
              
                    BuilderBehaviour.Instance.PlacePrefab();
                    return; 

                }
                WheelAxis = scrollingAction.ReadValue<float>();
                if (WheelAxis > 0)
                {
                    BuilderBehaviour.Instance.RotatePreview(BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                }
                else if (WheelAxis < 0)
                {
                    BuilderBehaviour.Instance.RotatePreview(-BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                }
                //if (cancelAction.ReadValue<float>() > pressTolerance) BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
            }
            else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Edition)
            {
                if (IsPointerOverUIElement())
                {
                    return;
                }

                if (ConfirmInput())
                {
                    BuilderBehaviour.Instance.EditPrefab();
                }

                if (CancelInput())
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                }
            }
            else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Destruction)
            {
                if (IsPointerOverUIElement())
                {
                    return;
                }

                if (ConfirmInput())
                {
                    if (BuilderBehaviour.Instance.CurrentRemovePreview != null)
                    {
                        BuilderBehaviour.Instance.DestroyPrefab();
                    }
                }

                if (CancelInput())
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                }
            }
        }
    }

    private bool ConfirmInput()
    {
        if (confirmAction_Input)
        {
            confirmAction_Input = false;
            return true;
        }
        else
        {
            return false;
        }
            

    }

    private bool CancelInput()
    {
        if (cancelAction.ReadValue<float>() > pressTolerance)
            return true;
        return false;
    }

    private void UpdatePrefabSelection()
    {
        if (WheelAxis > 0)
        {
            if (SelectedIndex < BuildManager.Instance.Pieces.Count - 1)
            {
                SelectedIndex++;
            }
            else
            {
                SelectedIndex = 0;
            }
        }
        else if (WheelAxis < 0)
        {
            if (SelectedIndex > 0)
            {
                SelectedIndex--;
            }
            else
            {
                SelectedIndex = BuildManager.Instance.Pieces.Count - 1;
            }
        }

        if (SelectedIndex == -1)
        {
            return;
        }

        if (BuildManager.Instance.Pieces.Count != 0)
        {
            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.Pieces[SelectedIndex]);
        }
    }



    private bool IsPointerOverUIElement()
    {
        if (!uiBlocking)
        {
            return false;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            return false;
        }

        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData EventData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };

        List<RaycastResult> Results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(EventData, Results);
        return Results.Count > 0;
    }



    #endregion
}
