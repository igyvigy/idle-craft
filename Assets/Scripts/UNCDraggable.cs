using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UNCDraggable : MonoBehaviour,
IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image ghost;
    void Awake()
    {
        ghost.raycastTarget = false;
        ghost.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ghost.transform.position = transform.position;
        ghost.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ghost.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ghost.enabled = false;
    }

    public void OnDrop(PointerEventData data)
    {
        GameObject fromItem = data.pointerDrag;
        if (data.pointerDrag == null) return; 

        UNCDraggable d = fromItem.GetComponent<UNCDraggable>();
        if (d == null)
        {
            return;
        }
    }
}