using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyBuildSystem.Addons;


public class CoroutineManager : MonoBehaviour
{
    private Coroutine instantReturn;
    private bool toggleCollider;
    public bool isSomethingDestroyed;

    private PlayerLocomotion playerLocomotion;
    private AddonNavMesh addonNavMesh;

    private void Awake()
    {
        addonNavMesh = FindObjectOfType<AddonNavMesh>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void LateUpdate()
    {
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {

        if (playerLocomotion.isGrounded == false && toggleCollider == false)
        {
            playerLocomotion.LongCollider();
            toggleCollider = true;
            yield return new WaitForSeconds(3.0f);
            toggleCollider = false;
        }
        else if (playerLocomotion.isGrounded == true && toggleCollider == false)
        {
            playerLocomotion.ShortCollider();
            toggleCollider = true;
            yield return new WaitForSeconds(0.1f);
            toggleCollider = false;
        }

        if (isSomethingDestroyed == true)
        {
            Debug.Log("Will Rebuild Nav Mesh");
            isSomethingDestroyed = false;
            yield return new WaitForSeconds(3.0f);
            Debug.Log("Building Now");
            addonNavMesh.UpdateMeshData();

        }

        yield return null;
        //Debug.Log("IS this even working?");
    }
}
