using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
class LoadManager
{
    public static Dictionary<ChunkPos, ChunkData> chunks = new Dictionary<ChunkPos, ChunkData>();
    private static readonly string FILE_PATH = Path.Combine(Application.persistentDataPath, "data.igy");
    public static PlayerData playerData;

    public static void SavePlayer()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (playerData == null) playerData = new PlayerData();
        Player player = TagResolver.i.player;
        SetPlayerPosition(player.transform.position);
        if (CameraSettings.isFirstPerson)
        {
            SetPlayerRotation(CameraSettings.CurrentCamera.transform.localRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y);
        }
        FileStream stream = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
        bf.Serialize(stream, playerData);
        stream.Close();
    }

    public static void SaveInventory()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (playerData == null) playerData = new PlayerData();
        SetInventoryStacks(TagResolver.i.inventory.GetStacks());
        FileStream stream = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
        bf.Serialize(stream, playerData);
        stream.Close();
    }

    public static void SaveDock()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (playerData == null) playerData = new PlayerData();
        Item[] items = new Item[DockUI.DOCK_SLOTS_COUNT];
        Inventory inventory = TagResolver.i.inventory;
        for (int index = 0; index < DockUI.DOCK_SLOTS_COUNT; index++)
        {
            ItemSlot slot = inventory.dockUI.GetItemSlotForIndex(index);
            if (slot.hasItem)
            {
                items[index] = slot.item;
            }
            else
            {
                items[index] = null;
            }
        }
        LoadManager.SetDockItems(items);
        FileStream stream = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
        bf.Serialize(stream, playerData);
        stream.Close();
    }

    public static void SaveAll()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (playerData == null) playerData = new PlayerData();

        Player player = TagResolver.i.player;

        SetPlayerPosition(player.transform.position);
        SetPlayerRotation(player.transform.Find("MainCamera").localRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y);
        SetInventoryStacks(TagResolver.i.inventory.GetStacks());

        FileStream stream = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
        bf.Serialize(stream, playerData);
        stream.Close();
    }

    public static PlayerData Load()
    {
        if (File.Exists(FILE_PATH))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(FILE_PATH, FileMode.Open);
            PlayerData data = bf.Deserialize(stream) as PlayerData;
            playerData = data;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }

    private static bool LoadPlayer(BinaryFormatter bf)
    {
        if (!File.Exists(FILE_PATH))
        {
            return false;
        }

        using (FileStream stream = new FileStream(FILE_PATH, FileMode.Open, FileAccess.Read))
        {
            playerData = bf.Deserialize(stream) as PlayerData;
        }

        return true;
    }
    private static void SetPlayerPosition(Vector3 playerPosition)
    {
        if (playerData == null) playerData = new PlayerData();
        var pp = new float[3];
        pp[0] = playerPosition.x;
        pp[1] = playerPosition.y;
        pp[2] = playerPosition.z;
        playerData.playerPosition = pp;
    }
    public static Vector3 GetPlayerPosition()
    {
        if (playerData == null) playerData = new PlayerData();
        return new Vector3(playerData.playerPosition[0], playerData.playerPosition[1], playerData.playerPosition[2]);
    }

    private static void SetPlayerRotation(float cameraRotationX, float playerRotationY)
    {
        if (playerData == null) playerData = new PlayerData();
        playerData.cameraRotationX = cameraRotationX;
        playerData.playerRotationY = playerRotationY;
    }
    public static (float, float) GetPlayerRotation()
    {
        if (playerData == null) playerData = new PlayerData();
        return (playerData.cameraRotationX, playerData.playerRotationY);
    }
    public static void SaveBlock(Block block, BlockPos pos)
    {
        if (playerData == null) playerData = new PlayerData();
        SavedBlock blockData = new SavedBlock(block, pos);
        if (playerData.blocks == null)
        {
            playerData.blocks = new List<SavedBlock>();
            playerData.blocks.Add(blockData);
        }
        else
        {
            var index = 0;
            bool found = false;
            while (index < playerData.blocks.Count)
            {
                SavedBlock existingBlock = playerData.blocks[index];
                if (existingBlock.blockPos.Equals(pos))
                {
                    playerData.blocks[index] = blockData;
                    found = true;
                    break;
                }
                index++;
            }
            if (!found)
            {
                playerData.blocks.Add(blockData);
            }
        }
    }
    public static Block? GetSavedBlock(BlockPos pos)
    {
        if (playerData == null) playerData = new PlayerData();

        if (playerData.blocks == null)
        {
            return null;
        }
        else
        {
            var index = 0;
            while (index < playerData.blocks.Count)
            {
                SavedBlock existingBlock = playerData.blocks[index];
                if (existingBlock.blockPos.Equals(pos))
                {
                    Block block = new Block((BlockType)existingBlock.type, existingBlock.level);
                    return block;
                }
                index++;
            }
            return null;
        }
    }

    private static void SetInventoryStacks(List<Stack> stacks)
    {
        if (playerData == null) playerData = new PlayerData();
        playerData.items = new List<SavedItem>();
        foreach (Stack stack in stacks)
        {
            playerData.items.Add(new SavedItem(stack.item, stack.amount));
        }
    }
    public static List<Stack> GetInventoryStacks()
    {
        if (playerData == null) playerData = new PlayerData();
        if (playerData.items == null) return new List<Stack>();
        List<Stack> stacks = new List<Stack>();
        foreach (SavedItem savedItem in playerData.items)
        {
            stacks.Add(new Stack(savedItem.GetItem(), savedItem.amount));
        }
        return stacks;
    }
    private static void SetDockItems(Item[] items)
    {
        if (playerData == null) playerData = new PlayerData();
        if (playerData.dockItems == null) playerData.dockItems = new SavedDockItem[DockUI.DOCK_SLOTS_COUNT];
        if (items.Length != DockUI.DOCK_SLOTS_COUNT)
        {
            Debug.LogError("Failed to save dock items. Slots count is not correct");
            return;
        }
        playerData.dockItems = new SavedDockItem[DockUI.DOCK_SLOTS_COUNT];
        for (int index = 0; index < DockUI.DOCK_SLOTS_COUNT; index++)
        {
            playerData.dockItems[index] = new SavedDockItem(items[index]);
        }
    }
    public static Item[] GetDockItems()
    {
        if (playerData == null) playerData = new PlayerData();
        if (playerData.dockItems == null) return new Item[] { };
        if (playerData.dockItems.Length != DockUI.DOCK_SLOTS_COUNT)
        {
            Debug.LogError("Failed to load dock items. Saved data has different count");
            return new Item[] { };
        }
        Item[] dockItems = new Item[DockUI.DOCK_SLOTS_COUNT];
        for (int index = 0; index < DockUI.DOCK_SLOTS_COUNT; index++)
        {
            if (playerData.dockItems[index] == null || playerData.dockItems[index].type == -1)
            {
                dockItems[index] = null;
            }
            else
            {
                dockItems[index] = Item.Make((ItemType)playerData.dockItems[index].type, playerData.dockItems[index].level);
            }
        }
        return dockItems;
    }
};

[Serializable]
public class PlayerData
{
    [SerializeField] public float[] playerPosition;
    [SerializeField] public float cameraRotationX;
    [SerializeField] public float playerRotationY;
    [SerializeField] public List<SavedItem> items;
    [SerializeField] public List<SavedBlock> blocks;
    [SerializeField] public SavedDockItem[] dockItems;
}

[Serializable]
public class SavedBlock
{
    public int[] pos;
    public int type;
    public int level;

    public BlockPos blockPos
    {
        get
        {
            return new BlockPos(pos[0], pos[1], pos[2]);
        }
    }
    public SavedBlock(Block block, BlockPos pos)
    {
        var _pos = new int[3];
        _pos[0] = pos.x;
        _pos[1] = pos.y;
        _pos[2] = pos.z;
        this.pos = _pos;
        this.type = (sbyte)block.type;
        this.level = block.level;
    }

    public SavedBlock(int x, int y, int z, BlockType type, int level)
    {
        var _pos = new int[3];
        _pos[0] = x;
        _pos[1] = y;
        _pos[2] = z;
        this.pos = _pos;
        this.type = (sbyte)type;
        this.level = level;
    }

    public override string ToString()
    {
        return "SavedBlock: " + pos + " " + type + " " + level;
    }
}

[Serializable]
public class SavedItem
{
    public int type;
    public int level;
    public int amount;
    public SavedItem(ItemType type, int level, int amount)
    {
        this.type = (sbyte)type;
        this.level = level;
        this.amount = amount;
    }
    public SavedItem(Item item, int amount)
    {
        this.type = (sbyte)item.type;
        this.level = item.level;
        this.amount = amount;
    }
    public Item GetItem()
    {
        return Item.Make((ItemType)type, level);
    }
    public override string ToString()
    {
        return "SavedItem: " + type + " " + level + " " + amount;
    }
}

[Serializable]
public class SavedDockItem
{
    public int type;
    public int level;
    public SavedDockItem(Item item)
    {
        if (item != null)
        {
            this.type = (sbyte)item.type;
            this.level = item.level;
        }
        else
        {
            this.type = -1;
            this.level = 1;
        }

    }
    public Item GetItem()
    {
        return Item.Make((ItemType)type, level);
    }
    public override string ToString()
    {
        return "SavedDockItem: " + type + " " + level;
    }
}