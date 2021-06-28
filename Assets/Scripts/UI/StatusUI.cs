using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class StatusUI : MonoBehaviour
{
    public TMP_Text textComponent;

    // Start is called before the first frame update
    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        textComponent.text = "Normal Mode";
    }

    public void EnableStatus()
    {
        textComponent.enabled = true;
    }

    public void DisableStatus()
    {
        textComponent.enabled = false;
    }

    public void SetStatus(string text)
    {
        textComponent.text = text;
    }
}
