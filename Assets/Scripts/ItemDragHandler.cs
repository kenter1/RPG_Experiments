using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private UIManager uiManager;
    public RectTransform rectTransform;

    public void OnDrag(PointerEventData eventData)
    {
        Image icon = GetComponentInChildren<Image>();

        if (icon.sprite.name != "square")
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;

        int item1 = -1;
        int item2 = -1;

        item1 = int.Parse(eventData.selectedObject.name);

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach(var result in results)
        {
            if (int.TryParse(result.gameObject.name, out _))
            {
                item2 = int.Parse(result.gameObject.name);
                break;
            }
        }

        if(item1 > -1 && item2 > -1)
        {
            uiManager.SwapItemPosition(item1, item2);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
