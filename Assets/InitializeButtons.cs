using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Button[] buttons = GetComponentsInChildren<Button>();

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].name = ""+i+"";
        }

        Debug.Log("I only do this once");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
