using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerDownHandler
{
    public event Action<ItemSlot, PointerEventData> OnBeginDrag = delegate { };
    public event Action<ItemSlot, PointerEventData> OnEndDrag = delegate { };
    public event Action<ItemSlot, PointerEventData> OnDrag = delegate { };
    public event Action<ItemSlot, PointerEventData> OnDrop = delegate { };
    public ItemSlotPositionType positionType;
    [HideInInspector] public Item item;
    public int? amount;
    public int index;
    public string key;
    public bool hasItem = false;
    private Image image;
    private TextMeshProUGUI itemLevel;
    private TextMeshProUGUI itemCount;
    private TextMeshProUGUI slotKey;
    private void InstantiateChildrenIfNeeded()
    {
        if (image == null) image = transform.Find("ItemBackground").gameObject.GetComponent<Image>();
        if (itemLevel == null) itemLevel = transform.Find("ItemLevel").GetComponent<TextMeshProUGUI>();
        if (itemCount == null) itemCount = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
        if (slotKey == null) slotKey = transform.Find("SlotKey").GetComponent<TextMeshProUGUI>();
    }
    public void SetItem(Item item)
    {
        if (item != null)
        {
            this.item = item;
            InstantiateChildrenIfNeeded();
            image.gameObject.SetActive(true);
            itemLevel.gameObject.SetActive(true);
            slotKey.gameObject.SetActive(true);
            image.sprite = item.image;
            itemLevel.SetText(item.level.ToString());
            slotKey.SetText(key);
            hasItem = true;
        }
        else
        {
            this.item = null;
            InstantiateChildrenIfNeeded();
            image.gameObject.SetActive(false);
            itemLevel.gameObject.SetActive(false);
            itemCount.gameObject.SetActive(false);
            slotKey.gameObject.SetActive(false);
            itemLevel.SetText("");
            slotKey.SetText("");
            hasItem = false;
        }
    }

    public void SetAmount(int? amount)
    {
        if (amount != null)
        {
            this.amount = amount;
            InstantiateChildrenIfNeeded();
            itemCount.gameObject.SetActive(true);
            itemCount.SetText(amount.ToString());
        }
        else
        {
            this.amount = null;
            InstantiateChildrenIfNeeded();
            itemCount.gameObject.SetActive(false);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        TagResolver.i.inventory.GetDockUI().SetSelection(index);
    }
}
