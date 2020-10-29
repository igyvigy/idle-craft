using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class DockUI : MonoBehaviour
{
    public const int DOCK_SLOTS_COUNT = 9;
    public int selectedSlotIndex = 0;
    [SerializeField] private RectTransform dragableItem;
    [SerializeField] private Transform itemsUIContainer;
    [HideInInspector] public UIDragableItem movingDragableItem;
    [HideInInspector] public bool hasDragable = false;
    private Inventory inventory;
    private GamepadInputManager inputManager;
    private RectTransform selection;
    private void Awake()
    {

    }
    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        selection = transform.Find("DockSelection").GetComponent<RectTransform>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetSelection(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetSelection(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetSelection(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SetSelection(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SetSelection(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            SetSelection(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            SetSelection(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            SetSelection(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            SetSelection(8);
    }

    public ItemSlot GetItemSlotForIndex(int index)
    {
        return transform.Find(string.Format("DockSlot{0}", index + 1)).GetComponent<ItemSlot>();
    }
    public bool HasSelectedItem()
    {
        ItemSlot slot = GetItemSlotForIndex(selectedSlotIndex);
        Stack stackForItem = inventory.GetStacks().Find(stack => stack.item.Equals(slot.item));
        if (stackForItem != null)
        {
            return slot.hasItem && stackForItem.amount > 0;
        }
        else
        {
            return false;
        }
    }
    public Item GetSelectedItem()
    {
        return GetItemSlotForIndex(selectedSlotIndex).item;
    }

    public void SetSlotItemAndSave(ItemSlot slot, Item item)
    {
        slot.SetItem(item);
        LoadManager.SaveDock();
    }

    public void SetSlotAmount(ItemSlot slot, int? amount)
    {
        slot.SetAmount(amount);
    }

    public void UpdateWithInventoryChange(Inventory inv)
    {
        inventory = inv;
        RefreshInventoryItems();
    }

    public void SetSelection(int index)
    {
        selection.anchoredPosition = new Vector3(60 * index, 0, 0);
        selectedSlotIndex = index;
    }

    private Dictionary<int, ItemSlot> itemSlotSubscribtions = new Dictionary<int, ItemSlot>();

    public void SubscribeSlot(ItemSlot slot)
    {
        slot.OnBeginDrag += BeginDragItemSlot;
        slot.OnDrag += DragItemSlot;
        slot.OnDrop += DropItemSlot;
        slot.OnEndDrag += EndDragItemSlot;
    }
    public void UnSubscribeSlot(ItemSlot slot)
    {
        slot.OnBeginDrag -= BeginDragItemSlot;
        slot.OnDrag -= DragItemSlot;
        slot.OnDrop -= DropItemSlot;
        slot.OnEndDrag -= EndDragItemSlot;
    }
    private void SubscribeForSlotIfNeeded(ItemSlot slot)
    {
        if (!itemSlotSubscribtions.ContainsKey(slot.index))
        {
            itemSlotSubscribtions.Add(slot.index, slot);
            SubscribeSlot(slot);
        }
    }
    private void UnsubscribeFromSlotIfSubscribed(ItemSlot slot)
    {
        if (itemSlotSubscribtions.ContainsKey(slot.index))
        {
            UnSubscribeSlot(slot);
            itemSlotSubscribtions.Remove(slot.index);
        }
    }
    private void RefreshInventoryItems()
    {
        for (int number = 1; number <= DOCK_SLOTS_COUNT; number++)
        {
            ItemSlot slot = transform.Find(string.Format("DockSlot{0}", number)).GetComponent<ItemSlot>();
            if (slot.hasItem)
            {
                SubscribeForSlotIfNeeded(slot);
                List<Stack> stacksForItem = inventory.GetStacks().FindAll(stack => stack.item.Equals(slot.item));
                if (stacksForItem != null && stacksForItem.Count > 0)
                {
                    List<int> amounts = stacksForItem.ConvertAll(stack => stack.amount);
                    int totalAmount = 0;
                    foreach (int amount in amounts)
                    {
                        totalAmount += amount;
                    }
                    SetSlotAmount(slot, totalAmount);
                }
                else
                {
                    SetSlotItemAndSave(slot, null);
                }
            }
        }
    }

    private void BeginDragItemSlot(ItemSlot itemSlot, PointerEventData eventData)
    {
        RectTransform dragableItemTransform = Instantiate(dragableItem, itemsUIContainer).GetComponent<RectTransform>();
        dragableItemTransform.gameObject.SetActive(true);
        movingDragableItem = dragableItemTransform.GetComponent<UIDragableItem>();
        hasDragable = true;
        movingDragableItem.SetItem(itemSlot.item);
        movingDragableItem.OnBeginDrag(eventData);
    }
    private void EndDragItemSlot(ItemSlot itemSlot, PointerEventData eventData)
    {
        movingDragableItem.OnEndDrag(eventData);
        hasDragable = false;
        Destroy(movingDragableItem.gameObject);
        if (eventData.pointerEnter == null)
        {
            // throw away from dock
            SetSlotItemAndSave(itemSlot, null);
        }
    }
    private void DragItemSlot(ItemSlot itemSlot, PointerEventData eventData)
    {
        movingDragableItem.OnDrag(eventData);
    }
    private void DropItemSlot(ItemSlot itemSlot, PointerEventData eventData)
    {
    }
}
