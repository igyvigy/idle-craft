using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWindowHeader : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private RectTransform contentRect;
    [SerializeField]
    private Canvas canvas;
    private RectTransform rectTransform;
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)eventData.delta / canvas.scaleFactor;
        contentRect.position += (Vector3)eventData.delta / canvas.scaleFactor;

    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {

    }
}
