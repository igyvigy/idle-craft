using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDropReciver : MonoBehaviour, IDropHandler
{
    private Inventory inventory;
    private ItemSlot itemSlot;
    private RectTransform rectTransform;
    void Start()
    {
        inventory = TagResolver.i.inventory;
        itemSlot = GetComponent<ItemSlot>();
        rectTransform = GetComponent<RectTransform>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        InventoryUI invUI = inventory.GetInventoryUI();
        DockUI dockUI = inventory.GetDockUI();
        UIDragableItem inventoryDragable = invUI.movingDragableItem;
        BagItem preBagItem = eventData.pointerDrag.GetComponent<BagItem>();
        ItemSlot preSlot = eventData.pointerDrag.GetComponent<ItemSlot>();

        if (preBagItem != null && preBagItem.gameObject.activeInHierarchy && invUI.hasDragable)
        {
            inventoryDragable.OnPlacementConfirm(this);
            inventoryDragable.GetComponent<RectTransform>().position = rectTransform.position;
            if (itemSlot != null)
            {
                dockUI.SetSlotItemAndSave(itemSlot, inventoryDragable.item);
                switch (itemSlot.positionType)
                {
                    case ItemSlotPositionType.Dock:
                        break;
                    case ItemSlotPositionType.Wear:
                        break;
                    case ItemSlotPositionType.Bag:
                        break;
                }
                dockUI.UpdateWithInventoryChange(inventory);
            }
        }
        UIDragableItem dockDragable = dockUI.movingDragableItem;
        if (preSlot != null && preSlot.gameObject.activeInHierarchy && dockUI.hasDragable)
        {
            dockDragable.OnPlacementConfirm(this);
            dockDragable.GetComponent<RectTransform>().position = rectTransform.position;
            if (itemSlot != null)
            {
                Item itemWhichWasInTheSlotBeforeReplacement = itemSlot.item;
                inventory.dockUI.SetSlotItemAndSave(itemSlot, dockDragable.item);
                switch (itemSlot.positionType)
                {
                    case ItemSlotPositionType.Dock:
                        // move from one dock slot to another
                        inventory.dockUI.SetSlotItemAndSave(preSlot, itemWhichWasInTheSlotBeforeReplacement);
                        break;
                    case ItemSlotPositionType.Wear:
                        break;
                    case ItemSlotPositionType.Bag:
                        break;
                }
                // update the dock
                dockUI.UpdateWithInventoryChange(inventory);
            }
        }
    }
}
