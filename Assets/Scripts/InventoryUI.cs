using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform dragableItem;
    [SerializeField] private RectTransform itemTemplate;
    [SerializeField] private Transform bagContainer;
    [SerializeField] private Transform itemsUIContainer;
    [HideInInspector] public bool hasDragable = false;
    [HideInInspector] public UIDragableItem movingDragableItem;

    private Inventory inventory;
    public void UpdateWithInventoryChange(Inventory inv)
    {
        inventory = inv;
        RefreshInventoryItems();
    }
    private BagItem MakeBagItem(Item item, int amount)
    {
        RectTransform itemSlotRectTransform = Instantiate(itemTemplate, bagContainer);
        itemSlotRectTransform.gameObject.SetActive(true);
        BagItem bagItem = itemSlotRectTransform.GetComponent<BagItem>();
        bagItem.SetItem(item, amount);
        return bagItem;
    }
    private void RefreshInventoryItems()
    {
        foreach (Transform child in bagContainer)
        {
            if (child == itemTemplate) continue;
            Destroy(child.gameObject);
        }
        int x = 0;
        int y = 0;
        float itemSlotCellSIze = 60f;

        foreach (Stack stack in inventory.GetStacks())
        {

            BagItem bagItem = MakeBagItem(stack.item, stack.amount);
            bagItem.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * itemSlotCellSIze, -y * itemSlotCellSIze);

            bagItem.OnBeginDrag += BeginDragBagItem;
            bagItem.OnEndDrag += EndDragBagItem;
            bagItem.OnDrag += DragBagItem;
            bagItem.OnDrop += DropBagItem;

            x++;
            if (x * itemSlotCellSIze + itemSlotCellSIze >= bagContainer.GetComponent<RectTransform>().rect.width)
            {
                x = 0;
                y++;
            }
        }
    }

    private void BeginDragBagItem(BagItem item, PointerEventData eventData)
    {
        RectTransform dragableItemTransform = Instantiate(dragableItem, itemsUIContainer).GetComponent<RectTransform>();
        dragableItemTransform.gameObject.SetActive(true);
        movingDragableItem = dragableItemTransform.GetComponent<UIDragableItem>();
        hasDragable = true;
        movingDragableItem.SetItem(item.item, item.amount);
        movingDragableItem.OnBeginDrag(eventData);
    }
    private void EndDragBagItem(BagItem item, PointerEventData eventData)
    {
        movingDragableItem.OnEndDrag(eventData);
        Destroy(movingDragableItem.gameObject);
        hasDragable = false;

    }
    private void DragBagItem(BagItem item, PointerEventData eventData)
    {
        movingDragableItem.OnDrag(eventData);
    }
    private void DropBagItem(BagItem item, PointerEventData eventData)
    {
    }

}
