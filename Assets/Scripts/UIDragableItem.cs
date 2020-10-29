using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIDragableItem : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public Item item;
    public int amount;
    private Image image;
    private TextMeshProUGUI itemLevel;
    private TextMeshProUGUI itemCount;
    public void SetItem(Item item, int amount = 1)
    {
        this.item = item;
        this.amount = amount;
        image = transform.Find("ItemBackground").gameObject.GetComponent<Image>();
        itemLevel = transform.Find("ItemLevel").GetComponent<TextMeshProUGUI>();
        itemCount = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        image.sprite = item.image;
        itemLevel.SetText(item.level.ToString());
        if (amount > 1)
        {
            itemCount.SetText(amount.ToString());
        }
        else
        {
            itemCount.SetText("");
        }
    }
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .6f;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void OnPlacementConfirm(UIDropReciver reciver)
    {
        
    }
}
