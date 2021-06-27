using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyEffect : MonoBehaviour
{
    public GameObject effect1;
    public GameObject effect2;
    public GameObject effect3;
    public GameObject effect4;

    private SoundManager soundManager;

    public PSMeshRendererUpdater psUpdater;
    bool effectFlag;

    public void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void ApplyOnWeapon(GameObject meshObject)
    {
        GameObject currentInstance = Instantiate(effect2, meshObject.transform, false);
    
        currentInstance.transform.SetParent(meshObject.transform);

        psUpdater = currentInstance.GetComponent<PSMeshRendererUpdater>();
        psUpdater.MeshObject = meshObject;
        //psUpdater.StartScaleMultiplier = 0.5f;
        psUpdater.UpdateMeshEffect(meshObject);
        effectFlag = true;
        soundManager.PlaySound(soundManager.electric_effect_01);
    }

    public void ToggleEffect()
    {
        if(psUpdater != null)
        {
            effectFlag = !effectFlag;

            if (effectFlag)
            {
                psUpdater.IsActive = true;
                soundManager.PlaySound(soundManager.electric_effect_01);
            }
            else
            {
                psUpdater.IsActive = false;
            }
        }

    }
}
