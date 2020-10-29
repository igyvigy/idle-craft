using System.Collections.Generic;

[System.Serializable]
public class Stack
{
    public static Dictionary<ItemType, int> maxStackAmountForBlockType = new Dictionary<ItemType, int> {
        {ItemType.BlockGrass, 100},
        {ItemType.BlockDirt, 100},
        {ItemType.BlockLeaves, 100},
        {ItemType.BlockStone, 100},
        {ItemType.BlockTrunk, 100},
    };
    public Item item;
    public int amount = 0;
    public Stack(Item item)
    {
        this.item = item;
    }
    public Stack(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
    public bool CanIncreaseAmount()
    {
        return this.amount < maxStackAmountForBlockType[item.type];
    }

    public bool CanDecreaseAmount(int amount)
    {
        return this.amount >= amount;
    }

    public bool IncreaseAmount(int amount)
    {
        if (CanIncreaseAmount())
        {
            this.amount += amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DecreaseAmount(int amount)
    {
        if (CanDecreaseAmount(amount))
        {
            this.amount -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
}
