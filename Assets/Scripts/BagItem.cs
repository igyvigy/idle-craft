using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BagItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public event Action<BagItem, PointerEventData> OnBeginDrag = delegate { };
    public event Action<BagItem, PointerEventData> OnEndDrag = delegate { };
    public event Action<BagItem, PointerEventData> OnDrag = delegate { };
    public event Action<BagItem, PointerEventData> OnDrop = delegate { };
    public Item item;
    public int amount;
    private Image image;
    private TextMeshProUGUI itemLevel;
    private TextMeshProUGUI itemCount;
    public void SetItem(Item item, int amount)
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
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDrag(this, eventData);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        OnDrag(this, eventData);
    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        OnEndDrag(this, eventData);
    }
    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        OnDrop(this, eventData);
    }
}
