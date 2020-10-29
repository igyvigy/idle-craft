using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxStacks = 45;
    [SerializeField]
    private List<Stack> stacks = new List<Stack>();
    public DockUI dockUI;
    public InventoryUI invUI;
    private GamepadInputManager inputManager;
    private bool? lastSelectButtonValue = null;
    private bool isHoldingSelectButton = false;
    private bool loadedDock = false;
    private void Start()
    {
        inputManager = TagResolver.i.inputManager;
        stacks = LoadManager.GetInventoryStacks();
        if (stacks.Count > 0)
        {
            invUI.UpdateWithInventoryChange(this);
        }

        Item[] dockItems = LoadManager.GetDockItems();
        if (dockItems.Length > 0)
        {
            for (int index = 0; index < DockUI.DOCK_SLOTS_COUNT; index++)
            {
                Item item = dockItems[index];
                if (item != null)
                {
                    ItemSlot slot = dockUI.GetItemSlotForIndex(index);
                    slot.SetItem(item);
                }
            }
        }
        dockUI.UpdateWithInventoryChange(this);
    }
    public bool HasSelectedItem()
    {
        return dockUI.HasSelectedItem();
    }
    public Item GetSelectedItem()
    {
        return dockUI.GetSelectedItem();
    }
    public List<Stack> GetStacks()
    {
        return stacks;
    }
    public void ReduceSelectedBlockAmount()
    {
        Stack stackForBlock = stacks.Find(s => s.item.Equals(GetSelectedItem()) && s.CanDecreaseAmount(1));
        if (stackForBlock != null)
        {
            bool stackStillHasBlocks = stackForBlock.DecreaseAmount(1);
            if (!stackStillHasBlocks || stackForBlock.amount == 0)
            {
                stacks.Remove(stackForBlock);
            }
        }
        invUI.UpdateWithInventoryChange(this);
        dockUI.UpdateWithInventoryChange(this);
        LoadManager.SaveInventory();
    }
    public bool Add(Item item, int amount = 1)
    {
        Stack stackForBlock = stacks.Find(s => s.item.Equals(item) && s.CanIncreaseAmount());
        bool canIncreaseAmount = false;
        if (stackForBlock != null)
        {
            canIncreaseAmount = stackForBlock.IncreaseAmount(amount);
        }
        if (stackForBlock == null || !canIncreaseAmount)
        {
            if (stacks.Count < maxStacks)
            {
                stackForBlock = new Stack(item);
                stacks.Add(stackForBlock);
                canIncreaseAmount = stackForBlock.IncreaseAmount(amount);
            }
            else
            {
                canIncreaseAmount = false;
            }
        }
        invUI.UpdateWithInventoryChange(this);
        dockUI.UpdateWithInventoryChange(this);
        LoadManager.SaveInventory();
        return canIncreaseAmount;
    }

    public InventoryUI GetInventoryUI()
    {
        return invUI;
    }
    public DockUI GetDockUI()
    {
        return dockUI;
    }

    public void ShowBag(bool isShowing)
    {
        invUI.gameObject.SetActive(isShowing);
        if (isShowing)
        {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Confined;
#endif
            Cursor.visible = true;
        }
        else if (CameraSettings.isFirstPerson)
        {
#if (!UNITY_ANDROID && !UNITY_IOS) || UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
            Cursor.visible = false;
        }
        LoadManager.SavePlayer();
    }
    public bool isInventoryUIVisible
    {
        get
        {
            return invUI.gameObject.activeInHierarchy;
        }
    }
    void Update()
    {
        if (inputManager != null)
        {
            bool isSelectButtonPressed = inputManager.SelectButtonValue;
            if (lastSelectButtonValue == null)
            {
                lastSelectButtonValue = isSelectButtonPressed;
            }
            if (isSelectButtonPressed != lastSelectButtonValue)
            {
                if (isSelectButtonPressed)
                {
                    isHoldingSelectButton = true;
                }
                else
                {
                    if (isHoldingSelectButton)
                    {
                        isHoldingSelectButton = false;
                        ShowBag(!isInventoryUIVisible);
                    }
                }
            }
            lastSelectButtonValue = isSelectButtonPressed;
        }
    }
};
