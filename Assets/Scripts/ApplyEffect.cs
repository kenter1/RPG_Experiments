using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyEffect : MonoBehaviour
{
    public GameObject effect1;
    public GameObject effect2;
    public GameObject effect3;
    public GameObject effect4;

    public PSMeshRendererUpdater psUpdater;
    bool effectFlag;

    public void ApplyOnWeapon(GameObject meshObject)
    {
        GameObject currentInstance = Instantiate(effect2, meshObject.transform, false);
    
        currentInstance.transform.SetParent(meshObject.transform);

        psUpdater = currentInstance.GetComponent<PSMeshRendererUpdater>();
        psUpdater.MeshObject = meshObject;
        //psUpdater.StartScaleMultiplier = 0.5f;
        psUpdater.UpdateMeshEffect(meshObject);
        effectFlag = true;
    }

    public void ToggleEffect()
    {
        if(psUpdater != null)
        {
            effectFlag = !effectFlag;

            if (effectFlag)
            {
                psUpdater.IsActive = true;
            }
            else
            {
                psUpdater.IsActive = false;
            }
        }

    }
}
